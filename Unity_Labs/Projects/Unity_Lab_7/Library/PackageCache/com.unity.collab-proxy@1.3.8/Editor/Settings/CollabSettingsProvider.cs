// using JetBrains.Annotations;
// using UnityEditor;
// using UnityEditor.SettingsManagement;
//
// namespace Unity.Cloud.Collaborate.Settings
// {
//     [UsedImplicitly]
//     internal class CollabSettingsProvider
//     {
//         const string k_PreferencesPath = "Preferences/Collaborate";
//
//         [SettingsProvider]
//         [UsedImplicitly]
//         static SettingsProvider CreateSettingsProvider()
//         {
//             var provider = new UserSettingsProvider(k_PreferencesPath,
//                 CollabSettingsManager.instance,
//                 new [] { typeof(CollabSettingsProvider).Assembly });
//
//             return provider;
//         }
//     }
// }
