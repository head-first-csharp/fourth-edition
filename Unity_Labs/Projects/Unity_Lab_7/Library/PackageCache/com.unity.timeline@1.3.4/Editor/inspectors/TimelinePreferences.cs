using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

/// <summary>
/// Store the editor preferences for Timeline.
/// </summary>
[FilePath("TimelinePreferences.asset", FilePathAttribute.Location.PreferencesFolder)]
public class TimelinePreferences : ScriptableSingleton<TimelinePreferences>
{
    /// <summary>
    /// Define the time unit for the timeline window.
    /// true : frame unit.
    /// false : seconds unit.
    /// </summary>
    [SerializeField]
    public bool timeUnitInFrame = true;
    /// <summary>
    /// Draw the waveforms for all audio clips.
    /// </summary>
    [SerializeField]
    public bool showAudioWaveform = true;

    /// <summary>
    /// Allow the users to hear audio while scrubbing on audio clip.
    /// </summary>
    [SerializeField]
    bool m_AudioScrubbing;
    public bool audioScrubbing
    {
        get { return m_AudioScrubbing; }
        set
        {
            if (m_AudioScrubbing != value)
            {
                m_AudioScrubbing = value;
                TimelinePlayable.muteAudioScrubbing = !value;
                TimelineEditor.Refresh(RefreshReason.ContentsModified);
            }
        }
    }
    /// <summary>
    /// Enable Snap to Frame to manipulate clips and align them on frames.
    /// </summary>
    [SerializeField]
    public bool snapToFrame = true;
    /// <summary>
    /// Enable the ability to snap clips on the edge of another clip.
    /// </summary>
    [SerializeField]
    public bool edgeSnap = true;
    /// <summary>
    /// Behavior of the timeline window during playback.
    /// </summary>
    [SerializeField]
    public PlaybackScrollMode playbackScrollMode = PlaybackScrollMode.None;

    void OnDisable()
    {
        Save();
    }

    /// <summary>
    /// Save the timeline preferences settings file.
    /// </summary>
    public void Save()
    {
        Save(true);
    }

    internal SerializedObject GetSerializedObject()
    {
        return new SerializedObject(this);
    }
}

class TimelinePreferencesProvider : SettingsProvider
{
    SerializedObject m_SerializedObject;
    SerializedProperty m_ShowAudioWaveform;
    SerializedProperty m_TimeUnitInFrame;
    SerializedProperty m_SnapToFrame;
    SerializedProperty m_EdgeSnap;
    SerializedProperty m_PlaybackScrollMode;

    static string[] timeUnitsList = new string[]
    {
        "Seconds",
        "Frames"
    };

    private class Styles
    {
        public static readonly GUIContent TimeUnitLabel = EditorGUIUtility.TrTextContent("Time Unit", "Define the time unit for the timeline window (Frames or Seconds).");
        public static readonly GUIContent ShowAudioWaveformLabel = EditorGUIUtility.TrTextContent("Show Audio Waveforms", "Draw the waveforms for all audio clips.");
        public static readonly GUIContent AudioScrubbingLabel = EditorGUIUtility.TrTextContent("Allow Audio Scrubbing", "Allow the users to hear audio while scrubbing on audio clip.");
        public static readonly GUIContent SnapToFrameLabel = EditorGUIUtility.TrTextContent("Snap To Frame", "Enable Snap to Frame to manipulate clips and align them on frames.");
        public static readonly GUIContent EdgeSnapLabel = EditorGUIUtility.TrTextContent("Edge Snap", "Enable the ability to snap clips on the edge of another clip.");
        public static readonly GUIContent PlaybackScrollModeLabel = EditorGUIUtility.TrTextContent("Playback Scrolling Mode", "Define scrolling behavior during playback.");
        public static readonly GUIContent EditorSettingLabel = EditorGUIUtility.TrTextContent("Timeline Editor Settings", "");
    }

    public TimelinePreferencesProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        TimelinePreferences.instance.Save();
        m_SerializedObject = TimelinePreferences.instance.GetSerializedObject();
        m_ShowAudioWaveform = m_SerializedObject.FindProperty("showAudioWaveform");
        m_TimeUnitInFrame = m_SerializedObject.FindProperty("timeUnitInFrame");
        m_SnapToFrame = m_SerializedObject.FindProperty("snapToFrame");
        m_EdgeSnap = m_SerializedObject.FindProperty("edgeSnap");
        m_PlaybackScrollMode = m_SerializedObject.FindProperty("playbackScrollMode");
    }

    public override void OnGUI(string searchContext)
    {
        m_SerializedObject.Update();
        EditorGUI.BeginChangeCheck();
        using (new SettingsWindow.GUIScope())
        {
            EditorGUILayout.LabelField(Styles.EditorSettingLabel, EditorStyles.boldLabel);

            int timeUnitIdx = EditorGUILayout.Popup(Styles.TimeUnitLabel, m_TimeUnitInFrame.boolValue ? 1 : 0, timeUnitsList);
            m_TimeUnitInFrame.boolValue = timeUnitIdx == 1;

            m_PlaybackScrollMode.enumValueIndex = EditorGUILayout.Popup(Styles.PlaybackScrollModeLabel, m_PlaybackScrollMode.enumValueIndex, m_PlaybackScrollMode.enumNames);
            m_ShowAudioWaveform.boolValue = EditorGUILayout.Toggle(Styles.ShowAudioWaveformLabel, m_ShowAudioWaveform.boolValue);
            TimelinePreferences.instance.audioScrubbing = EditorGUILayout.Toggle(Styles.AudioScrubbingLabel, TimelinePreferences.instance.audioScrubbing);
            m_SnapToFrame.boolValue = EditorGUILayout.Toggle(Styles.SnapToFrameLabel, m_SnapToFrame.boolValue);
            m_EdgeSnap.boolValue = EditorGUILayout.Toggle(Styles.EdgeSnapLabel, m_EdgeSnap.boolValue);
        }
        if (EditorGUI.EndChangeCheck())
        {
            m_SerializedObject.ApplyModifiedProperties();
            TimelinePreferences.instance.Save();
            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateTimelineProjectSettingProvider()
    {
        var provider = new TimelinePreferencesProvider("Preferences/Timeline", SettingsScope.User, GetSearchKeywordsFromGUIContentProperties<Styles>());
        return provider;
    }
}
