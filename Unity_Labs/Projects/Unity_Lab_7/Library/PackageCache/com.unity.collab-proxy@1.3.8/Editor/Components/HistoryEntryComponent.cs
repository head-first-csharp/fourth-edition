using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Components
{
    internal class HistoryEntryComponent : VisualElement
    {
        public const string UssClassName = "history-entry-component";
        public const string ProfileInitialUssClassName = UssClassName + "__profile-initial";
        public const string AuthorNameUssClassName = UssClassName + "__author-name";
        public const string TimestampUssClassName = UssClassName + "__timestamp";
        public const string RevisionIdUssClassName = UssClassName + "__revision-id";
        public const string CommitMessageUssClassName = UssClassName + "__commit-message";
        public const string ChangedFilesCountUssClassName = UssClassName + "__changed-files-count";
        public const string ChangedFilesUssClassName = UssClassName + "__changed-files";
        public const string RollbackButtonUssClassName = UssClassName + "__goto-button";
        public const string ShowFilesButtonUssClassName = UssClassName + "__files-button";
        public const string BuildStatusUssClassName = UssClassName + "__cloud-build-status";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(HistoryEntryComponent)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(HistoryEntryComponent)}.uss";

        public readonly Label profileInitial;
        public readonly Label authorName;
        public readonly Label timestamp;
        public readonly Label revisionId;
        public readonly Label commitMessage;
        public readonly Button gotoButton;
        public readonly Button showFilesButton;
        public readonly Label cloudStatusText;
        public readonly Label changedFilesCount;
        public readonly AdapterListView changedFiles;

        public HistoryEntryComponent()
        {
            // Get the layout
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Initialise fields
            profileInitial = this.Q<Label>(className: ProfileInitialUssClassName);
            authorName = this.Q<Label>(className: AuthorNameUssClassName);
            timestamp = this.Q<Label>(className: TimestampUssClassName);
            revisionId = this.Q<Label>(className: RevisionIdUssClassName);
            commitMessage = this.Q<Label>(className: CommitMessageUssClassName);
            changedFilesCount = this.Q<Label>(className: ChangedFilesCountUssClassName);
            changedFiles = this.Q<AdapterListView>(className: ChangedFilesUssClassName);
            gotoButton = this.Q<Button>(className: RollbackButtonUssClassName);
            showFilesButton = this.Q<Button>(className: ShowFilesButtonUssClassName);
            cloudStatusText = this.Q<Label>(className: BuildStatusUssClassName);

            changedFiles.SelectionType = SelectionType.None;

            gotoButton.text = StringAssets.rollback;
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<HistoryEntryComponent> { }
    }
}
