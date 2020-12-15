using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Models.Api
{
    internal interface ISourceControlProvider
    {
        /// <summary>
        /// Event called whenever the change list is updated.
        /// </summary>
        event Action UpdatedChangeList;

        /// <summary>
        /// Event called whenever the selected change list is updated.
        /// </summary>
        event Action<IReadOnlyList<string>> UpdatedSelectedChangeList;

        /// <summary>
        /// Event called whenever the list of historical revisions is updated.
        /// </summary>
        event Action UpdatedHistoryEntries;

        /// <summary>
        /// Event called whenever a long operation (with progress) has completed / started.
        /// </summary>
        event Action<bool> UpdatedOperationStatus;

        /// <summary>
        /// Event called whenever a long operation progress has changed.
        /// </summary>
        event Action<IProgressInfo> UpdatedOperationProgress;

        /// <summary>
        /// Event called whenever an error has occurred.
        /// </summary>
        event Action<IErrorInfo> ErrorOccurred;

        /// <summary>
        /// Event called whenever an error has cleared.
        /// </summary>
        event Action ErrorCleared;

        /// <summary>
        /// Event called whenever the conflict state changes.
        /// </summary>
        event Action<bool> UpdatedConflictState;

        /// <summary>
        /// Event called whenever the availability of remote revisions changes.
        /// </summary>
        event Action<bool> UpdatedRemoteRevisionsAvailability;

        /// <summary>
        /// Event that is triggered when the project status changes.
        /// </summary>
        event Action<ProjectStatus> UpdatedProjectStatus;

        /// <summary>
        /// Get whether there are available revisions to fetch.
        /// </summary>
        /// <returns>True if there are revisions available</returns>
        bool GetRemoteRevisionAvailability();

        /// <summary>
        /// Get whether there is a conflict or not.
        /// </summary>
        /// <returns>True if there is a conflict.</returns>
        bool GetConflictedState();

        /// <summary>
        /// Get the current progress state.
        /// </summary>
        /// <returns>The current progress state. Null if no operation ongoing.</returns>
        [CanBeNull]
        IProgressInfo GetProgressState();

        /// <summary>
        /// Get the current error state.
        /// </summary>
        /// <returns>The current error state. Null if no errors presently.</returns>
        [CanBeNull]
        IErrorInfo GetErrorState();

        /// <summary>
        /// Returns the current project status.
        /// </summary>
        /// <returns>Current project status.</returns>
        ProjectStatus GetProjectStatus();

        /// <summary>
        /// Request the current change list.
        /// </summary>
        /// <param name="callback">Callback for the result.</param>
        void RequestChangeList([NotNull] Action<IReadOnlyList<IChangeEntry>> callback);

        /// <summary>
        /// Publish files to Collaborate.
        /// </summary>
        /// <param name="message">Message to commit with.</param>
        /// <param name="changes">Changes to publish. Pass null to publish all.</param>
        void RequestPublish([NotNull] string message, [CanBeNull] IReadOnlyList<IChangeEntry> changes = null);

        /// <summary>
        /// Request a single historical revision.
        /// </summary>
        /// <param name="revisionId">Id of the revision to request.</param>
        /// <param name="callback">Callback for the result.</param>
        void RequestHistoryEntry([NotNull] string revisionId, [NotNull] Action<IHistoryEntry> callback);

        /// <summary>
        /// Request a page of historical revisions.
        /// </summary>
        /// <param name="offset">Where to start the page from.</param>
        /// <param name="pageSize">Number of entries in the page.</param>
        /// <param name="callback">Callback for the result.</param>
        void RequestHistoryPage(int offset, int pageSize, [NotNull] Action<IReadOnlyList<IHistoryEntry>> callback);

        /// <summary>
        /// Request the total number of historical revisions.
        /// </summary>
        /// <param name="callback">Callback for the result.</param>
        void RequestHistoryCount([NotNull] Action<int?> callback);

        /// <summary>
        /// Revert the specified file to the state of the current revision.
        /// of the source control system, or delete it if it's a new file.
        /// </summary>
        /// <param name="entry">Entry to discard.</param>
        void RequestDiscard([NotNull] IChangeEntry entry);

        /// <summary>
        /// Revert the specified files to the state of the current revision.
        /// of the source control system.
        /// </summary>
        /// <param name="paths">List of entries to discard.</param>
        void RequestBulkDiscard([NotNull] IReadOnlyList<IChangeEntry> entries);

        /// <summary>
        /// Diff the changes for the file at the given path.
        /// </summary>
        /// <param name="path">Path of the file to diff.</param>
        void RequestDiffChanges([NotNull] string path);

        /// <summary>
        /// Returns true if the provider supports revert.
        /// </summary>
        bool SupportsRevert { get; }

        /// <summary>
        /// Request revert the specified files to the given revision.
        /// </summary>
        /// <param name="revisionId">Revision to revert the files back to.</param>
        /// <param name="files">Files to revert back.</param>
        void RequestRevert([NotNull] string revisionId, [NotNull] IReadOnlyList<string> files);

        /// <summary>
        /// Request to update the state of the project to a new provided revision.
        /// </summary>
        /// <param name="revisionId">New revision id of the project to go to.</param>
        void RequestUpdateTo([NotNull] string revisionId);

        /// <summary>
        /// Request to take the state of the project back to the given (and current) revision.
        /// </summary>
        /// <param name="revisionId">Current revision id of the project to go back to.</param>
        void RequestRestoreTo([NotNull] string revisionId);

        /// <summary>
        /// Request to take the state of the project back to the given revision, but do not change the current revision or history.
        /// </summary>
        /// <param name="revisionId">Revision id to go back to.</param>
        void RequestGoBackTo([NotNull] string revisionId);

        /// <summary>
        /// Clears the error state.
        /// </summary>
        void ClearError();

        /// <summary>
        /// Show the difference between both version of a conlicted file.
        /// </summary>
        /// <param name="path">Path of the file to show.</param>
        void RequestShowConflictedDifferences([NotNull] string path);

        /// <summary>
        /// Request to choose merge for the provided conflict.
        /// </summary>
        /// <param name="path">Path of the file to choose merge for.</param>
        void RequestChooseMerge([NotNull] string path);

        /// <summary>
        /// Request to choose mine for the provided conflict.
        /// </summary>
        /// <param name="paths">Paths of the files to choose mine for.</param>
        void RequestChooseMine([NotNull] string[] paths);

        /// <summary>
        /// Request to choose remote for the provided conflict.
        /// </summary>
        /// <param name="paths">Paths of the files to choose remote for.</param>
        void RequestChooseRemote([NotNull] string[] paths);

        /// <summary>
        /// Request sync to latest revision.
        /// </summary>
        void RequestSync();

        /// <summary>
        /// Request cancel current job.
        /// </summary>
        void RequestCancelJob();

        /// <summary>
        /// Request to turn on the service.
        /// </summary>
        /// <returns></returns>
        void RequestTurnOnService();

        /// <summary>
        /// Show the service page.
        /// </summary>
        void ShowServicePage();

        /// <summary>
        /// Show login page.
        /// </summary>
        void ShowLoginPage();

        /// <summary>
        /// Show no seat page.
        /// </summary>
        void ShowNoSeatPage();
    }
}
