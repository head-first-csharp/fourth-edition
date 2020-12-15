using System;
using Unity.Cloud.Collaborate.Models.Structures;
using JetBrains.Annotations;

namespace Unity.Cloud.Collaborate.Models
{
    internal interface IMainModel : IModel
    {
        /// <summary>
        /// Signal when the local state switches between conflicted or not.
        /// </summary>
        event Action<bool> ConflictStatusChange;

        /// <summary>
        /// Signal when an operation with progress has started or stopped.
        /// </summary>
        event Action<bool> OperationStatusChange;

        /// <summary>
        /// Signal with incremental details of the operation in progress.
        /// </summary>
        event Action<IProgressInfo> OperationProgressChange;

        /// <summary>
        /// Signal when an error has occurred.
        /// </summary>
        event Action<IErrorInfo> ErrorOccurred;

        /// <summary>
        /// Signal when the error has cleared.
        /// </summary>
        event Action ErrorCleared;

        /// <summary>
        /// Signal whether or not the there are remote revisions to be fetched.
        /// </summary>
        event Action<bool> RemoteRevisionsAvailabilityChange;

        /// <summary>
        /// Signal when the state of the back button is updated. For example: clearing it or showing a new one.
        /// The string included is the new label for the back navigation button. If that value is null, clear the back
        /// navigation.
        /// </summary>
        event Action<string> BackButtonStateUpdated;

        /// <summary>
        /// Returns true if there are remote revisions available.
        /// </summary>
        bool RemoteRevisionsAvailable { get; }

        /// <summary>
        /// Returns true if there's a conflict locally.
        /// </summary>
        bool Conflicted { get; }

        /// <summary>
        /// Returns progress info if there is any.
        /// </summary>
        [CanBeNull]
        IProgressInfo ProgressInfo { get; }

        /// <summary>
        /// Returns error info if there is any.
        /// </summary>
        [CanBeNull]
        IErrorInfo ErrorInfo { get; }

        /// <summary>
        /// Current tab index being displayed.
        /// </summary>
        int CurrentTabIndex { get; set; }

        /// <summary>
        /// Returns a history model.
        /// </summary>
        /// <returns>Singleton history model for this main model.</returns>
        [NotNull]
        IHistoryModel ConstructHistoryModel();

        /// <summary>
        /// Returns a Changes model.
        /// </summary>
        /// <returns>Singleton change model for this main model.</returns>
        [NotNull]
        IChangesModel ConstructChangesModel();

        /// <summary>
        /// Clears any set error.
        /// </summary>
        void ClearError();

        /// <summary>
        /// Sync to latest revision.
        /// </summary>
        void RequestSync();

        /// <summary>
        /// Request cancel current job.
        /// </summary>
        void RequestCancelJob();

        /// <summary>
        /// Returns the current back navigation. Null if none exists presently.
        /// </summary>
        /// <returns>Current back navigation id, text, and action.</returns>
        (string id, string text, Action backAction)? GetBackNavigation();

        /// <summary>
        /// Register back navigation to be made available to the user to navigate backwards in the UI.
        /// </summary>
        /// <param name="id">Id for the back event.</param>
        /// <param name="text">Text for the back label.</param>
        /// <param name="backAction">Action to perform to go back.</param>
        void RegisterBackNavigation(string id, string text, Action backAction);

        /// <summary>
        /// Unregister back navigation if the given id matches the currently displayed back navigation.
        /// </summary>
        /// <param name="id">Id for the back event.</param>
        /// <returns>True if id matched.</returns>
        bool UnregisterBackNavigation(string id);
    }
}
