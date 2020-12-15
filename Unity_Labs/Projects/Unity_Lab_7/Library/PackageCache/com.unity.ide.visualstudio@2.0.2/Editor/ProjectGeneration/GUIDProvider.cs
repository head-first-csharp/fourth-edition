namespace Microsoft.Unity.VisualStudio.Editor
{
    public interface IGUIDGenerator
    {
        string ProjectGuid(string projectName, string assemblyName);
        string SolutionGuid(string projectName, ScriptingLanguage scriptingLanguage);
    }

    class GUIDProvider : IGUIDGenerator
    {
        public string ProjectGuid(string projectName, string assemblyName)
        {
            return SolutionGuidGenerator.GuidForProject(projectName + assemblyName);
        }

        public string SolutionGuid(string projectName, ScriptingLanguage scriptingLanguage)
        {
            return SolutionGuidGenerator.GuidForSolution(projectName, scriptingLanguage);
        }
    }
}