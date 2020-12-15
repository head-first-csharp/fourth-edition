/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SR = System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.Unity.VisualStudio.Editor
{
    public enum ScriptingLanguage
    {
        None,
        CSharp
    }

    public interface IGenerator
    {
        bool SyncIfNeeded(IEnumerable<string> affectedFiles, IEnumerable<string> reimportedFiles);
        void Sync();
        bool HasSolutionBeenGenerated();
        bool IsSupportedFile(string path);
        string SolutionFile();
        string ProjectDirectory { get; }
        IAssemblyNameProvider AssemblyNameProvider { get; }
    }

    public class ProjectGeneration : IGenerator
    {
        public static readonly string MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";

        const string k_WindowsNewline = "\r\n";

        string m_SolutionProjectEntryTemplate = @"Project(""{{{0}}}"") = ""{1}"", ""{2}"", ""{{{3}}}""{4}EndProject";

        string m_SolutionProjectConfigurationTemplate = string.Join("\r\n",
            @"        {{{0}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
            @"        {{{0}}}.Debug|Any CPU.Build.0 = Debug|Any CPU",
            @"        {{{0}}}.Release|Any CPU.ActiveCfg = Release|Any CPU",
            @"        {{{0}}}.Release|Any CPU.Build.0 = Release|Any CPU").Replace("    ", "\t");

        static readonly string[] k_ReimportSyncExtensions = { ".dll", ".asmdef" };

        static readonly Regex k_ScriptReferenceExpression = new Regex(
            @"^Library.ScriptAssemblies.(?<dllname>(?<project>.*)\.dll$)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        string[] m_ProjectSupportedExtensions = new string[0];
        string[] m_BuiltinSupportedExtensions = new string[0];

        public string ProjectDirectory { get; }

        readonly string m_ProjectName;
        readonly IAssemblyNameProvider m_AssemblyNameProvider;
        readonly IFileIO m_FileIOProvider;
        readonly IGUIDGenerator m_GUIDGenerator;
        VisualStudioInstallation m_CurrentInstallation;
        public IAssemblyNameProvider AssemblyNameProvider => m_AssemblyNameProvider;

        public ProjectGeneration() : this(Directory.GetParent(Application.dataPath).FullName)
        {
        }

        public ProjectGeneration(string tempDirectory) : this(tempDirectory, new AssemblyNameProvider(), new FileIOProvider(), new GUIDProvider())
        {
        }

        public ProjectGeneration(string tempDirectory, IAssemblyNameProvider assemblyNameProvider, IFileIO fileIoProvider, IGUIDGenerator guidGenerator)
        {
            ProjectDirectory = tempDirectory.Replace('\\', '/');
            m_ProjectName = Path.GetFileName(ProjectDirectory);
            m_AssemblyNameProvider = assemblyNameProvider;
            m_FileIOProvider = fileIoProvider;
            m_GUIDGenerator = guidGenerator;

            SetupProjectSupportedExtensions();
        }

        /// <summary>
        /// Syncs the scripting solution if any affected files are relevant.
        /// </summary>
        /// <returns>
        /// Whether the solution was synced.
        /// </returns>
        /// <param name='affectedFiles'>
        /// A set of files whose status has changed
        /// </param>
        /// <param name="reimportedFiles">
        /// A set of files that got reimported
        /// </param>
        public bool SyncIfNeeded(IEnumerable<string> affectedFiles, IEnumerable<string> reimportedFiles)
        {
            SetupProjectSupportedExtensions();

            // Don't sync if we haven't synced before
            if (HasSolutionBeenGenerated() && HasFilesBeenModified(affectedFiles, reimportedFiles))
            {
                Sync();
                return true;
            }
            return false;
        }

        bool HasFilesBeenModified(IEnumerable<string> affectedFiles, IEnumerable<string> reimportedFiles)
        {
            return affectedFiles.Any(ShouldFileBePartOfSolution) || reimportedFiles.Any(ShouldSyncOnReimportedAsset);
        }

        static bool ShouldSyncOnReimportedAsset(string asset)
        {
            return k_ReimportSyncExtensions.Contains(new FileInfo(asset).Extension);
        }

        private void RefreshCurrentInstallation()
        {
            var editor = CodeEditor.CurrentEditor as VisualStudioEditor;
            editor?.TryGetVisualStudioInstallationForPath(CodeEditor.CurrentEditorInstallation, out m_CurrentInstallation);
        }

        public void Sync()
        {
            // We need the exact VS version/capabilities to tweak project generation (analyzers/langversion)
            RefreshCurrentInstallation();

            SetupProjectSupportedExtensions();
            var externalCodeAlreadyGeneratedProjects = OnPreGeneratingCSProjectFiles();

            if (!externalCodeAlreadyGeneratedProjects)
            {
                GenerateAndWriteSolutionAndProjects();
            }
            OnGeneratedCSProjectFiles();
        }

        public bool HasSolutionBeenGenerated()
        {
            return m_FileIOProvider.Exists(SolutionFile());
        }

        void SetupProjectSupportedExtensions()
        {
            m_ProjectSupportedExtensions = m_AssemblyNameProvider.ProjectSupportedExtensions;
            m_BuiltinSupportedExtensions = EditorSettings.projectGenerationBuiltinExtensions;
        }

        bool ShouldFileBePartOfSolution(string file)
        {
            // Exclude files coming from packages except if they are internalized.
            if (m_AssemblyNameProvider.IsInternalizedPackagePath(file))
            {
                return false;
            }

            return IsSupportedFile(file);
        }

        static string GetExtensionWithoutDot(string path)
        {
            // Prevent re-processing and information loss
            if (!Path.HasExtension(path))
                return path;

            return Path
                .GetExtension(path)
                .TrimStart('.')
                .ToLower();
        }

        public bool IsSupportedFile(string path)
        {
            var extension = GetExtensionWithoutDot(path);

            // Dll's are not scripts but still need to be included
            if (extension == "dll")
                return true;

            if (extension == "asmdef")
                return true;

            if (m_BuiltinSupportedExtensions.Contains(extension))
                return true;

            if (m_ProjectSupportedExtensions.Contains(extension))
                return true;

            return false;
        }

        static ScriptingLanguage ScriptingLanguageFor(Assembly assembly)
        {
            var files = assembly.sourceFiles;

            if (files.Length == 0)
                return ScriptingLanguage.None;

            return ScriptingLanguageFor(files[0]);
        }

        static ScriptingLanguage ScriptingLanguageFor(string path)
        {
            return GetExtensionWithoutDot(path) == "cs" ? ScriptingLanguage.CSharp : ScriptingLanguage.None;
        }

        public void GenerateAndWriteSolutionAndProjects()
        {
            // Only synchronize assemblies that have associated source files and ones that we actually want in the project.
            // This also filters out DLLs coming from .asmdef files in packages.
            var assemblies = m_AssemblyNameProvider.GetAssemblies(ShouldFileBePartOfSolution);

            var allAssetProjectParts = GenerateAllAssetProjectParts();

            var assemblyList = assemblies.ToList();

            SyncSolution(assemblyList);
            var allProjectAssemblies = RelevantAssembliesForMode(assemblyList).ToList();
            foreach (Assembly assembly in allProjectAssemblies)
            {
                var responseFileData = ParseResponseFileData(assembly);
                SyncProject(assembly, allAssetProjectParts, responseFileData, allProjectAssemblies);
            }
        }

        IEnumerable<ResponseFileData> ParseResponseFileData(Assembly assembly)
        {
            var systemReferenceDirectories = CompilationPipeline.GetSystemAssemblyDirectories(assembly.compilerOptions.ApiCompatibilityLevel);

            Dictionary<string, ResponseFileData> responseFilesData = assembly.compilerOptions.ResponseFiles.ToDictionary(x => x, x => m_AssemblyNameProvider.ParseResponseFile(
                x,
                ProjectDirectory,
                systemReferenceDirectories
            ));

            Dictionary<string, ResponseFileData> responseFilesWithErrors = responseFilesData.Where(x => x.Value.Errors.Any())
                .ToDictionary(x => x.Key, x => x.Value);

            if (responseFilesWithErrors.Any())
            {
                foreach (var error in responseFilesWithErrors)
                foreach (var valueError in error.Value.Errors)
                {
                    Debug.LogError($"{error.Key} Parse Error : {valueError}");
                }
            }

            return responseFilesData.Select(x => x.Value);
        }

        Dictionary<string, string> GenerateAllAssetProjectParts()
        {
            Dictionary<string, StringBuilder> stringBuilders = new Dictionary<string, StringBuilder>();

            foreach (string asset in m_AssemblyNameProvider.GetAllAssetPaths())
            {
                // Exclude files coming from packages except if they are internalized.
                if (m_AssemblyNameProvider.IsInternalizedPackagePath(asset))
                {
                    continue;
                }

                if (IsSupportedFile(asset) && ScriptingLanguage.None == ScriptingLanguageFor(asset))
                {
                    // Find assembly the asset belongs to by adding script extension and using compilation pipeline.
                    var assemblyName = m_AssemblyNameProvider.GetAssemblyNameFromScriptPath(asset + ".cs");

                    if (string.IsNullOrEmpty(assemblyName))
                    {
                        continue;
                    }

                    assemblyName = Path.GetFileNameWithoutExtension(assemblyName);

                    if (!stringBuilders.TryGetValue(assemblyName, out var projectBuilder))
                    {
                        projectBuilder = new StringBuilder();
                        stringBuilders[assemblyName] = projectBuilder;
                    }

                    projectBuilder.Append("     <None Include=\"").Append(EscapedRelativePathFor(asset)).Append("\" />").Append(k_WindowsNewline);
                }
            }

            var result = new Dictionary<string, string>();

            foreach (var entry in stringBuilders)
                result[entry.Key] = entry.Value.ToString();

            return result;
        }

        void SyncProject(
            Assembly assembly,
            Dictionary<string, string> allAssetsProjectParts,
            IEnumerable<ResponseFileData> responseFilesData,
            List<Assembly> allProjectAssemblies)
        {
            SyncProjectFileIfNotChanged(ProjectFile(assembly), ProjectText(assembly, allAssetsProjectParts, responseFilesData, allProjectAssemblies));
        }

        void SyncProjectFileIfNotChanged(string path, string newContents)
        {
            if (Path.GetExtension(path) == ".csproj")
            {
                newContents = OnGeneratedCSProject(path, newContents);
            }

            SyncFileIfNotChanged(path, newContents);
        }

        void SyncSolutionFileIfNotChanged(string path, string newContents)
        {
            newContents = OnGeneratedSlnSolution(path, newContents);

            SyncFileIfNotChanged(path, newContents);
        }

        static IEnumerable<SR.MethodInfo> GetPostProcessorCallbacks(string name)
        {
            return TypeCache
                .GetTypesDerivedFrom<AssetPostprocessor>()
                .Select(t => t.GetMethod(name, SR.BindingFlags.Public | SR.BindingFlags.NonPublic | SR.BindingFlags.Static))
                .Where(m => m!= null);
        }

        static void OnGeneratedCSProjectFiles()
        {
            foreach(var method in GetPostProcessorCallbacks(nameof(OnGeneratedCSProjectFiles)))
            {
                method.Invoke(null, Array.Empty<object>());
            }
        }

        static bool OnPreGeneratingCSProjectFiles()
        {
            bool result = false;

            foreach(var method in GetPostProcessorCallbacks(nameof(OnPreGeneratingCSProjectFiles)))
            {
                var retValue = method.Invoke(null, Array.Empty<object>());
                if (method.ReturnType == typeof(bool))	
                {	
                    result |= (bool)retValue;	
                }
            }

            return result;
        }

        static string InvokeAssetPostProcessorGenerationCallbacks(string name, string path, string content)
        {
            foreach(var method in GetPostProcessorCallbacks(name))
            {
                var args = new [] { path, content };
                var returnValue = method.Invoke(null, args);
                if (method.ReturnType == typeof(string))
                {
                    // We want to chain content update between invocations
                    content = (string)returnValue;
                }
            }

            return content;
        }

        static string OnGeneratedCSProject(string path, string content)
        {
            return InvokeAssetPostProcessorGenerationCallbacks(nameof(OnGeneratedCSProject), path, content);
        }

        static string OnGeneratedSlnSolution(string path, string content)
        {
            return InvokeAssetPostProcessorGenerationCallbacks(nameof(OnGeneratedSlnSolution), path, content);
        }

        void SyncFileIfNotChanged(string filename, string newContents)
        {
            try
            {
                if (m_FileIOProvider.Exists(filename) && newContents == m_FileIOProvider.ReadAllText(filename))
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            m_FileIOProvider.WriteAllText(filename, newContents);
        }

        string ProjectText(Assembly assembly,
            Dictionary<string, string> allAssetsProjectParts,
            IEnumerable<ResponseFileData> responseFilesData,
            List<Assembly> allProjectAsemblies)
        {
            var projectBuilder = new StringBuilder(ProjectHeader(assembly, responseFilesData));
            var references = new List<string>();

            projectBuilder.Append(@"  <ItemGroup>").Append(k_WindowsNewline);
            foreach (string file in assembly.sourceFiles)
            {
                if (!IsSupportedFile(file))
                    continue;

                var extension = Path.GetExtension(file).ToLower();
                var fullFile = EscapedRelativePathFor(file);
                if (".dll" != extension)
                {
                    projectBuilder.Append("    <Compile Include=\"").Append(fullFile).Append("\" />").Append(k_WindowsNewline);
                }
                else
                {
                    references.Add(fullFile);
                }
            }
            projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);

            projectBuilder.Append(@"  <ItemGroup>").Append(k_WindowsNewline);

            // Append additional non-script files that should be included in project generation.
            if (allAssetsProjectParts.TryGetValue(assembly.name, out var additionalAssetsForProject))
                projectBuilder.Append(additionalAssetsForProject);

            var responseRefs = responseFilesData.SelectMany(x => x.FullPathReferences.Select(r => r));
            var internalAssemblyReferences = assembly.assemblyReferences
              .Where(i => !i.sourceFiles.Any(ShouldFileBePartOfSolution)).Select(i => i.outputPath);
            var allReferences =
              assembly.compiledAssemblyReferences
                .Union(responseRefs)
                .Union(references)
                .Union(internalAssemblyReferences);
            foreach (var reference in allReferences)
            {
                string fullReference = Path.IsPathRooted(reference) ? reference : Path.Combine(ProjectDirectory, reference);
                AppendReference(fullReference, projectBuilder);
            }

            projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);

            if (0 < assembly.assemblyReferences.Length)
            {
                projectBuilder.Append("  <ItemGroup>").Append(k_WindowsNewline);
                foreach (Assembly reference in assembly.assemblyReferences.Where(i => i.sourceFiles.Any(ShouldFileBePartOfSolution)))
                {
                    projectBuilder.Append("    <ProjectReference Include=\"").Append(reference.name).Append(GetProjectExtension()).Append("\">").Append(k_WindowsNewline);
                    projectBuilder.Append("      <Project>{").Append(ProjectGuid(reference)).Append("}</Project>").Append(k_WindowsNewline);
                    projectBuilder.Append("      <Name>").Append(reference.name).Append("</Name>").Append(k_WindowsNewline);
                    projectBuilder.Append("    </ProjectReference>").Append(k_WindowsNewline);
                }

                projectBuilder.Append(@"  </ItemGroup>").Append(k_WindowsNewline);
            }

            projectBuilder.Append(ProjectFooter());
            return projectBuilder.ToString();
        }

        static string XmlFilename(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            path = path.Replace(@"%", "%25");
            path = path.Replace(@";", "%3b");

            return XmlEscape(path);
        }

        static string XmlEscape(string s)
        {
            return SecurityElement.Escape(s);
        }

        void AppendReference(string fullReference, StringBuilder projectBuilder)
        {
            var escapedFullPath = EscapedRelativePathFor(fullReference);
            projectBuilder.Append("    <Reference Include=\"").Append(Path.GetFileNameWithoutExtension(escapedFullPath)).Append("\">").Append(k_WindowsNewline);
            projectBuilder.Append("      <HintPath>").Append(escapedFullPath).Append("</HintPath>").Append(k_WindowsNewline);
            projectBuilder.Append("    </Reference>").Append(k_WindowsNewline);
        }

        public string ProjectFile(Assembly assembly)
        {
			return Path.Combine(ProjectDirectory, $"{m_AssemblyNameProvider.GetAssemblyName(assembly.outputPath, assembly.name)}.csproj");
		}

        private static readonly Regex InvalidCharactersRegexPattern = new Regex(@"\?|&|\*|""|<|>|\||#|%|\^|;" + (VisualStudioEditor.IsWindows ? "" : "|:"));
        public string SolutionFile()
        {
            return Path.Combine(FileUtility.Normalize(ProjectDirectory), $"{InvalidCharactersRegexPattern.Replace(m_ProjectName,"_")}.sln");
        }

        string ProjectHeader(
            Assembly assembly,
            IEnumerable<ResponseFileData> responseFilesData
        )
        {
            var toolsVersion = "4.0";
            var productVersion = "10.0.20506";
            const string baseDirectory = ".";

            var targetFrameworkVersion = "v4.7.1";
            var targetLanguageVersion = "latest"; // danger: latest is not the same absolute value depending on the VS version.

            if (m_CurrentInstallation != null && m_CurrentInstallation.SupportsCSharp8)
            {
                // Current installation is compatible with C# 8.
                // But Unity has no support for C# 8 constructs so far, so tell the compiler to accept only C# 7.3 or lower.
                targetLanguageVersion = "7.3";
            }

            var projectType = ProjectTypeOf(assembly.name);

            var arguments = new object[]
            {
                toolsVersion,
                productVersion,
                ProjectGuid(assembly),
                XmlFilename(FileUtility.Normalize(InternalEditorUtility.GetEngineAssemblyPath())),
                XmlFilename(FileUtility.Normalize(InternalEditorUtility.GetEditorAssemblyPath())),
                string.Join(";", assembly.defines.Concat(responseFilesData.SelectMany(x => x.Defines)).Distinct().ToArray()),
                MSBuildNamespaceUri,
                assembly.name,
                assembly.outputPath,
                m_AssemblyNameProvider.ProjectGenerationRootNamespace,
                targetFrameworkVersion,
                targetLanguageVersion,
                baseDirectory,
                assembly.compilerOptions.AllowUnsafeCode | responseFilesData.Any(x => x.Unsafe),
                // flavoring
                projectType + ":" + (int)projectType,
                EditorUserBuildSettings.activeBuildTarget + ":" + (int)EditorUserBuildSettings.activeBuildTarget,
                Application.unityVersion,
            };

            try
            {
                return string.Format(GetProjectHeaderTemplate(), arguments);
            }
            catch (Exception)
            {
                throw new NotSupportedException("Failed creating c# project because the c# project header did not have the correct amount of arguments, which is " + arguments.Length);
            }
        }

        private enum ProjectType
        {
            GamePlugins = 3,
            Game = 1,
            EditorPlugins = 7,
            Editor = 5,
        }

        private static ProjectType ProjectTypeOf(string fileName)
        {
            var plugins = fileName.Contains("firstpass");
            var editor = fileName.Contains("Editor");

            if (plugins && editor)
                return ProjectType.EditorPlugins;
            if (plugins)
                return ProjectType.GamePlugins;
            if (editor)
                return ProjectType.Editor;

            return ProjectType.Game;
        }

        static string GetSolutionText()
        {
            return string.Join("\r\n",
            @"",
            @"Microsoft Visual Studio Solution File, Format Version {0}",
            @"# Visual Studio {1}",
            @"{2}",
            @"Global",
            @"    GlobalSection(SolutionConfigurationPlatforms) = preSolution",
            @"        Debug|Any CPU = Debug|Any CPU",
            @"        Release|Any CPU = Release|Any CPU",
            @"    EndGlobalSection",
            @"    GlobalSection(ProjectConfigurationPlatforms) = postSolution",
            @"{3}",
            @"    EndGlobalSection",
            @"{4}",
            @"EndGlobal",
            @"").Replace("    ", "\t");
        }

        static string GetProjectFooterTemplate()
        {
            return string.Join("\r\n",
            @"  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />",
            @"  <Target Name=""GenerateTargetFrameworkMonikerAttribute"" />",
            @"  <!-- To modify your build process, add your task inside one of the targets below and uncomment it.",
            @"       Other similar extension points exist, see Microsoft.Common.targets.",
            @"  <Target Name=""BeforeBuild"">",
            @"  </Target>",
            @"  <Target Name=""AfterBuild"">",
            @"  </Target>",
            @"  -->",
            @"</Project>",
            @"");
        }

        string GetProjectHeaderTemplate()
        {
            var header = new[]
            {
                @"<?xml version=""1.0"" encoding=""utf-8""?>",
                @"<Project ToolsVersion=""{0}"" DefaultTargets=""Build"" xmlns=""{6}"">",
                @"  <PropertyGroup>",
                @"    <LangVersion>{11}</LangVersion>",
                @"  </PropertyGroup>",
                @"  <PropertyGroup>",
                @"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>",
                @"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>",
                @"    <ProductVersion>{1}</ProductVersion>",
                @"    <SchemaVersion>2.0</SchemaVersion>",
                @"    <RootNamespace>{9}</RootNamespace>",
                @"    <ProjectGuid>{{{2}}}</ProjectGuid>",
                @"    <OutputType>Library</OutputType>",
                @"    <AppDesignerFolder>Properties</AppDesignerFolder>",
                @"    <AssemblyName>{7}</AssemblyName>",
                @"    <TargetFrameworkVersion>{10}</TargetFrameworkVersion>",
                @"    <FileAlignment>512</FileAlignment>",
                @"    <BaseDirectory>{12}</BaseDirectory>",
                @"  </PropertyGroup>",
                @"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">",
                @"    <DebugSymbols>true</DebugSymbols>",
                @"    <DebugType>full</DebugType>",
                @"    <Optimize>false</Optimize>",
                @"    <OutputPath>{8}</OutputPath>",
                @"    <DefineConstants>{5}</DefineConstants>",
                @"    <ErrorReport>prompt</ErrorReport>",
                @"    <WarningLevel>4</WarningLevel>",
                @"    <NoWarn>0169</NoWarn>",
                @"    <AllowUnsafeBlocks>{13}</AllowUnsafeBlocks>",
                @"  </PropertyGroup>",
                @"  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">",
                @"    <DebugType>pdbonly</DebugType>",
                @"    <Optimize>true</Optimize>",
                @"    <OutputPath>Temp\bin\Release\</OutputPath>",
                @"    <ErrorReport>prompt</ErrorReport>",
                @"    <WarningLevel>4</WarningLevel>",
                @"    <NoWarn>0169</NoWarn>",
                @"    <AllowUnsafeBlocks>{13}</AllowUnsafeBlocks>",
                @"  </PropertyGroup>"
            };

            var forceExplicitReferences = new[]
            {
                @"  <PropertyGroup>",
                @"    <NoConfig>true</NoConfig>",
                @"    <NoStdLib>true</NoStdLib>",
                @"    <AddAdditionalExplicitAssemblyReferences>false</AddAdditionalExplicitAssemblyReferences>",
                @"    <ImplicitlyExpandNETStandardFacades>false</ImplicitlyExpandNETStandardFacades>",
                @"    <ImplicitlyExpandDesignTimeFacades>false</ImplicitlyExpandDesignTimeFacades>",
                @"  </PropertyGroup>"
            };

            var flavoring = new[]
            {
                @"  <PropertyGroup>",
                @"    <ProjectTypeGuids>{{E097FAD1-6243-4DAD-9C02-E9B9EFC3FFC1}};{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}</ProjectTypeGuids>",
                @"    <UnityProjectGenerator>Package</UnityProjectGenerator>",
                @"    <UnityProjectType>{14}</UnityProjectType>",
                @"    <UnityBuildTarget>{15}</UnityBuildTarget>",
                @"    <UnityVersion>{16}</UnityVersion>",
                @"  </PropertyGroup>"
            };

            var footer = new[]
            {
                @""
            };

            var lines = header
                .Concat(forceExplicitReferences)
                .Concat(flavoring)
                .ToList();

            // Only add analyzer block for compatible Visual Studio
            if (m_CurrentInstallation != null && m_CurrentInstallation.SupportsAnalyzers)
            {
                var analyzers = m_CurrentInstallation.GetAnalyzers();
                if (analyzers != null && analyzers.Length > 0)
                {
                    lines.Add(@"  <ItemGroup>");
                    foreach (var analyzer in analyzers)
                        lines.Add(string.Format(@"    <Analyzer Include=""{0}"" />", EscapedRelativePathFor(analyzer)));
                    lines.Add(@"  </ItemGroup>");
                }
            }

            return string.Join("\r\n", lines
                .Concat(footer));
        }

        void SyncSolution(IEnumerable<Assembly> assemblies)
        {
            if (InvalidCharactersRegexPattern.IsMatch(ProjectDirectory))
                Debug.LogWarning("Project path contains special characters, which can be an issue when opening Visual Studio");

            var solutionFile = SolutionFile();
            var previousSolution = m_FileIOProvider.Exists(solutionFile) ? SolutionParser.ParseSolutionFile(solutionFile, m_FileIOProvider) : null;
            SyncSolutionFileIfNotChanged(solutionFile, SolutionText(assemblies, previousSolution));
        }

        string SolutionText(IEnumerable<Assembly> assemblies, Solution previousSolution = null)
        {
            const string fileversion = "12.00";
            const string vsversion = "15";

            var relevantAssemblies = RelevantAssembliesForMode(assemblies);
            var generatedProjects = ToProjectEntries(relevantAssemblies).ToList();

            SolutionProperties[] properties = null;

            // First, add all projects generated by Unity to the solution
            var projects = new List<SolutionProjectEntry>();
            projects.AddRange(generatedProjects);

            if (previousSolution != null)
            {
                // Add all projects that were previously in the solution and that are not generated by Unity, nor generated in the project root directory
                var externalProjects = previousSolution.Projects
                    .Where(p => p.IsSolutionFolderProjectFactory() || !FileUtility.IsFileInProjectDirectory(p.FileName))
                    .Where(p => generatedProjects.All(gp => gp.FileName != p.FileName));

                projects.AddRange(externalProjects);
                properties = previousSolution.Properties;
            }

            string propertiesText = GetPropertiesText(properties);
            string projectEntriesText = GetProjectEntriesText(projects);

            // do not generate configurations for SolutionFolders
            var configurableProjects = projects.Where(p => !p.IsSolutionFolderProjectFactory());
            string projectConfigurationsText = string.Join(k_WindowsNewline, configurableProjects.Select(p => GetProjectActiveConfigurations(p.ProjectGuid)).ToArray());

            return string.Format(GetSolutionText(), fileversion, vsversion, projectEntriesText, projectConfigurationsText, propertiesText);
        }

        static IEnumerable<Assembly> RelevantAssembliesForMode(IEnumerable<Assembly> assemblies)
        {
            return assemblies.Where(i => ScriptingLanguage.CSharp == ScriptingLanguageFor(i));
        }

        private string GetPropertiesText(SolutionProperties[] array)
        {
            if (array == null || array.Length == 0)
            {
                // HideSolution by default
                array = new SolutionProperties[] {
                    new SolutionProperties() {
                        Name = "SolutionProperties",
                        Type = "preSolution",
                        Entries = new List<KeyValuePair<string,string>>() { new KeyValuePair<string, string> ("HideSolutionNode", "FALSE") }
                    }
                };
            }
            var result = new StringBuilder();

            for (var i = 0; i<array.Length; i++)
            {
                if (i > 0)
                    result.Append(k_WindowsNewline);

                var properties = array[i];

                result.Append($"\tGlobalSection({properties.Name}) = {properties.Type}");
                result.Append(k_WindowsNewline);

                foreach (var entry in properties.Entries)
                {
                    result.Append($"\t\t{entry.Key} = {entry.Value}");
                    result.Append(k_WindowsNewline);
                }

                result.Append("\tEndGlobalSection");
            }

            return result.ToString();
        }

        /// <summary>
        /// Get a Project("{guid}") = "MyProject", "MyProject.unityproj", "{projectguid}"
        /// entry for each relevant language
        /// </summary>
        string GetProjectEntriesText(IEnumerable<SolutionProjectEntry> entries)
        {
            var projectEntries = entries.Select(entry => string.Format(
                m_SolutionProjectEntryTemplate,
                entry.ProjectFactoryGuid, entry.Name, entry.FileName, entry.ProjectGuid, entry.Metadata
            ));

            return string.Join(k_WindowsNewline, projectEntries.ToArray());
        }

        IEnumerable<SolutionProjectEntry> ToProjectEntries(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                yield return new SolutionProjectEntry()
                {
                    ProjectFactoryGuid = SolutionGuid(assembly),
                    Name = assembly.name,
                    FileName = Path.GetFileName(ProjectFile(assembly)),
                    ProjectGuid = ProjectGuid(assembly),
                    Metadata = k_WindowsNewline
                };
        }

        /// <summary>
        /// Generate the active configuration string for a given project guid
        /// </summary>
        string GetProjectActiveConfigurations(string projectGuid)
        {
            return string.Format(
                m_SolutionProjectConfigurationTemplate,
                projectGuid);
        }

        string EscapedRelativePathFor(string file)
        {
            var projectDir = FileUtility.Normalize(ProjectDirectory);
            file = FileUtility.Normalize(file);
            var path = SkipPathPrefix(file, projectDir);

            var packageInfo = m_AssemblyNameProvider.FindForAssetPath(path.Replace('\\', '/'));
            if (packageInfo != null) {
                // We have to normalize the path, because the PackageManagerRemapper assumes
                // dir seperators will be os specific.
                var absolutePath = Path.GetFullPath(FileUtility.Normalize(path));
                path = SkipPathPrefix(absolutePath, projectDir);
            }

            return XmlFilename(path);
        }

        static string SkipPathPrefix(string path, string prefix)
        {
            if (path.StartsWith($"{prefix}{Path.DirectorySeparatorChar}") && (path.Length > prefix.Length))
                return path.Substring(prefix.Length + 1);
            return path;
        }

        static string ProjectFooter()
        {
            return GetProjectFooterTemplate();
        }

        static string GetProjectExtension()
        {
            return ".csproj";
        }

        string ProjectGuid(Assembly assembly)
        {
            return m_GUIDGenerator.ProjectGuid(
				m_ProjectName,
				m_AssemblyNameProvider.GetAssemblyName(assembly.outputPath, assembly.name));
		}

        string SolutionGuid(Assembly assembly)
        {
            return m_GUIDGenerator.SolutionGuid(m_ProjectName, ScriptingLanguageFor(assembly));
        }
    }

    public static class SolutionGuidGenerator
    {
        public static string GuidForProject(string projectName)
        {
            return ComputeGuidHashFor(projectName + "salt");
        }

        public static string GuidForSolution(string projectName, ScriptingLanguage language)
        {
            if (language == ScriptingLanguage.CSharp)
            {
                // GUID for a C# class library: http://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
                return "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
            }

            return ComputeGuidHashFor(projectName);
        }

        static string ComputeGuidHashFor(string input)
        {
            var hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
            return HashAsGuid(HashToString(hash));
        }

        static string HashAsGuid(string hash)
        {
            var guid = hash.Substring(0, 8) + "-" + hash.Substring(8, 4) + "-" + hash.Substring(12, 4) + "-" + hash.Substring(16, 4) + "-" + hash.Substring(20, 12);
            return guid.ToUpper();
        }

        static string HashToString(byte[] bs)
        {
            var sb = new StringBuilder();
            foreach (byte b in bs)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
