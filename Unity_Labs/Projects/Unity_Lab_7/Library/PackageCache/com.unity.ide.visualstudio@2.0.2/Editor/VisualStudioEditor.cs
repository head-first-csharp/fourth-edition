/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.CodeEditor;
using System.Runtime.InteropServices;

namespace Microsoft.Unity.VisualStudio.Editor
{
	[InitializeOnLoad]
	public class VisualStudioEditor : IExternalCodeEditor
	{
		private static readonly VisualStudioInstallation[] _installations;

		internal static bool IsOSX => Application.platform == RuntimePlatform.OSXEditor;
		internal static bool IsWindows => !IsOSX && Path.DirectorySeparatorChar == '\\' && Environment.NewLine == "\r\n";

		CodeEditor.Installation[] IExternalCodeEditor.Installations => _installations
			.Select(i => i.ToCodeEditorInstallation())
			.ToArray(); 

		private readonly IGenerator _generator = new ProjectGeneration();

		static VisualStudioEditor()
		{
			try
			{
				_installations = Discovery
					.GetVisualStudioInstallations()
					.ToArray();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log($"Error detecting Visual Studio installations: {ex}");
				_installations = Array.Empty<VisualStudioInstallation>();
			}

			CodeEditor.Register(new VisualStudioEditor());
		}

		internal static bool IsEnabled
		{
			get
			{
				return CodeEditor.CurrentEditor is VisualStudioEditor;
			}
		}

		public void CreateIfDoesntExist()
		{
			if (!_generator.HasSolutionBeenGenerated())
				_generator.Sync();
		}

		public void Initialize(string editorInstallationPath)
		{
		}

		internal bool TryGetVisualStudioInstallationForPath(string editorPath, out VisualStudioInstallation installation)
		{
			// lookup for well known installations
			foreach (var candidate in _installations)
			{
				if (!string.Equals(Path.GetFullPath(editorPath), Path.GetFullPath(candidate.Path), StringComparison.OrdinalIgnoreCase))
					continue;

				installation = candidate;
				return true;
			}

			return Discovery.TryDiscoverInstallation(editorPath, out installation);
		}

		public bool TryGetInstallationForPath(string editorPath, out CodeEditor.Installation installation)
		{
			var result = TryGetVisualStudioInstallationForPath(editorPath, out var vsi);
			installation = vsi == null ? default : vsi.ToCodeEditorInstallation();
			return result;
		}

		public void OnGUI()
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(GetType().Assembly);

			var style = new GUIStyle
			{
				richText = true,
				margin = new RectOffset(0, 4, 0, 0)
			};

			GUILayout.Label($"<size=10><color=grey>{package.displayName} v{package.version} enabled</color></size>", style);
			GUILayout.EndHorizontal();

			EditorGUILayout.LabelField("Generate .csproj files for:");
			EditorGUI.indentLevel++;
			SettingsButton(ProjectGenerationFlag.Embedded, "Embedded packages", "");
			SettingsButton(ProjectGenerationFlag.Local, "Local packages", "");
			SettingsButton(ProjectGenerationFlag.Registry, "Registry packages", "");
			SettingsButton(ProjectGenerationFlag.Git, "Git packages", "");
			SettingsButton(ProjectGenerationFlag.BuiltIn, "Built-in packages", "");
			SettingsButton(ProjectGenerationFlag.LocalTarBall, "Local tarball", "");
			SettingsButton(ProjectGenerationFlag.Unknown, "Packages from unknown sources", "");
			SettingsButton(ProjectGenerationFlag.PlayerAssemblies, "Player projects", "For each player project generate an additional csproj with the name 'project-player.csproj'");
			RegenerateProjectFiles();
			EditorGUI.indentLevel--;
		}

