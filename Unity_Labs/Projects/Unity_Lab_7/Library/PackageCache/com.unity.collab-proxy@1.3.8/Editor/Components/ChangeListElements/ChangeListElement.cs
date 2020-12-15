using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.ChangeListEntries
{
    internal class ChangeListElement : BaseChangeListElement
    {
        public new const string UssClassName = "change-list-element";
        public const string StatusIconUssClassName = UssClassName + "__icon";
        public const string DiffButtonUssClassName = UssClassName + "__diff-button";
        public const string DiscardButtonUssClassName = UssClassName + "__revert-button";

        public readonly IconButton discardButton;
        public readonly IconButton diffButton;
        public readonly VisualElement statusIcon;

        public ChangeListElement()
        {
            AddToClassList(UssClassName);

            // Setup icons
            statusIcon = new VisualElement();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);
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

        public override void ClearData()
        {
            base.ClearData();

            diffButton.UnregisterClickEvents();
            discardButton.UnregisterClickEvents();

            statusIcon.ClearClassList();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ChangeListElement> { }
    }
}
