using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.ChangeListEntries
{
    internal class ConflictedChangeListElement : BaseChangeListElement
    {
        public new const string UssClassName = "conflicted-change-list-element";
        public const string StatusIconUssClassName = UssClassName + "__icon";
        public const string ShowButtonUssClassName = UssClassName + "__show-button";
        public const string ChooseMergeButtonUssClassName = UssClassName + "__choose-merge-button";
        public const string ChooseRemoteButtonUssClassName = UssClassName + "__choose-remote-button";
        public const string ChooseMineButtonUssClassName = UssClassName + "__choose-mine-button";

        public readonly VisualElement statusIcon;
        public readonly IconButton showButton;
        public readonly IconButton chooseMergeButton;
        public readonly IconButton chooseRemoteButton;
        public readonly IconButton chooseMineButton;

        public ConflictedChangeListElement()
        {
            AddToClassList(UssClassName);

            // Setup icons
            statusIcon = new VisualElement();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);
            icons.Add(statusIcon);

            // Setup buttons
            showButton = new IconButton();
            showButton.AddToClassList(IconButton.ShowUssClassName);
            showButton.AddToClassList(ButtonUssClassName);
            showButton.AddToClassList(ShowButtonUssClassName);
            showButton.tooltip = StringAssets.viewDiff;

            chooseMergeButton = new IconButton();
            chooseMergeButton.AddToClassList(IconButton.MergeUssClassName);
            chooseMergeButton.AddToClassList(ButtonUssClassName);
            chooseMergeButton.AddToClassList(ChooseMergeButtonUssClassName);
            chooseMergeButton.tooltip = StringAssets.useMergeTool;

            chooseMineButton = new IconButton();
            chooseMineButton.AddToClassList(IconButton.ChooseMineUssClassName);
            chooseMineButton.AddToClassList(ButtonUssClassName);
            chooseMineButton.AddToClassList(ChooseMineButtonUssClassName);
            chooseMineButton.tooltip = StringAssets.useMyChanges;

            chooseRemoteButton = new IconButton();
            chooseRemoteButton.AddToClassList(IconButton.ChooseRemoteUssClassName);
            chooseRemoteButton.AddToClassList(ButtonUssClassName);
            chooseRemoteButton.AddToClassList(ChooseRemoteButtonUssClassName);
            chooseRemoteButton.tooltip = StringAssets.useRemoteChanges;

            buttons.Add(showButton);
            buttons.Add(chooseMergeButton);
            buttons.Add(chooseMineButton);
            buttons.Add(chooseRemoteButton);
        }

        public override void ClearData()
        {
            base.ClearData();

            showButton.UnregisterClickEvents();
            chooseMergeButton.UnregisterClickEvents();
            chooseRemoteButton.UnregisterClickEvents();
            chooseMineButton.UnregisterClickEvents();

            statusIcon.ClearClassList();
            statusIcon.AddToClassList(IconUssClassName);
            statusIcon.AddToClassList(StatusIconUssClassName);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ConflictedChangeListElement> { }
    }
}
