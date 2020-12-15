using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Views;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Presenters
{
    internal class MainPresenter : IMainPresenter
    {
        [NotNull]
        readonly IMainView m_View;
        [NotNull]
        readonly IMainModel m_Model;

        bool m_IsStarted;

        const string k_ErrorOccuredId = "error_occured";
        const string k_ConflictsDetectedId = "conflicts_detected";
        const string k_RevisionsAvailableId = "revisions_available";

        public MainPresenter([NotNull] IMainView view, [NotNull] IMainModel model)
        {
            m_View = view;
            m_Model = model;
        }

        /// <inheritdoc />
        public void Start()
        {
            Assert.IsFalse(m_IsStarted, "The presenter has already been started.");
            m_IsStarted = true;

            // Setup listeners.
            m_Model.ConflictStatusChange += OnConflictStatusChange;
            m_Model.OperationStatusChange += OnOperationStatusChange;
            m_Model.OperationProgressChange += OnOperationProgressChange;
            m_Model.ErrorOccurred += OnErrorOccurred;
            m_Model.ErrorCleared += OnErrorCleared;
            m_Model.RemoteRevisionsAvailabilityChange += OnRemoteRevisionsAvailabilityChange;
            m_Model.BackButtonStateUpdated += OnBackButtonStateUpdated;
            m_Model.StateChanged += OnStateChanged;

            // Update progress info.
            var progressInfo = m_Model.ProgressInfo;
            if (progressInfo != null)
            {
                OnOperationStatusChange(true);
                OnOperationProgressChange(m_Model.ProgressInfo);
            }

            // Update error info.
            var errorInfo = m_Model.ErrorInfo;
            if (errorInfo != null)
            {
                OnErrorOccurred(errorInfo);
            }
            else
            {
                OnErrorCleared();
            }

            // Get initial values.
            OnConflictStatusChange(m_Model.Conflicted);
            OnRemoteRevisionsAvailabilityChange(m_Model.RemoteRevisionsAvailable);

            PopulateInitialData();
        }

        /// <inheritdoc />
        public void Stop()
        {
            Assert.IsTrue(m_IsStarted, "The presenter has already been stopped.");
            m_IsStarted = false;

            m_Model.ConflictStatusChange -= OnConflictStatusChange;
            m_Model.OperationStatusChange -= OnOperationStatusChange;
            m_Model.OperationProgressChange -= OnOperationProgressChange;
            m_Model.ErrorOccurred -= OnErrorOccurred;
            m_Model.ErrorCleared -= OnErrorCleared;
            m_Model.RemoteRevisionsAvailabilityChange -= OnRemoteRevisionsAvailabilityChange;
            m_Model.BackButtonStateUpdated -= OnBackButtonStateUpdated;
            m_Model.StateChanged -= OnStateChanged;
        }

        /// <summary>
        /// Refresh state from the model.
        /// </summary>
        void OnStateChanged()
        {
            PopulateInitialData();
        }

        /// <summary>
        /// Populate the view with the initial data from the model.
        /// </summary>
        void PopulateInitialData()
        {
            // Set tab.
            m_View.SetTab(m_Model.CurrentTabIndex);

            // Update back navigation
            OnBackButtonStateUpdated(m_Model.GetBackNavigation()?.text);
        }

        /// <inheritdoc />
        public IHistoryPresenter AssignHistoryPresenter(IHistoryView view)
        {
            var presenter = new HistoryPresenter(view, m_Model.ConstructHistoryModel(), m_Model);
            view.Presenter = presenter;
            return presenter;
        }

        /// <inheritdoc />
        public IChangesPresenter AssignChangesPresenter(IChangesView view)
        {
            var presenter = new ChangesPresenter(view, m_Model.ConstructChangesModel(), m_Model);
            view.Presenter = presenter;
            return presenter;
        }

        /// <inheritdoc />
        public void RequestCancelJob()
        {
            m_Model.RequestCancelJob();
        }

        /// <inheritdoc />
        public void UpdateTabIndex(int index)
        {
            m_Model.CurrentTabIndex = index;
        }

        /// <inheritdoc />
        public void NavigateBack()
        {
            // Grab back action from the model, clear it, then invoke it.
            var nav = m_Model.GetBackNavigation();
            if (nav == null) return;
            m_Model.UnregisterBackNavigation(nav.Value.id);
            nav.Value.backAction.Invoke();
        }

        /// <summary>
        /// Display an alert if there is conflicts detected.
        /// </summary>
        /// <param name="conflicts">True if conflicts exist.</param>
        void OnConflictStatusChange(bool conflicts)
        {
            if (conflicts)
            {
                m_View.AddAlert(k_ConflictsDetectedId, AlertBox.AlertLevel.Alert, StringAssets.conflictsDetected);
            }
            else
            {
                m_View.RemoveAlert(k_ConflictsDetectedId);
            }
        }

        /// <summary>
        /// Display a progress bar if an operation has started.
        /// </summary>
        /// <param name="inProgress"></param>
        void OnOperationStatusChange(bool inProgress)
        {
            if (inProgress)
            {
                m_View.AddOperationProgress();
            }
            else
            {
                m_View.RemoveOperationProgress();
            }
        }

        /// <summary>
        /// Update progress bar with incremental details.
        /// </summary>
        /// <param name="progressInfo"></param>
        void OnOperationProgressChange(IProgressInfo progressInfo)
        {
            m_View.SetOperationProgress(progressInfo.Title, progressInfo.Details,
                progressInfo.PercentageComplete, progressInfo.CurrentCount,
                progressInfo.TotalCount, progressInfo.PercentageProgressType, progressInfo.CanCancel);
        }

        /// <summary>
        /// Display an error.
        /// </summary>
        /// <param name="errorInfo"></param>
        void OnErrorOccurred(IErrorInfo errorInfo)
        {
            if (errorInfo.Behaviour == ErrorInfoBehavior.Alert)
            {
                m_View.AddAlert(k_ErrorOccuredId, AlertBox.AlertLevel.Alert, errorInfo.Message, (StringAssets.clear, m_Model.ClearError));
            }
        }

        /// <summary>
        /// Clear the error state.
        /// </summary>
        void OnErrorCleared()
        {
            m_View.RemoveAlert(k_ErrorOccuredId);
        }

        /// <summary>
        /// Show or clear the revisions to fetch alert based on whether or not they are available.
        /// </summary>
        /// <param name="remoteRevisionsAvailable">True if there are remote revisions to pull down.</param>
        void OnRemoteRevisionsAvailabilityChange(bool remoteRevisionsAvailable)
        {
            if (remoteRevisionsAvailable)
            {
                m_View.AddAlert(k_RevisionsAvailableId, AlertBox.AlertLevel.Info, StringAssets.syncRemoteRevisionsMessage, (StringAssets.sync, m_Model.RequestSync));
            }
            else
            {
                m_View.RemoveAlert(k_RevisionsAvailableId);
            }
        }

        /// <summary>
        /// Clear or show back navigation button.
        /// </summary>
        /// <param name="title">Text to display next to the back navigation. Null means no back navigation.</param>
        void OnBackButtonStateUpdated([CanBeNull] string title)
        {
            if (title == null)
            {
                m_View.ClearBackNavigation();
            }
            else
            {
                m_View.DisplayBackNavigation(title);
            }
        }
    }
}
