using UnityEditor;

namespace Unity.Cloud.Collaborate.Settings
{
    /// <summary>
    /// This class will act as a manager for the <see cref="Settings"/> singleton.
    /// </summary>
    internal static class CollabSettingsManager
    {
        // Project settings will be stored in a JSON file in a directory matching this name.
        // const string k_PackageName = "com.unity.collab-proxy";

        // static UnityEditor.SettingsManagement.Settings s_Instance;
        //
        // internal static UnityEditor.SettingsManagement.Settings instance =>
        //     s_Instance ?? (s_Instance = new UnityEditor.SettingsManagement.Settings(k_PackageName));

        // The rest of this file is just forwarding the various setting methods to the instance.

        // public static void Save()
        // {
        //     instance.Save();
        // }

        public static T Get<T>(string key, SettingsScope scope = SettingsScope.Project, T fallback = default)
        {
            return fallback;
            //return instance.Get(key, scope, fallback);
        }

        // public static void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        // {
        //     instance.Set(key, value, scope);
        // }
        //
        // public static bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        // {
        //     return instance.ContainsKey<T>(key, scope);
        // }
    }
}
