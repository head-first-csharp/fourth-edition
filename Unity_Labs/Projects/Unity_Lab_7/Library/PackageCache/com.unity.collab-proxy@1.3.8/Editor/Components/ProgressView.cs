using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    [UsedImplicitly]
    internal class ProgressView : VisualElement
    {
        public const string UssClassName = "progress-view";
        public const string LabelUssClassName = UssClassName + "__label";
        public const string ProgressBarUssClassName = UssClassName + "__progress-bar";
        public const string ButtonUssClassName = UssClassName + "__button";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(ProgressView)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(ProgressView)}.uss";

        readonly Label m_Label;
        readonly ProgressBar m_ProgressBar;
        readonly Button m_Button;

        public ProgressView()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_Label = this.Q<Label>(className: LabelUssClassName);
            m_Label.text = string.Empty;

            m_ProgressBar = this.Q<ProgressBar>(className: ProgressBarUssClassName);

            m_Button = this.Q<Button>(className: ButtonUssClassName);
            m_Button.text = StringAssets.cancel;
        }

        public void SetText(string text, string progressText)
        {
            m_Label.text = text;
            m_ProgressBar.title = progressText;
        }

        public void SetPercentComplete(int percent)
        {
            m_ProgressBar.value = percent;
        }

        public void SetCancelCallback(Action callback)
        {
            m_Button.clickable.clicked += callback;
        }

        public void SetCancelButtonActive(bool active)
        {
            m_Button.SetEnabled(active);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ProgressView> { }
    }


}
