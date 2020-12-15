using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class ListNotice : VisualElement
    {
        public const string UssClassName = "list-notice";
        public const string ListNoticeTextUssClassName = UssClassName + "__text";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(ListNotice)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(ListNotice)}.uss";

        readonly Label m_Text;

        public ListNotice()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_Text = this.Q<Label>(className: ListNoticeTextUssClassName);
        }

        public string Text
        {
            set => m_Text.text = value;
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ListNotice> { }
    }
}
