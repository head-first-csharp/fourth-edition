using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Views
{
    internal class MainPageView : PageComponent, IMainView
    {
        IMainPresenter m_Presenter;

        public const string UssClassName = "main-page-view";
        public const string TopBarUssClassName = UssClassName + "__top-bar";
        public const string AlertBoxUssClassName = UssClassName + "__alert-box";
        public const string TabViewUssClassName = UssClassName + "__tab-view";
        public const string ContainerUssClassName = UssClassName + "__container";

        // WARNING - These are hard-coded values. If you do anything to change the order
        // these tabs are initialized, you'll need to change these
        public const int ChangesTabIndex = 0;
        public const int HistoryTabIndex = 1;

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(MainPageView)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(MainPageView)}.uss";

        readonly AlertBox m_AlertBox;
        readonly TabView m_TabView;
        readonly HistoryTabPageView m_HistoryView;
        readonly ChangesTabPageView m_ChangesView;
        readonly VisualElement m_Container;
        readonly TopBar m_TopBar;
        ProgressView m_ProgressView;

        DisplayMode m_DisplayMode;

        public MainPageView()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_TopBar = this.Q<TopBar>(className: TopBarUssClassName);
            m_AlertBox = this.Q<AlertBox>(className: AlertBoxUssClassName);
            m_TabView = this.Q<TabView>(className: TabViewUssClassName);
            m_Container = this.Q<VisualElement>(className: ContainerUssClassName);

            m_ChangesView = new ChangesTabPageView();
            m_HistoryView = new HistoryTabPageView();
            m_TabView.AddTab(StringAssets.changes, m_ChangesView);
            m_TabView.AddTab(StringAssets.history, m_HistoryView);

            // Set the current display mode.
            m_DisplayMode = DisplayMode.TabView;
        }

        /// <inheritdoc />
        public IMainPresenter Presenter
        {
            set
            {
                m_Presenter = value;
                m_Presenter.AssignHistoryPresenter(m_HistoryView);
                m_Presenter.AssignChangesPresenter(m_ChangesView);
                m_TabView.TabSwitched += OnTabSwitched;
                m_TopBar.BackButtonClicked += OnBackButtonClicked;
                // If page active before presenter has been added, call start once we have it.
                if (Active)
                {
                    m_Presenter.Start();
                }
            }
        }


        /// <inheritdoc />
        protected override void SetActive()
        {
            // Set TabView active if it's currently being displayed.
            if (m_DisplayMode == DisplayMode.TabView)
            {
                m_TabView.SetActive();
            }
            m_Presenter?.Start();
        }

        /// <inheritdoc />
        protected override void SetInactive()
        {
            // Set TabView inactive if it's current being displayed.
            if (m_DisplayMode == DisplayMode.TabView)
            {
                m_TabView.SetInactive();
            }
            m_Presenter?.Stop();
        }

        /// <inheritdoc />
        public void AddAlert(string id, AlertBox.AlertLevel level, string message, (string text, Action action)? button = null)
        {
            m_AlertBox.QueueAlert(id, level, message, button);
        }

        /// <inheritdoc />
        public void RemoveAlert(string id)
        {
            m_AlertBox.DequeueAlert(id);
        }

        /// <inheritdoc />
        public void SetTab(int index)
        {
            m_TabView.SwitchTab(index);
        }

        /// <inheritdoc />
        public void AddOperationProgress()
        {
            SetDisplay(DisplayMode.ProgressView);
        }

        /// <inheritdoc />
        public void RemoveOperationProgress()
        {
            SetDisplay(DisplayMode.TabView);
        }

        /// <inheritdoc />
        public void SetOperationProgress(string title, string details, int percentage, int completed, int total, bool isPercentage, bool canCancel)
        {
            Assert.IsNotNull(m_ProgressView);
            if (m_ProgressView == null) return;
            var progress = isPercentage ? $"{percentage}%" : $"({completed} of {total})";
            m_ProgressView.SetText($"{title}\n\n{details}", progress);
            m_ProgressView.SetPercentComplete(percentage);
            m_ProgressView.SetCancelButtonActive(canCancel);
        }

        /// <inheritdoc />
        public void ClearBackNavigation()
        {
            m_TopBar.HideBackNavigation();
        }

        /// <inheritdoc />
        public void DisplayBackNavigation(string text)
        {
            m_TopBar.DisplayBackNavigation(text);
        }

        void SetDisplay(DisplayMode mode)
        {
            Assert.AreNotEqual(m_DisplayMode, mode, "Cannot switch to the current display mode.");
            m_DisplayMode = mode;

            // Switch into tab or progress view.
            if (m_DisplayMode == DisplayMode.TabView)
            {
                m_ProgressView?.AddToClassList(UiConstants.ussHidden);

                m_TabView.RemoveFromClassList(UiConstants.ussHidden);
                m_TabView.SetActive();
            }
            else
            {
                if (m_ProgressView == null)
                {
                    m_ProgressView = new ProgressView();
                    m_ProgressView.SetCancelCallback(m_Presenter.RequestCancelJob);
                    m_Container.Add(m_ProgressView);
                }
                m_ProgressView.RemoveFromClassList(UiConstants.ussHidden);
                m_TabView.AddToClassList(UiConstants.ussHidden);
                m_TabView.SetInactive();
            }
        }

        void OnTabSwitched(int index)
        {
            m_Presenter.UpdateTabIndex(index);
        }

        void OnBackButtonClicked()
        {
            m_Presenter.NavigateBack();
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<MainPageView> { }

        enum DisplayMode
        {
            TabView,
            ProgressView
        }
    }
}
