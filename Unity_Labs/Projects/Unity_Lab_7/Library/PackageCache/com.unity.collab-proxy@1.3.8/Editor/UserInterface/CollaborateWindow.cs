using System;
using System.Collections.Generic;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Components.Menus;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Models.Providers;
using Unity.Cloud.Collaborate.Views;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.UserInterface
{
    internal class CollaborateWindow : EditorWindow
    {
        public const string UssClassName = "main-window";
        public const string ContainerUssClassName = UssClassName + "__container";

        public const string PackagePath = "Packages/com.unity.collab-proxy";
        public const string UserInterfacePath = PackagePath + "/Editor/UserInterface";
        public const string ResourcePath = PackagePath + "/Editor/Assets";
        public const string LayoutPath = ResourcePath + "/Layouts";
        public const string StylePath = ResourcePath + "/Styles";
        public const string IconPath = ResourcePath + "/Icons";
        public const string TestWindowPath = UserInterfacePath + "/TestWindows";

        const string k_LayoutPath = LayoutPath + "/main-window.uxml";
        public const string MainStylePath = StylePath + "/styles.uss";

        MainPageView m_MainView;
        ErrorPageView m_ErrorPageView;
        StartPageView m_StartView;
        VisualElement m_ViewContainer;

        PageComponent m_ActivePage;

        ISourceControlProvider m_Provider;

        List<IModel> m_Models;

        [MenuItem("Window/Collaborate")]
        internal static void Init()
        {
            Init(FocusTarget.None);
        }

        internal static void Init(FocusTarget focusTarget)
        {
            var openLocation = CollabSettingsManager.Get(CollabSettings.settingDefaultOpenLocation, fallback: CollabSettings.OpenLocation.Docked);

            CollaborateWindow window;
            if (openLocation == CollabSettings.OpenLocation.Docked)
            {
                // Dock next to inspector, if available
                var inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
                window = GetWindow<CollaborateWindow>(inspectorType);
            }
            else
            {
                window = GetWindow<CollaborateWindow>();
            }

            // Set up window
            window.titleContent = new GUIContent("Collaborate");
            window.minSize = new Vector2(256, 400);

            // Display window
            window.Show();
            window.Focus();
            if (focusTarget != FocusTarget.None)
            {
                window.RequestFocus(focusTarget);
            }
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            m_Provider.UpdatedProjectStatus -= OnUpdatedProjectStatus;
            m_Models.ForEach(m => m.OnStop());
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(MainStylePath));

            root.AddToClassList(EditorGUIUtility.isProSkin
                ? UiConstants.ussDark
                : UiConstants.ussLight);

            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(root);

            m_Provider = new Collab();
            m_Provider.UpdatedProjectStatus += OnUpdatedProjectStatus;

            m_ViewContainer = root.Q<VisualElement>(className: ContainerUssClassName);

            // Create models and configure them.
            var mainModel = new MainModel(m_Provider);
            var startModel = new StartModel(m_Provider);

            m_Models = new List<IModel> { mainModel, startModel };
            m_Models.ForEach(m => m.OnStart());

            // Get the views and configure them.
            m_MainView = new MainPageView();
            m_MainView.Presenter = new MainPresenter(m_MainView, mainModel);
            m_StartView = new StartPageView();
            m_StartView.Presenter = new StartPresenter(m_StartView, startModel);
            m_ErrorPageView = new ErrorPageView();

            // Add floating dialogue so it can be displayed anywhere in the window.
            root.Add(FloatingDialogue.Instance);

            OnUpdatedProjectStatus(m_Provider.GetProjectStatus());
        }

        /// <summary>
        /// React to the play mode state changing. When in play mode, disable collab.
        /// </summary>
        /// <param name="state">Editor play mode state.</param>
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            bool enabled;
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingEditMode:
                    enabled = true;
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.ExitingPlayMode:
                    enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            m_ViewContainer.SetEnabled(enabled);
        }

        /// <summary>
        /// Restore window state after assembly reload.
        /// </summary>
        void OnAfterAssemblyReload()
        {
            m_Models.ForEach(m => m.RestoreState(WindowCache.Instance));
        }

        /// <summary>
        /// Save state before domain reload.
        /// </summary>
        void OnBeforeAssemblyReload()
        {
            m_Models.ForEach(m => m.SaveState(WindowCache.Instance));
            WindowCache.Instance.Serialize();
        }

        /// <summary>
        /// Respond to changes in the project status.
        /// </summary>
        /// <param name="status">New project status.</param>
        void OnUpdatedProjectStatus(ProjectStatus status)
        {
            if (status == ProjectStatus.Ready)
            {
                UpdateDisplayMode(Display.Main);
            }
            else
            {
                WindowCache.Instance.Clear();
                m_Models.ForEach(m => m.RestoreState(WindowCache.Instance));
                UpdateDisplayMode(Display.Add);
            }
        }

        void RequestFocus(FocusTarget focusTarget)
        {
            if (m_ActivePage != m_MainView)
            {
                // Cannot focus changes or history pane if we're not already on mainview
                return;
            }

            if (focusTarget == FocusTarget.Changes)
            {
                m_MainView.SetTab(MainPageView.ChangesTabIndex);
            }
            else if (focusTarget == FocusTarget.History)
            {
                m_MainView.SetTab(MainPageView.HistoryTabIndex);
            }
            else
            {
                Debug.LogError("Collab Error: Attempting to focus unknown target.");
            }
        }

        /// <summary>
        /// Switch the view displayed in the window.
        /// </summary>
        /// <param name="newDisplay">Display to switch the window to.</param>
        void UpdateDisplayMode(Display newDisplay)
        {
            m_ActivePage?.RemoveFromHierarchy();
            m_ActivePage?.SetActive(false);
            m_ViewContainer.Clear();

            // Get new page to display
            switch (newDisplay)
            {
                case Display.Add:
                    m_ActivePage = m_StartView;
                    break;
                case Display.Error:
                    m_ActivePage = m_ErrorPageView;
                    break;
                case Display.Main:
                    m_ActivePage = m_MainView;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_ActivePage.SetActive(true);
            m_ViewContainer.Add(m_ActivePage);
        }

        enum Display
        {
            Add,
            Error,
            Main
        }

        public enum FocusTarget
        {
            None,
            History,
            Changes
        }
    }
}