		void RegenerateProjectFiles()
        {
            var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(new GUILayoutOption[] {}));
            rect.width = 252;
            if (GUI.Button(rect, "Regenerate project files"))
            {
                _generator.Sync();
            }
        }

		void SettingsButton(ProjectGenerationFlag preference, string guiMessage, string toolTip)
		{
			var prevValue = _generator.AssemblyNameProvider.ProjectGenerationFlag.HasFlag(preference);
			var newValue = EditorGUILayout.Toggle(new GUIContent(guiMessage, toolTip), prevValue);
			if (newValue != prevValue)
			{
				_generator.AssemblyNameProvider.ToggleProjectGeneration(preference);
			}
		}

		public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles, string[] importedFiles)
		{
			_generator.SyncIfNeeded(addedFiles.Union(deletedFiles).Union(movedFiles).Union(movedFromFiles), importedFiles);

			foreach (var file in importedFiles.Where(a => Path.GetExtension(a) == ".pdb"))
			{
				var pdbFile = FileUtility.GetAssetFullPath(file);
				var asmFile = Path.ChangeExtension(pdbFile, ".dll");

				if (!File.Exists(asmFile) || !Image.IsAssembly(asmFile))
					continue;

				if (Symbols.IsPortableSymbolFile(pdbFile))
					continue;

				UnityEngine.Debug.LogWarning($"Unity is only able to load mdb or portable-pdb symbols. {file} is using a legacy pdb format.");
			}
		}

		public void SyncAll()
		{
			AssetDatabase.Refresh();
			_generator.Sync();
		}

		bool IsSupportedPath(string path)
		{
			// Path is empty with "Open C# Project", as we only want to open the solution without specific files
			if (string.IsNullOrEmpty(path))
				return true;

			// cs, uxml, uss, shader, compute, cginc, hlsl, glslinc, template are part of Unity builtin extensions
			// txt, xml, fnt, cd are -often- par of Unity user extensions
			// asdmdef is mandatory included
			if (_generator.IsSupportedFile(path))
				return true;

			return false;
		}

		public bool OpenProject(string path, int line, int column)
		{
			if (!IsSupportedPath(path))
				return false;

			if (IsOSX)
				return OpenOSXApp(path, line, column);

			if (IsWindows)
				return OpenWindowsApp(path, line);

			return false;
		}

		private bool OpenWindowsApp(string path, int line)
		{
			var progpath = FileUtility
				.FindPackageAssetFullPath("COMIntegration a:packages", "COMIntegration.exe")
				.FirstOrDefault();

			if (string.IsNullOrWhiteSpace(progpath))
				return false;

			string absolutePath = "";
			if (!string.IsNullOrWhiteSpace(path))
			{
				absolutePath = Path.GetFullPath(path);
			}

			// We remove all invalid chars from the solution filename, but we cannot prevent the user from using a specific path for the Unity project
			// So process the fullpath to make it compatible with VS
			var solution = GetOrGenerateSolutionFile(path);
			if (!string.IsNullOrWhiteSpace(solution))
			{
				solution = $"\"{solution}\"";
				solution = solution.Replace("^", "^^");
			}

			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = progpath,
					Arguments = $"\"{CodeEditor.CurrentEditorInstallation}\" \"{absolutePath}\" {solution} {line}",
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
				}
			};
			var result = process.Start();

			while (!process.StandardOutput.EndOfStream)
			{
				var outputLine = process.StandardOutput.ReadLine();
				if (outputLine == "displayProgressBar")
				{
					EditorUtility.DisplayProgressBar("Opening Visual Studio", "Starting up Visual Studio, this might take some time.", .5f);
				}

				if (outputLine == "clearprogressbar")
				{
					EditorUtility.ClearProgressBar();
				}
			}

			var errorOutput = process.StandardError.ReadToEnd();
			if (!string.IsNullOrEmpty(errorOutput))
			{
				Console.WriteLine("Error: \n" + errorOutput);
			}

			process.WaitForExit();
			return result;
		}

		[DllImport("AppleEventIntegration")]
		static extern bool OpenVisualStudio(string appPath, string solutionPath, string filePath, int line);

		bool OpenOSXApp(string path, int line, int column)
		{
			string absolutePath = "";
			if (!string.IsNullOrWhiteSpace(path))
			{
				absolutePath = Path.GetFullPath(path);
			}

			string solution = GetOrGenerateSolutionFile(path);
			return OpenVisualStudio(CodeEditor.CurrentEditorInstallation, solution, absolutePath, line);
		}

		private string GetOrGenerateSolutionFile(string path)
		{
			var solution = GetSolutionFile(path);
			if (solution == "")
			{
				_generator.Sync();
				solution = GetSolutionFile(path);
			}

			return solution;
		}

		string GetSolutionFile(string path)
		{
			var solutionFile = _generator.SolutionFile();
			if (File.Exists(solutionFile))
			{
				return solutionFile;
			}

			return "";
		}
	}
}
