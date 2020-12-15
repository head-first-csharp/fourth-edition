using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.ChangeListEntries
{
    internal class HistoryChangeListElement : BaseChangeListElement
    {
        public new const string UssClassName = "history-change-list-element";
        public const string StatusIconUssClassName = UssClassName + "__icon";
        public const string RevertButtonUssClassName = UssClassName + "__revert-button";

        public readonly VisualElement statusIcon;
        public readonly IconButton revertButton;

        public HistoryChangeListElement()
        {
            AddToClassList(UssClassName);

            // Setup icons
            statusIcon = new VisualElement();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);
            icons.Add(statusIcon);

            // Setup buttons
            revertButton = new IconButton();
            revertButton.AddToClassList(IconButton.UndoUssClassName);
            revertButton.AddToClassList(ButtonUssClassName);
            revertButton.AddToClassList(RevertButtonUssClassName);
            buttons.Add(revertButton);
        }

        public override void ClearData()
        {
            base.ClearData();

            revertButton.UnregisterClickEvents();

            statusIcon.ClearClassList();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<HistoryChangeListElement> { }
    }
}
