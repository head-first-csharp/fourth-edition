using System;

namespace Unity.Cloud.Collaborate.Assets
{
    /// <summary>
    /// Static string resources used throughout the UI.
    /// </summary>
    internal static class StringAssets
    {
        public const string searchResults = "Search Results";
        public const string cannotPublishWhileSearching = "Please clear the search before publishing changes.";
        public const string cannotPublishWhileConflicted = "Please fix the above conflicts before publishing.";
        public const string cannotPublishWithoutFiles = "No files to publish.";
        public const string cannotPublishWithIncomingChanges = "Please sync latest changes before publishing.";
        public const string changeListFullHeader = "Publish Changes";
        public const string changeListConflictedHeader = "Conflicted Items";
        public const string changeListConflictedList = "conflicted";
        public const string noticeNoRevisionsToDisplay = "No revisions to display.";
        public const string noticeNoChangesToDisplay = "No changes to display.";
        public const string noticeNoResultsForQuery = "No results found for this query.";
        public const string publishButton = "Publish";
        public const string publishSummaryPlaceholder = "Summary";
        public const string menuDiscardToggledChanges = "Discard toggled changes";
        public const string cancel = "Cancel";
        public const string discardChanges = "Discard changes";
        public const string confirmDiscardChangeMessage = "Are you sure you want to irreversibly discard changes to a file?";
        public const string confirmDiscardChangesMessage = "Are you sure you want to irreversibly discard changes to {0} files?";
        public const string confirmDiscardChangesTitle = "Collaborate: discard changes";
        public const string search = "Search";
        public const string syncRemoteRevisionMessage = "1 revision has been published to the server. Please sync to get the latest changes.";
        public const string syncRemoteRevisionsMessage = "Some revisions have been published to the server. Please sync to get the latest changes.";
        public const string syncLocalRevisionMessage = "1 revision has been made locally, but hasn't been published to the server. Please sync to ensure these changes are published.";
        public const string syncLocalRevisionsMessage = "{0} revisions have been made locally, but haven't been published to the server. Please sync to ensure these changes are published.";
        public const string sync = "Sync";
        public const string confirmRollbackTitle = "Collaborate: rollback";
        public const string confirmRollbackMessage = "Are you sure you want to rollback your project to this revision?";
        public const string rollback = "Rollback";
        public const string confirmRollbackDiscardChangesTitle = "Collaborate: rollback and discard changes";
        public const string confirmRollbackDiscardChangeMessage = "Rollback will irreversibly discard changes to 1 file. Are you sure you want to rollback your project to this revision?";
        public const string confirmRollbackDiscardChangesMessage = "Rollback will irreversibly discard changes to {0} files. Are you sure you want to rollback your project to this revision?";
        public const string rollbackAndDiscard = "Discard and rollback";
        public const string all = "All";
        public const string includedToPublishByAnotherGitTool = "This file has been included to publish by another Git tool.";
        public const string viewDiff = "View diff";
        public const string useMyChanges = "Use my changes";
        public const string useRemoteChanges = "Use remote changes";
        public const string useMergeTool = "Use merge tool";
        public const string noMergeToolIsConfigured = "You have not set any Diff/Merge tools. Check your Unity preferences.";
        public const string showChange = "1 change";
        public const string showChanges = "{0} changes";
        public const string history = "History";
        public const string changes = "Changes";
        public const string loadingRevisions = "Please wait, loading revisions.";
        public const string allHistory = "All History";
        public const string restore = "Restore";
        public const string goBackTo = "Go back to";
        public const string update = "Update";
        public const string clear = "Clear";
        public const string changeGroupHeaderFormat = "{0} ( {1} )";
        public const string conflictsDetected = "Conflicts detected. Please resolve them before continuing.";
        public const string projectStatusTitleUnbound = "Welcome to Collaborate. Before starting, please click the button below to set a new or existing Unity Project ID for this project. Return to this window once it is set.";
        public const string projectStatusTitleOffline = "No internet connection.";
        public const string projectStatusTitleMaintenance = "Undergoing maintenance, please come back later.";
        public const string projectStatusTitleLoggedOut = "Sign in to access Collaborate.";
        public const string projectStatusTitleNoSeat = "Ask your project owner for access to Unity Teams.";
        public const string projectStatusTitleBound = "Welcome to Collaborate. Please click the button below to start.";
        public const string projectStatusTitleLoading = "Loading, please wait...";
        public const string projectStatusButtonBound = "Start Collab";
        public const string projectStatusButtonUnbound = "Set Project ID";
        public const string projectStatusButtonLoggedOut = "Sign in...";
        public const string projectStatusButtonNoSeat = "Learn More";
    }
}
