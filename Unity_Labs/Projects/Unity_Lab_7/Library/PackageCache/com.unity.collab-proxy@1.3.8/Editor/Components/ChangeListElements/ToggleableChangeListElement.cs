using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.ChangeListEntries
{
    internal class ToggleableChangeListElement : BaseChangeListElement
    {
        public new const string UssClassName = "toggleable-change-list-element";
        public const string ToggleUssClassName = UssClassName + "__toggle";
        public const string StatusIconUssClassName = UssClassName + "__icon";
        public const string DiffButtonUssClassName = UssClassName + "__diff-button";
        public const string DiscardButtonUssClassName = UssClassName + "__revert-button";

        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(ToggleableChangeListElement)}.uss";

        public readonly Toggle toggle;
        public readonly IconButton diffButton;
        public readonly IconButton discardButton;
        public readonly VisualElement statusIcon;

        [CanBeNull]
        EventCallback<ChangeEvent<bool>> m_ToggleCallback;

        public ToggleableChangeListElement()
        {
            AddToClassList(UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Setup icons
            var toggleContainer = new VisualElement();
            toggle = new Toggle();
            toggle.AddToClassList(ToggleUssClassName);
            toggleContainer.Add(toggle);

            statusIcon = new VisualElement();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);

            icons.Add(toggleContainer);
            icons.Add(statusIcon);

            // Setup buttons
            diffButton = new IconButton();
            diffButton.AddToClassList(IconButton.DiffUssCLassName);
            diffButton.AddToClassList(ButtonUssClassName);
            diffButton.AddToClassList(DiffButtonUssClassName);

            discardButton = new IconButton();
            discardButton.AddToClassList(IconButton.UndoUssClassName);
            discardButton.AddToClassList(ButtonUssClassName);
            discardButton.AddToClassList(DiscardButtonUssClassName);

            buttons.Add(diffButton);
            buttons.Add(discardButton);
        }

        public void SetToggleCallback(Action<bool> callback)
        {
            Assert.IsNull(m_ToggleCallback);
            m_ToggleCallback = c => callback(c.newValue);
            toggle.RegisterValueChangedCallback(m_ToggleCallback);
        }

        public override void ClearData()
        {
            base.ClearData();

            diffButton.UnregisterClickEvents();
            discardButton.UnregisterClickEvents();

            statusIcon.ClearClassList();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);

            if (m_ToggleCallback != null)
            {
                toggle.UnregisterValueChangedCallback(m_ToggleCallback);
                m_ToggleCallback = null;
            }
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ToggleableChangeListElement> { }
    }
}
