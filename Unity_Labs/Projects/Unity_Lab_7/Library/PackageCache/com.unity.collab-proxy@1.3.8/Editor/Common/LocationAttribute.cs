using System;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Common {
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class LocationAttribute : Attribute
    {
        public enum Location { PreferencesFolder, LibraryFolder }

        string m_RelativePath;
        readonly Location m_Location;
        string m_FilePath;

        public string FilePath {
            get {
                if (m_FilePath != null) return m_FilePath;

                if (m_RelativePath[0] == '/')
                    m_RelativePath = m_RelativePath.Substring(1);

                if (m_Location == Location.PreferencesFolder)
                    m_FilePath = $"{InternalEditorUtility.unityPreferencesFolder}/{m_RelativePath}";
                else if (m_Location == Location.LibraryFolder)
                    m_FilePath = $"Library/Collab/{m_RelativePath}";

                return m_FilePath;
            }
        }

        public LocationAttribute(string relativePath, Location location)
        {
            //Guard.ArgumentNotNullOrWhiteSpace(relativePath, "relativePath");
            m_RelativePath = relativePath;
            m_Location = location;
        }
    }
}
