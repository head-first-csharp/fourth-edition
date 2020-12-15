using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEngine;

namespace Unity.Cloud.Collaborate.UserInterface
{
    internal class ToolbarButton : SubToolbar
    {
        protected enum ToolbarButtonState
        {
            NeedToEnableCollab,
            UpToDate,
            Conflict,
            OperationError,
            ServerHasChanges,
            FilesToPush,
            InProgress,
            Disabled,
            Offline
        }

        ToolbarButtonState m_CurrentState;
        string m_ErrorMessage;
        readonly Dictionary<ToolbarButtonState, GUIContent> m_IconCache = new Dictionary<ToolbarButtonState, GUIContent>();
        ButtonWithAnimatedIconRotation m_CollabButton;

        public ToolbarButton()
        {
            Collab.instance.StateChanged += OnCollabStateChanged;
            UnityConnect.instance.StateChanged += OnUnityConnectStateChanged;
            UnityConnect.instance.UserStateChanged += OnUnityConnectUserStateChanged;
        }

        ~ToolbarButton()
        {
            Collab.instance.StateChanged -= OnCollabStateChanged;
            UnityConnect.instance.StateChanged -= OnUnityConnectStateChanged;
            UnityConnect.instance.UserStateChanged -= OnUnityConnectUserStateChanged;
        }

        void OnUnityConnectUserStateChanged(UserInfo state)
        {
            Update();
        }

        void OnUnityConnectStateChanged(ConnectInfo state)
        {
            Update();
        }

        void OnCollabStateChanged(CollabInfo info)
        {
            Update();
        }

        [CanBeNull]
        static Texture LoadIcon([NotNull] string iconName)
        {
            var hidpi = EditorGUIUtility.pixelsPerPoint > 1f ? "@2x" : string.Empty;
            return AssetDatabase.LoadAssetAtPath<Texture>($"{CollaborateWindow.IconPath}/{iconName}-{(EditorGUIUtility.isProSkin ? "dark" : "light")}{hidpi}.png");
        }

        [NotNull]
        GUIContent GetIconForState()
        {
            // Get cached icon, or construct it.
            if (!m_IconCache.TryGetValue(m_CurrentState, out var content))
            {
                string iconName;
                string tooltip;
                switch (m_CurrentState)
                {
                    case ToolbarButtonState.NeedToEnableCollab:
                        iconName = "collaborate";
                        tooltip = "You need to enable collab.";
                        break;
                    case ToolbarButtonState.UpToDate:
                        iconName = "collaborate";
                        tooltip = "You are up to date.";
                        break;
                    case ToolbarButtonState.Conflict:
                        iconName = "collaborate-error";
                        tooltip = "Please fix your conflicts prior to publishing.";
                        break;
                    case ToolbarButtonState.OperationError:
                        iconName = "collaborate-error";
                        tooltip = "Last operation failed. Please retry later.";
                        break;
                    case ToolbarButtonState.ServerHasChanges:
                        iconName = "collaborate-incoming";
                        tooltip = "Please update, there are server changes.";
                        break;
                    case ToolbarButtonState.FilesToPush:
                        iconName = "collaborate-available-changes";
                        tooltip = "You have files to publish.";
                        break;
                    case ToolbarButtonState.InProgress:
                        iconName = "collaborate-progress";
                        tooltip = "Operation in progress.";
                        break;
                    case ToolbarButtonState.Disabled:
                        iconName = "collaborate";
                        tooltip = "Collab is disabled.";
                        break;
                    case ToolbarButtonState.Offline:
                        iconName = "collaborate-offline";
                        tooltip = "Please check your network connection.";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Create icon with tooltip and cache.
                content = new GUIContent(LoadIcon(iconName), $"Collaborate • {tooltip}");
                m_IconCache[m_CurrentState] = content;
            }

            // Add error message tooltip if there's a message.
            var icon = new GUIContent(content);
            if (!string.IsNullOrEmpty(m_ErrorMessage))
            {
                icon.tooltip = $"Collaborate • {m_ErrorMessage}";
            }

            return icon;
        }

        public override void OnGUI(Rect rect)
        {
            GUIStyle collabButtonStyle = "AppCommand";
            var disable = EditorApplication.isPlaying;
            using (new EditorGUI.DisabledScope(disable))
            {
                var icon = GetIconForState();
                EditorGUIUtility.SetIconSize(new Vector2(16, 16));
                if (GUI.Button(rect, icon, collabButtonStyle))
                {
                    CollaborateWindow.Init();
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
        }

        public void Update()
        {
            var currentState = GetCurrentState();

            if (m_CurrentState == currentState) return;
            m_CurrentState = currentState;
            Toolbar.RepaintToolbar();
        }

        protected virtual ToolbarButtonState GetCurrentState()
        {
            var currentState = ToolbarButtonState.UpToDate;
            var networkAvailable = UnityConnect.instance.connectInfo.online && UnityConnect.instance.connectInfo.loggedIn;
            m_ErrorMessage = string.Empty;

            if (UnityConnect.instance.isDisableCollabWindow)
            {
                currentState = ToolbarButtonState.Disabled;
            }
            else if (networkAvailable)
            {
                var collab = Collab.instance;
                var currentInfo = collab.collabInfo;

                if (!currentInfo.ready)
                {
                    currentState = ToolbarButtonState.InProgress;
                }
                else if (collab.GetError(UnityConnect.UnityErrorFilter.ByContext | UnityConnect.UnityErrorFilter.ByChild, out var errInfo) && 
                    errInfo.priority <= (int)UnityConnect.UnityErrorPriority.Error)
                {
                    currentState = ToolbarButtonState.OperationError;
                    m_ErrorMessage = errInfo.shortMsg;
                }
                else if (currentInfo.inProgress)
                {
                    currentState = ToolbarButtonState.InProgress;
                }
                else
                {
                    var collabEnabled = Collab.instance.IsCollabEnabledForCurrentProject();

                    if (UnityConnect.instance.projectInfo.projectBound == false || !collabEnabled)
                    {
                        currentState = ToolbarButtonState.NeedToEnableCollab;
                    }
                    else if (currentInfo.update)
                    {
                        currentState = ToolbarButtonState.ServerHasChanges;
                    }
                    else if (currentInfo.conflict)
                    {
                        currentState = ToolbarButtonState.Conflict;
                    }
                    else if (currentInfo.publish)
                    {
                        currentState = ToolbarButtonState.FilesToPush;
                    }
                }
            }
            else
            {
                currentState = ToolbarButtonState.Offline;
            }

            return currentState;
        }
    }
}
