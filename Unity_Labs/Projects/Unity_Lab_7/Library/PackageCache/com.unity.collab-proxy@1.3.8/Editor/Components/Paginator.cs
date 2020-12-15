using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class Paginator : VisualElement
    {
        public const string UssClassName = "paginator";
        public const string PageTextUssClassName = UssClassName + "__page-text";
        public const string BackButtonUssClassName = UssClassName + "__back-button";
        public const string ForwardsButtonUssClassName = UssClassName + "__forwards-button";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(Paginator)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(Paginator)}.uss";

        public const int MoveBackwards = -1;
        public const int MoveForwards = 1;

        public event Action<int> ClickedMovePage;

        readonly Label m_PageText;
        readonly Button m_BackButton;
        readonly Button m_ForwardsButton;

        public Paginator()
        {
            // Get the layout
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_PageText = this.Q<Label>(className: PageTextUssClassName);
            m_BackButton = this.Q<Button>(className: BackButtonUssClassName);
            m_ForwardsButton = this.Q<Button>(className: ForwardsButtonUssClassName);

            m_BackButton.text = "<";
            m_ForwardsButton.text = ">";

            m_BackButton.clickable.clicked += () => ClickedMovePage?.Invoke(MoveBackwards);
            m_ForwardsButton.clickable.clicked += () => ClickedMovePage?.Invoke(MoveForwards);
        }

        public void SetPage(int page, int maxPage)
        {
            m_PageText.text = $"Page {page + 1} of {maxPage + 1}";
            m_BackButton.SetEnabled(page != 0);
            m_ForwardsButton.SetEnabled(page != maxPage);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<Paginator> { }
    }
}
