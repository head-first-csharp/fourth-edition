using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Views
{
    internal class StartPageView : PageComponent, IStartView
    {
        public const string UssClassName = "start-page-view";
        public const string UssTitleClassName = UssClassName + "__title";
        public const string UssButtonClassName = UssClassName + "__button";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(StartPageView)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(StartPageView)}.uss";

        IStartPresenter m_Presenter;

        readonly Label m_Text;
        readonly Button m_Button;

        public StartPageView()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_Text = this.Q<Label>(className: UssTitleClassName);
            m_Button = this.Q<Button>(className: UssButtonClassName);
        }

        /// <inheritdoc />
        public IStartPresenter Presenter
        {
            set
            {
                m_Presenter = value;
                SetupEvents();
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
            m_Presenter?.Start();
        }

        /// <inheritdoc />
        protected override void SetInactive()
        {
            m_Presenter?.Stop();
        }

        void SetupEvents()
        {
            m_Button.clickable.clicked += m_Presenter.RequestStart;
        }

        /// <inheritdoc />
        public string Text
        {
            set => m_Text.text = value;
        }

        /// <inheritdoc />
        public string ButtonText
        {
            set => m_Button.text = value;
        }

        /// <inheritdoc />
        public void SetButtonVisible(bool isVisible)
        {
            if (isVisible)
            {
                m_Button.RemoveFromClassList(UiConstants.ussHidden);
            }
            else
            {
                m_Button.AddToClassList(UiConstants.ussHidden);
            }
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<StartPageView> { }
    }
}
