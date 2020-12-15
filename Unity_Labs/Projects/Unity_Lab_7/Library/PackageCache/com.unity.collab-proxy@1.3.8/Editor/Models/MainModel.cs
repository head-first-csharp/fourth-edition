using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Models
{
    internal class MainModel : IMainModel
    {
        [NotNull]
        readonly ISourceControlProvider m_Provider;

        /// <inheritdoc />
        public event Action<bool> ConflictStatusChange;

        /// <inheritdoc />
        public event Action<bool> OperationStatusChange;

        /// <inheritdoc />
        public event Action<IProgressInfo> OperationProgressChange;

        /// <inheritdoc />
        public event Action<IErrorInfo> ErrorOccurred;

        /// <inheritdoc />
        public event Action ErrorCleared;

        /// <inheritdoc />
        public event Action<bool> RemoteRevisionsAvailabilityChange;

        /// <inheritdoc />
        public event Action<string> BackButtonStateUpdated;

        /// <inheritdoc />
        public event Action StateChanged;

        [NotNull]
        readonly IHistoryModel m_HistoryModel;
        [NotNull]
        readonly IChangesModel m_ChangesModel;

        (string id, string text, Action backEvent)? m_BackNavigation;

        public MainModel([NotNull] ISourceControlProvider provider)
        {
            m_Provider = provider;
            m_HistoryModel = new HistoryModel(m_Provider);
            m_ChangesModel = new ChangesModel(m_Provider);
        }

        /// <inheritdoc />
        public void OnStart()
        {
            // Setup events
            m_Provider.UpdatedOperationStatus += OnUpdatedOperationStatus;
            m_Provider.UpdatedOperationProgress += OnUpdatedOperationProgress;
            m_Provider.ErrorOccurred += OnErrorOccurred;
            m_Provider.ErrorCleared += OnErrorCleared;
            m_Provider.UpdatedConflictState += OnUpdatedConflictState;
            m_Provider.UpdatedRemoteRevisionsAvailability += OnUpdatedRemoteRevisionsAvailability;

            // Propagate event to "child" models.
            m_HistoryModel.OnStart();
            m_ChangesModel.OnStart();
        }

        /// <inheritdoc />
        public void OnStop()
        {
            // Clean up.
            m_Provider.UpdatedOperationStatus -= OnUpdatedOperationStatus;
            m_Provider.UpdatedOperationProgress -= OnUpdatedOperationProgress;
            m_Provider.ErrorOccurred -= OnErrorOccurred;
            m_Provider.ErrorCleared -= OnErrorCleared;
            m_Provider.UpdatedConflictState -= OnUpdatedConflictState;
            m_Provider.UpdatedRemoteRevisionsAvailability -= OnUpdatedRemoteRevisionsAvailability;

            // Propagate event to "child" models.
            m_HistoryModel.OnStop();
            m_ChangesModel.OnStop();
        }

        /// <inheritdoc />
        public void RestoreState(IWindowCache cache)
        {
            // Read in cached data.
            CurrentTabIndex = cache.TabIndex;
            StateChanged?.Invoke();

            // Propagate restore call to "child" models.
            m_HistoryModel.RestoreState(cache);
            m_ChangesModel.RestoreState(cache);
        }

        /// <inheritdoc />
        public void SaveState(IWindowCache cache)
        {
            // Cache data.
            cache.TabIndex = CurrentTabIndex;

            // Propagate save call to "child" models.
            m_HistoryModel.SaveState(cache);
            m_ChangesModel.SaveState(cache);
        }

        /// <inheritdoc />
        public bool RemoteRevisionsAvailable => m_Provider.GetRemoteRevisionAvailability();

        /// <inheritdoc />
        public bool Conflicted => m_Provider.GetConflictedState();

        /// <inheritdoc />
        public IProgressInfo ProgressInfo => m_Provider.GetProgressState();

        /// <inheritdoc />
        public IErrorInfo ErrorInfo => m_Provider.GetErrorState();

        /// <inheritdoc />
        public int CurrentTabIndex { get; set; }

        /// <inheritdoc />
        public IHistoryModel ConstructHistoryModel()
        {
            return m_HistoryModel;
        }

        /// <inheritdoc />
        public IChangesModel ConstructChangesModel()
        {
            return m_ChangesModel;
        }

        /// <inheritdoc />
        public void ClearError()
        {
            m_Provider.ClearError();
        }

        /// <inheritdoc />
        public void RequestSync()
        {
            m_Provider.RequestSync();
        }

        /// <inheritdoc />
        public void RequestCancelJob()
        {
            m_Provider.RequestCancelJob();
        }

        /// <inheritdoc />
        public (string id, string text, Action backAction)? GetBackNavigation()
        {
            return m_BackNavigation;
        }

        /// <inheritdoc />
        public void RegisterBackNavigation(string id, string text, Action backAction)
        {
            Assert.IsTrue(m_BackNavigation == null, "There should only be one back navigation registered at a time.");
            m_BackNavigation = (id, text, backAction);
            BackButtonStateUpdated?.Invoke(text);
        }

        /// <inheritdoc />
        public bool UnregisterBackNavigation(string id)
        {
            if (m_BackNavigation?.id != id) return false;

            m_BackNavigation = null;
            BackButtonStateUpdated?.Invoke(null);
            return true;
        }

        /// <summary>
        /// Event handler for when the availability of remote revisions changes.
        /// </summary>
        /// <param name="available">New availability status.</param>
        void OnUpdatedRemoteRevisionsAvailability(bool available)
        {
            RemoteRevisionsAvailabilityChange?.Invoke(available);
        }

        /// <summary>
        /// Event handler for when the conflicted status changes.
        /// </summary>
        /// <param name="conflicted">New conflicted status.</param>
        void OnUpdatedConflictState(bool conflicted)
        {
            ConflictStatusChange?.Invoke(conflicted);
        }

        void OnUpdatedOperationStatus(bool inProgress)
        {
            OperationStatusChange?.Invoke(inProgress);
        }

        void OnUpdatedOperationProgress(IProgressInfo progressInfo)
        {
            OperationProgressChange?.Invoke(progressInfo);
        }

        void OnErrorOccurred(IErrorInfo errorInfo)
        {
            ErrorOccurred?.Invoke(errorInfo);
        }

        void OnErrorCleared()
        {
            ErrorCleared?.Invoke();
        }
    }
}
