using System;
using JetBrains.Annotations;
// using UnityEditor.SettingsManagement;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Settings
{
    [UsedImplicitly]
    internal class CollabSettings
    {
        public enum DisplayMode
        {
            Simple,
            Advanced
        }

        public enum OpenLocation
        {
            Docked,
            Window
        }

        // List of setting keys
        public const string settingRelativeTimestamp = "general.relativeTimestamps";
        // public const string settingAutoFetch = "general.autoFetch";
        // public const string settingDisplayMode = "general.displayMode";
        public const string settingDefaultOpenLocation = "general.defaultOpenLocation";

        // [UserSetting] attribute registers this setting with the UserSettingsProvider so that it can be automatically
        // shown in the UI.
        // [UserSetting("General Settings", "Default Open Location")]
        // [UsedImplicitly]
        // static CollabSetting<OpenLocation> s_DefaultOpenLocation = new CollabSetting<OpenLocation>(settingDefaultOpenLocation, OpenLocation.Docked);
        //
        // [UserSetting("General Settings", "Relative Timestamps")]
        // [UsedImplicitly]
        // static CollabSetting<bool> s_RelativeTimestamps = new CollabSetting<bool>(settingRelativeTimestamp, true);
        //
        // [UserSetting("General Settings", "Automatic Fetch")]
        // [UsedImplicitly]
        // static CollabSetting<bool> s_AutoFetch = new CollabSetting<bool>(settingAutoFetch, true);
        //
        // [UserSetting("General Settings", "Display Mode")]
        // [UsedImplicitly]
        // static CollabSetting<DisplayMode> s_DisplayMode = new CollabSetting<DisplayMode>(settingDisplayMode, DisplayMode.Simple);
    }
}
