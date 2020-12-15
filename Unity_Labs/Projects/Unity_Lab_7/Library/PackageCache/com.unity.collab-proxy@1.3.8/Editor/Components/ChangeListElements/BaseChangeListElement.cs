using System.IO;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.ChangeListEntries
{
    [UsedImplicitly]
    internal class BaseChangeListElement : VisualElement
    {
        public const string UssClassName = "base-change-list-element";
        public const string FileNameUssClassName = UssClassName + "__file-name";
        public const string FilePathUssClassName = UssClassName + "__file-path";
        public const string IconsUssClassName = UssClassName + "__icons";
        public const string ButtonsUssClassName = UssClassName + "__buttons";

        // Styling class names
        public const string IconUssClassName = UssClassName + "__icon";
        public const string ButtonUssClassName = UssClassName + "__button";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(BaseChangeListElement)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(BaseChangeListElement)}.uss";

        protected readonly Label m_FileName;
        protected readonly Label m_FilePath;
        public readonly VisualElement icons;
        public readonly VisualElement buttons;

        public BaseChangeListElement()
        {
            // Get the layout
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Initialise fields
            m_FileName = this.Q<Label>(className: FileNameUssClassName);
            m_FilePath = this.Q<Label>(className: FilePathUssClassName);
            icons = this.Q<VisualElement>(className: IconsUssClassName);
            buttons = this.Q<VisualElement>(className: ButtonsUssClassName);
        }

        public void UpdateFilePath([NotNull] string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            m_FileName.text = Path.GetFileName(path);
            m_FilePath.text = directoryName;
            m_FilePath.tooltip = directoryName;
        }

        public virtual void ClearData()
        {
            m_FileName.text = null;
            m_FileName.tooltip = null;
            m_FilePath.text = null;
            m_FilePath.tooltip = null;
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<BaseChangeListElement> { }
    }
}
