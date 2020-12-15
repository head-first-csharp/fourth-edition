using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Utilities;
using UnityEditor;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEditor.Collaboration.Collab;
using ProgressInfo = UnityEditor.Collaboration.ProgressInfo;

namespace Unity.Cloud.Collaborate.Models.Providers
{
    internal class Collab : ISourceControlProvider
    {
        const string k_KServiceUrl = "developer.cloud.unity3d.com";

        readonly RevisionsService m_RevisionsService;

        /// <inheritdoc />
        public event Action UpdatedChangeList;

        /// <inheritdoc />
        public event Action<IReadOnlyList<string>> UpdatedSelectedChangeList;

        /// <inheritdoc />
        public event Action<bool> UpdatedConflictState;

        /// <inheritdoc />
        public event Action<bool> UpdatedRemoteRevisionsAvailability;

        /// <inheritdoc />
        public event Action<ProjectStatus> UpdatedProjectStatus;

        /// <inheritdoc />
        public event Action<bool> UpdatedOperationStatus;

        /// <inheritdoc />
        public event Action<IProgressInfo> UpdatedOperationProgress;

        /// <inheritdoc />
        public event Action<IErrorInfo> ErrorOccurred;

        /// <inheritdoc />
        public event Action ErrorCleared;

        readonly List<IChangeEntry> m_Changes;
        bool m_ConflictCachedState;
        bool m_RemoteRevisionsAvailableState;

        // History entry requesting bits and bobs.
        readonly Queue<(int offset, int size, Action<int?, IReadOnlyList<IHistoryEntry>>)> m_HistoryRequests;
        [NotNull]
        IReadOnlyList<IHistoryEntry> m_HistoryEntries;
        (int offset, int size)? m_HistoryEntriesCache;
        [CanBeNull]
        IHistoryEntry m_HistoryEntryCache;
        int? m_HistoryEntryCountCache;
        string m_TipCache;

        [CanBeNull]
        IErrorInfo m_ErrorInfo;
        [CanBeNull]
        IProgressInfo m_ProgressInfo;

        ProjectStatus m_ProjectStatus;

        public Collab()
        {
            m_RevisionsService = new RevisionsService(instance, UnityConnect.instance);
            m_Changes = new List<IChangeEntry>();
            m_HistoryEntries = new List<IHistoryEntry>();
            m_HistoryRequests = new Queue<(int offset, int size, Action<int?, IReadOnlyList<IHistoryEntry>>)>();

            // Get initial values.
            var info = instance.collabInfo;
            m_ConflictCachedState = info.conflict;
            m_RemoteRevisionsAvailableState = info.update;
            m_TipCache = info.tip;
            m_ProgressInfo = info.inProgress ? ProgressInfoFromCollab(instance.GetJobProgress(0)) : null;
            m_ErrorInfo = instance.GetError(UnityConnect.UnityErrorFilter.ByContext | UnityConnect.UnityErrorFilter.ByChild, out var errInfo)
                ? ErrorInfoFromUnity(errInfo)
                : null;
            m_ProjectStatus = GetNewProjectStatus(info, UnityConnect.instance.connectInfo, UnityConnect.instance.projectInfo);

            SetupEvents();
        }

        /// <summary>
        /// Setup events for the provider.
        /// </summary>
        void SetupEvents()
        {
            // just connect notifier events.
            instance.ChangeItemsChanged += OnChangeItemsChanged;
            instance.SelectedChangeItemsChanged += OnSelectedChangeItemsChanged;
            instance.RevisionUpdated_V2 += OnRevisionUpdated;

            instance.CollabInfoChanged += OnCollabInfoChanged;
            instance.JobsCompleted += OnJobsCompleted;
            instance.ErrorOccurred_V2 += OnErrorOccurred;
            instance.ErrorCleared += OnErrorCleared;

            instance.StateChanged += OnCollabStateChanged;
            UnityConnect.instance.StateChanged += OnUnityConnectStateChanged;
            UnityConnect.instance.ProjectStateChanged += OnUnityConnectProjectStateChanged;

            m_RevisionsService.FetchRevisionsCallback += OnReceiveHistoryEntries;
        }

        #region Callback & Helper Methods

        /// <summary>
        /// Event handler for when the change list has changed.
        /// </summary>
        /// <param name="changes">New change list.</param>
        /// <param name="isFiltered">Whether or not the list is filtered. Should always be false.</param>
        void OnChangeItemsChanged(ChangeItem[] changes, bool isFiltered)
        {
            UpdateChanges(changes);
            UpdatedChangeList?.Invoke();
        }

        /// <summary>
        /// WIP method to handle partial publish in collab.
        /// </summary>
        /// <param name="changes">Received changes.</param>
        /// <param name="isFiltered">Whether or not it's a partial publish. Should always be true.</param>
        void OnSelectedChangeItemsChanged(ChangeItem[] changes, bool isFiltered)
        {
            // This is used by selective commit. Assert all API calls to here are setting isFiltered to true !
            Debug.Assert(isFiltered);
            var selectedChanges = changes.Select(e => e.Path).ToList();
            UpdatedSelectedChangeList?.Invoke(selectedChanges);
        }

        /// <summary>
        /// Event handler for when a revision has been created or updated. It's not called 100% of the time when a user
        /// publishes a new revision.
        /// </summary>
        /// <param name="info">New collab info.</param>
        /// <param name="rev">New revision id.</param>
        /// <param name="action">Action that occured.</param>
        void OnRevisionUpdated(CollabInfo info, string rev, string action)
        {
            // Invalidate the cache.
            m_HistoryEntriesCache = null;
            m_HistoryEntryCache = null;
            m_HistoryEntryCountCache = null;
            // Send update event.
            UpdatedHistoryEntries?.Invoke();

            OnCollabInfoChanged(info);
        }

        void OnCollabInfoChanged(CollabInfo info)
        {
            // Update conflict state.
            if (m_ConflictCachedState != info.conflict)
            {
                m_ConflictCachedState = info.conflict;
                UpdatedConflictState?.Invoke(info.conflict);
            }

            // Update revisions available state.
            if (m_RemoteRevisionsAvailableState != info.update)
            {
                m_RemoteRevisionsAvailableState = info.update;
                UpdatedRemoteRevisionsAvailability?.Invoke(info.update);
            }

            // Update history list if the tip has changed.
            if (m_TipCache != info.tip)
            {
                m_TipCache = info.tip;

                // Invalidate the cache.
                m_HistoryEntriesCache = null;
                m_HistoryEntryCache = null;
                m_HistoryEntryCountCache = null;

                // Send update event.
                UpdatedHistoryEntries?.Invoke();
            }

            // Update project state
            UpdateProjectStatus(info, UnityConnect.instance.connectInfo, UnityConnect.instance.projectInfo);

            // Update progress state.
            if (info.inProgress)
            {
                // Get progress info.
                var progressInfo = instance.GetJobProgress(0);
                Assert.IsNotNull(progressInfo);

                // Trigger start operation if not already known.
                if (m_ProgressInfo == null)
                {
                    UpdatedOperationStatus?.Invoke(true);
                }

                // Send progress info.
                m_ProgressInfo = ProgressInfoFromCollab(progressInfo);
                UpdatedOperationProgress?.Invoke(m_ProgressInfo);
            }
            else if (m_ProgressInfo != null)
            {
                // Signal end of job if job still exists
                m_ProgressInfo = null;
                UpdatedOperationStatus?.Invoke(false);
            }
        }

        void OnJobsCompleted(CollabInfo info)
        {
            // NOTE: The first start of collab sends a completion event with no prior progress info.
            // To handle this, skip sending completion event if there has been no start event.
            if (m_ProgressInfo == null) return;

            Assert.IsFalse(info.inProgress);
            m_ProgressInfo = null;
            UpdatedOperationStatus?.Invoke(false);
        }

        void OnErrorOccurred(UnityErrorInfo error)
        {
            if (m_ErrorInfo?.Code == error.code) return;
            m_ErrorInfo = ErrorInfoFromUnity(error);
            ErrorOccurred?.Invoke(m_ErrorInfo);
        }

        void OnErrorCleared()
        {
            m_ErrorInfo = null;
            ErrorCleared?.Invoke();
        }

        /// <summary>
        /// On receiving history result, remove the oldest request, send the received data, then make the next request.
        /// </summary>
        /// <param name="revisionsResult">Result from the history request.</param>
        void OnReceiveHistoryEntries(RevisionsResult revisionsResult)
        {
            Assert.AreNotEqual(0, m_HistoryRequests.Count, "There should be a history request.");
            var (offset, size, callback) = m_HistoryRequests.Dequeue();

            // Get results, cache, then send them.
            var results = revisionsResult?.Revisions.Select(RevisionToHistoryEntry).ToList();
            if (results != null)
            {
                m_HistoryEntries = results;
                m_HistoryEntriesCache = (offset, size);
                m_HistoryEntryCountCache = revisionsResult.RevisionsInRepo;
                callback(revisionsResult.RevisionsInRepo, m_HistoryEntries);
            }

            // Start the next request --> has to be outside of the callback.
            EditorApplication.delayCall += () => ConsumeHistoryQueue();
        }

        /// <summary>
        /// Event handler for receiving unity connect project state changes.
        /// </summary>
        /// <param name="projectInfo">New project info.</param>
        void OnUnityConnectProjectStateChanged(ProjectInfo projectInfo)
        {
            UpdateProjectStatus(instance.collabInfo, UnityConnect.instance.connectInfo, projectInfo);
        }

        /// <summary>
        /// Event handler for receiving collab state changes.
        /// </summary>
        /// <param name="info">New collab state.</param>
        void OnCollabStateChanged(CollabInfo info)
        {
            OnCollabInfoChanged(info);
        }

        /// <summary>
        /// Event handler for receiving collab state changes.
        /// </summary>
        /// <param name="connectInfo">UnityConnect connect info.</param>
        void OnUnityConnectStateChanged(ConnectInfo connectInfo)
        {
            UpdateProjectStatus(instance.collabInfo, connectInfo, UnityConnect.instance.projectInfo);
        }

        /// <summary>
        /// Update cached ready value and send event if it has changed.
        /// </summary>
        void UpdateProjectStatus(CollabInfo collabInfo, ConnectInfo connectInfo, ProjectInfo projectInfo)
        {
            var currentStatus = GetNewProjectStatus(collabInfo, connectInfo, projectInfo);
            if (m_ProjectStatus == currentStatus) return;
            m_ProjectStatus = currentStatus;
            UpdatedProjectStatus?.Invoke(m_ProjectStatus);
        }

        /// <summary>
        /// Returns the current project status.
        /// </summary>
        /// <returns>Current status of this project.</returns>
        static ProjectStatus GetNewProjectStatus(CollabInfo collabInfo, ConnectInfo connectInfo, ProjectInfo projectInfo)
        {
            // No UPID.
            if (!projectInfo.projectBound)
            {
                return ProjectStatus.Unbound;
            }

            if (!connectInfo.online)
            {
                return ProjectStatus.Offline;
            }

            if (connectInfo.maintenance || collabInfo.maintenance)
            {
                return ProjectStatus.Maintenance;
            }

            if (!connectInfo.loggedIn)
            {
                return ProjectStatus.LoggedOut;
            }

            if (!collabInfo.seat)
            {
                return ProjectStatus.NoSeat;
            }

            // UPID exists, but collab off.
            if (!instance.IsCollabEnabledForCurrentProject())
            {
                return ProjectStatus.Bound;
            }

            // Waiting for collab to connect and be ready.
            if (!instance.IsConnected() || !collabInfo.ready)
            {
                return ProjectStatus.Loading;
            }

            return ProjectStatus.Ready;
        }

        /// <summary>
        /// Consume the next entry on the history queue.
        /// </summary>
        /// <param name="afterEnqueue">True if an entry was just inserted. Starts the consumption cycle.</param>
        void ConsumeHistoryQueue(bool afterEnqueue = false)
        {
            // Start consuming the queue if the first entry was just enqueued.
            if (afterEnqueue && m_HistoryRequests.Count != 1) return;

            // Can't consume an empty queue.
            if (m_HistoryRequests.Count == 0) return;

            var (offset, size, callback) = m_HistoryRequests.Peek();
            // Execute next request. Discard if exception.
            try
            {
                m_RevisionsService.GetRevisions(offset, size);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                // Remove request and send failure callback.
                m_HistoryRequests.Dequeue();
                callback(null, null);
            }
        }

        /// <summary>
        /// Make a history request.
        /// </summary>
        /// <param name="offset">Offset for the request to start from.</param>
        /// <param name="size">Target length of the resultant list.</param>
        /// <param name="callback">Callback for the result.</param>
        void QueueHistoryRequest(int offset, int size, Action<int?, IReadOnlyList<IHistoryEntry>> callback)
        {
            m_HistoryRequests.Enqueue((offset, size, callback));
            ConsumeHistoryQueue(true);
        }

        /// <summary>
        /// Update cache of converted change entries from provided collab changes.
        /// </summary>
        /// <param name="changes">Received list of changes from collab.</param>
        void UpdateChanges(IEnumerable<Change> changes)
        {
            m_Changes.Clear();
            m_Changes.AddRange(changes.Select(change =>
                new ChangeEntry(change.path, change.path, ChangeEntryStatusFromCollabState(change.state),
                    false, IsCollabStateFlagSet(change.state, CollabStates.kCollabConflicted | CollabStates.kCollabPendingMerge), change))
                .Cast<IChangeEntry>());
        }

        /// <summary>
        /// Update cache of converted change entries from provided collab changes.
        /// </summary>
        /// <param name="changes">Received list of changes from collab.</param>
        void UpdateChanges(IEnumerable<ChangeItem> changes)
        {
            m_Changes.Clear();
            m_Changes.AddRange(changes.Select(change =>
                new ChangeEntry(change.Path, change.Path, ChangeEntryStatusFromCollabState(change.State),
                    false, IsCollabStateFlagSet(change.State, CollabStates.kCollabConflicted | CollabStates.kCollabPendingMerge), change))
                .Cast<IChangeEntry>());
        }

        /// <inheritdoc />
        public bool GetRemoteRevisionAvailability()
        {
            // Return cached value.
            return m_RemoteRevisionsAvailableState;
        }

        /// <inheritdoc />
        public bool GetConflictedState()
        {
            // Return cached value.
            return m_ConflictCachedState;
        }

        /// <inheritdoc />
        public IProgressInfo GetProgressState()
        {
            // Return cached value.
            return m_ProgressInfo;
        }

        /// <inheritdoc />
        public IErrorInfo GetErrorState()
        {
            return m_ErrorInfo;
        }

        /// <inheritdoc />
        public virtual ProjectStatus GetProjectStatus()
        {
            return m_ProjectStatus;
        }

        /// <inheritdoc />
        public void RequestChangeList(Action<IReadOnlyList<IChangeEntry>> callback)
        {
            var changes = instance.GetChangesToPublish_V2().changes;
            UpdateChanges(changes);
            callback(m_Changes);

            // Also check for errors.
            if (instance.GetError(UnityConnect.UnityErrorFilter.All, out var error) &&
                (CollabErrorCode)error.code != CollabErrorCode.Collab_ErrNone)
            {
                ErrorOccurred?.Invoke(ErrorInfoFromUnity(error));
            }
        }

        /// <inheritdoc />
        public void RequestPublish(string message, IReadOnlyList<IChangeEntry> changeEntries = null)
        {
            var changeItems = changeEntries?.Select(EntryToChangeItem).ToArray();

            instance.PublishAssetsAsync(message, changeItems);

            ChangeItem EntryToChangeItem(IChangeEntry entry)
            {
                return entry.Tag as ChangeItem;
            }
        }

        #endregion

        #region SourceControlHistoryCommands

        /// <inheritdoc />
        public event Action UpdatedHistoryEntries;

        /// <inheritdoc />
        public void RequestHistoryEntry(string revisionId, Action<IHistoryEntry> callback)
        {
            // Return cached entry if possible.
            if (m_HistoryEntryCache?.RevisionId == revisionId)
            {
                callback(m_HistoryEntryCache);
                return;
            }

            // Ensure that a cleanup occurs in the case of an exception.
            m_RevisionsService.FetchSingleRevisionCallback += OnFetchRevisionCallback;
            try
            {
                m_RevisionsService.GetRevision(revisionId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                m_RevisionsService.FetchSingleRevisionCallback -= OnFetchRevisionCallback;
                callback(null);
            }

            void OnFetchRevisionCallback(Revision? revision)
            {
                m_RevisionsService.FetchSingleRevisionCallback -= OnFetchRevisionCallback;
                // Failing to find the revision can result in a null revision or an empty revisionID.
                callback(string.IsNullOrEmpty(revision?.revisionID)
                    ? null
                    : RevisionToHistoryEntry(revision.GetValueOrDefault()));
            }
        }

        /// <inheritdoc />
        public void RequestHistoryPage(int offset, int pageSize, Action<IReadOnlyList<IHistoryEntry>> callback)
        {
            // Return cached entry is possible.
            if (m_HistoryEntriesCache?.offset == offset && m_HistoryEntriesCache?.size == pageSize)
            {
                callback(m_HistoryEntries);
                return;
            }

            // Queue up the request.
            QueueHistoryRequest(offset, pageSize, (_, r) => callback(r));
        }

        /// <inheritdoc />
        public void RequestHistoryCount(Action<int?> callback)
        {
            // Return cached value if possible.
            if (m_HistoryEntryCountCache != null)
            {
                callback(m_HistoryEntryCountCache);
                return;
            }

            QueueHistoryRequest(0, 0, (c, _) => callback(c));
        }

        /// <inheritdoc />
        public void RequestDiscard(IChangeEntry entry)
        {
            // Collab cannot revert a new file as it has nothing to go back to. So, instead we delete them.
            if (entry.Status == ChangeEntryStatus.Added)
            {
                File.Delete(entry.Path);
                // Notify ADB to refresh since a change has been made.
                AssetDatabase.Refresh();
            }
            else
            {
                instance.RevertFile(entry.Path, true);
            }
        }

        /// <inheritdoc />
        public void RequestBulkDiscard(IReadOnlyList<IChangeEntry> entries)
        {
            var revertEntries = new List<ChangeItem>();
            var deleteOccured = false;
            foreach (var entry in entries)
            {
                // Collab cannot revert a new file as it has nothing to go back to. So, instead we delete them.
                if (entry.Status == ChangeEntryStatus.Added)
                {
                    File.Delete(entry.Path);
                    deleteOccured = true;
                }
                else
                {
                    revertEntries.Add((ChangeItem)entry.Tag);
                }
            }

            // If a change has been made, notify the ADB to refresh.
            if (deleteOccured)
            {
                AssetDatabase.Refresh();
            }

            instance.RevertFiles(revertEntries.ToArray(), true);
        }

        /// <inheritdoc />
        public void RequestDiffChanges(string path)
        {
            instance.ShowDifferences(path);
        }

        /// <inheritdoc />
        public bool SupportsRevert { get; } = false;

        /// <inheritdoc />
        public void RequestRevert(string revisionId, IReadOnlyList<string> files)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void RequestUpdateTo(string revisionId)
        {
            instance.Update(revisionId, true);
        }

        /// <inheritdoc />
        public void RequestRestoreTo(string revisionId)
        {
            instance.ResyncToRevision(revisionId);
        }

        /// <inheritdoc />
        public void RequestGoBackTo(string revisionId)
        {
            instance.GoBackToRevision(revisionId, false);
        }

        /// <inheritdoc />
        public void ClearError()
        {
            instance.ClearErrors();
        }

        /// <inheritdoc />
        public void RequestShowConflictedDifferences(string path)
        {
            if (UnityEditor.Collaboration.Collab.IsDiffToolsAvailable())
            {
                instance.ShowConflictDifferences(path);
            }
            else
            {
                Debug.Log(StringAssets.noMergeToolIsConfigured);
            }
        }

        /// <inheritdoc />
        public void RequestChooseMerge(string path)
        {
            if (UnityEditor.Collaboration.Collab.IsDiffToolsAvailable())
            {
                instance.LaunchConflictExternalMerge(path);
            }
            else
            {
                Debug.Log(StringAssets.noMergeToolIsConfigured);
            }
        }

        /// <inheritdoc />
        public void RequestChooseMine(string[] paths)
        {
            instance.SetConflictsResolvedMine(paths);
        }

        /// <inheritdoc />
        public void RequestChooseRemote(string[] paths)
        {
            instance.SetConflictsResolvedTheirs(paths);
        }

        /// <inheritdoc />
        public void RequestSync()
        {
            QueueHistoryRequest(0, 1, Callback);

            void Callback(int? count, IReadOnlyList<IHistoryEntry> revisions)
            {
                if (revisions != null && revisions.Count > 0)
                {
                    instance.Update(revisions[0].RevisionId, true);
                }
                else
                {
                    Debug.LogError("Remote revision id is unknown. Please try again.");
                }
            }
        }

        /// <inheritdoc />
        public void RequestCancelJob()
        {
            instance.CancelJob(0);
        }

        /// <inheritdoc />
        public virtual void ShowServicePage()
        {
            SettingsService.OpenProjectSettings("Project/Services/Collaborate");
        }

        /// <inheritdoc />
        public void ShowLoginPage()
        {
            UnityConnect.instance.ShowLogin();
        }

        /// <inheritdoc />
        public void ShowNoSeatPage()
        {
            var unityConnect = UnityConnect.instance;
            var env = unityConnect.GetEnvironment();
            // Map environment to url - prod is special
            if (env == "production")
                env = "";
            else
                env += "-";

            var url = "https://" + env + k_KServiceUrl
                + "/orgs/" + unityConnect.GetOrganizationId()
                + "/projects/" + unityConnect.GetProjectName()
                + "/unity-teams/";
            Application.OpenURL(url);
        }

        /// <inheritdoc />
        public async void RequestTurnOnService()
        {
            try
            {
                await RequestTurnOnServiceInternal();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected async Task RequestTurnOnServiceInternal()
        {
            Assert.IsTrue(Threading.IsMainThread, "This must be run on the main thread.");

            // Fire up the update Genesis service flag request.
            var http = new HttpClientHandler { CookieContainer = new CookieContainer() };
            var client = new HttpClient(http);

            var projectGuid = UnityConnect.instance.projectInfo.projectGUID;
            var accessToken = UnityConnect.instance.GetAccessToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-UNITY-VERSION", InternalEditorUtility.GetFullUnityVersion());

            var fullUrl = $"{UnityConnect.instance.GetConfigurationURL(CloudConfigUrl.CloudCore)}/api/projects/{projectGuid}/service_flags";
            const string json = @"{ ""service_flags"": { ""collab"" : true} }";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PutAsync(client, fullUrl, content);

            // Success.
            if (response?.StatusCode == HttpStatusCode.OK)
            {
                SaveAssets();
                TurnOnCollabInternal();
            }
            // Error.
            else if (response?.StatusCode == HttpStatusCode.Forbidden)
            {
                ShowCredentialsError();
            }
            else
            {
                ShowGeneralError();
            }
        }

        protected virtual void SaveAssets()
        {
            instance.SaveAssets();
        }

        protected virtual Task<HttpResponseMessage> PutAsync(HttpClient client, string fullUrl, StringContent content)
        {
            return client.PutAsync(fullUrl, content);
        }

        protected virtual void TurnOnCollabInternal()
        {
            // enable the server from the client..
            instance.SetCollabEnabledForCurrentProject(true);
            // persist by marking collab on in settings
            PlayerSettings.SetCloudServiceEnabled("Collab", true);
        }

        protected virtual void ShowCredentialsError()
        {
            // TODO - ahmad :- Show an Error UI.
            Debug.LogError("You need owner privilege to enable or disable collab.");
        }

        protected virtual void ShowGeneralError()
        {
            // TODO - ahmad :- Show an Error UI.
            Debug.LogError("cannot enable collab");
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Converts a Collab Revision to an IHistoryEntry.
        /// </summary>
        /// <param name="revision">Revision to convert.</param>
        /// <returns>Resultant IHistoryEntry</returns>
        IHistoryEntry RevisionToHistoryEntry(Revision revision)
        {
            var time = DateTimeOffset.FromUnixTimeSeconds((long)revision.timeStamp);
            var entries = revision.entries.Select(ChangeActionToChangeEntry).ToList();
            var status = HistoryEntryStatus.Ahead;
            if (revision.isObtained)
                status = HistoryEntryStatus.Behind;
            if (revision.revisionID == m_RevisionsService.tipRevision)
                status = HistoryEntryStatus.Current;
            return new HistoryEntry(revision.revisionID, status, revision.author, revision.comment, time, entries);
        }

        /// <summary>
        /// Converts a Collab ChangeAction to an IChangeEntry.
        /// </summary>
        /// <param name="action">ChangeAction to convert.</param>
        /// <returns>Resultant IChangeEntry</returns>
        static IChangeEntry ChangeActionToChangeEntry(ChangeAction action)
        {
            var unmerged = false;
            var status = ChangeEntryStatus.None;
            switch (action.action.ToLower())
            {
                case "added":
                    status = ChangeEntryStatus.Added;
                    break;
                case "conflict":
                    status = ChangeEntryStatus.Unmerged;
                    unmerged = true;
                    break;
                case "deleted":
                    status = ChangeEntryStatus.Deleted;
                    break;
                case "ignored":
                    status = ChangeEntryStatus.Ignored;
                    break;
                case "renamed":
                case "moved":
                    status = ChangeEntryStatus.Renamed;
                    break;
                case "updated":
                    status = ChangeEntryStatus.Modified;
                    break;
                default:
                    Debug.LogError($"Unknown file status: {action.action}");
                    break;
            }

            return new ChangeEntry(action.path, status: status, unmerged: unmerged);
        }

        /// <summary>
        /// Converts a Collab CollabStates to an ChangeEntryStatus.
        /// Note that CollabStates is a bitwise flag, while
        /// ChangeEntryStatus is an enum, so ordering matters.
        /// </summary>
        /// <param name="state">ChangeAction to convert.</param>
        /// <returns>Resultant ChangeEntryStatus</returns>
        static ChangeEntryStatus ChangeEntryStatusFromCollabState(CollabStates state)
        {
            if (IsCollabStateFlagSet(state, CollabStates.kCollabIgnored))
            {
                return ChangeEntryStatus.Ignored;
            }
            if (IsCollabStateFlagSet(state, CollabStates.kCollabConflicted | CollabStates.kCollabPendingMerge))
            {
                return ChangeEntryStatus.Unmerged;
            }
            if (IsCollabStateFlagSet(state, CollabStates.kCollabAddedLocal))
            {
                return ChangeEntryStatus.Added;
            }
            if (IsCollabStateFlagSet(state, CollabStates.kCollabMovedLocal))
            {
                return ChangeEntryStatus.Renamed;
            }
            if (IsCollabStateFlagSet(state, CollabStates.kCollabDeletedLocal))
            {
                return ChangeEntryStatus.Deleted;
            }
            if (IsCollabStateFlagSet(state, CollabStates.kCollabCheckedOutLocal))
            {
                return ChangeEntryStatus.Modified;
            }

            return ChangeEntryStatus.Unknown;
        }

        /// <summary>
        /// Checks the state of a flag in CollabStates.
        /// </summary>
        /// <param name="state">State to check from.</param>
        /// <param name="flag">Flag to check in the state.</param>
        /// <returns>True if flag is set.</returns>
        static bool IsCollabStateFlagSet(CollabStates state, CollabStates flag)
        {
            return (state & flag) != 0;
        }

        static IProgressInfo ProgressInfoFromCollab([CanBeNull] ProgressInfo collabProgress)
        {
            if (collabProgress == null) return null;
            return new Structures.ProgressInfo(
                collabProgress.title,
                collabProgress.extraInfo,
                collabProgress.currentCount,
                collabProgress.totalCount,
                collabProgress.lastErrorString,
                collabProgress.lastError,
                collabProgress.canCancel,
                collabProgress.isProgressTypePercent,
                collabProgress.percentComplete);
        }

        static IErrorInfo ErrorInfoFromUnity(UnityErrorInfo error)
        {
            return new ErrorInfo(
                error.code,
                error.priority,
                error.behaviour,
                error.msg,
                error.shortMsg,
                error.codeStr);
        }

        #endregion

        enum CollabErrorCode
        {
            Collab_ErrNone = 0,
            Collab_Error,
            Collab_ErrProjectNotLinked,
            Collab_ErrNoSuchRepository,
            Collab_ErrNotLoggedIn,
            Collab_ErrNotConnected,
            Collab_ErrLocalCache,
            Collab_ErrNotUpToDate,
            Collab_ErrCannotGetRevision,
            Collab_ErrCannotGetRemote,
            Collab_ErrCannotGetLocal,
            Collab_ErrInvalidHost,
            Collab_ErrInvalidPort,
            Collab_ErrInvalidRevision,
            Collab_ErrNotSnapshot,
            Collab_ErrNoSuchRemoteFile,
            Collab_ErrNoSuchLocalFile,
            Collab_ErrJobNotDefined,
            Collab_ErrJobAlreadyRunning,
            Collab_ErrAlreadyUpToDate,
            Collab_ErrJobNotRunning,
            Collab_ErrNotSupported,
            Collab_ErrJobCancelled,
            Collab_ErrCannotSubmitChanges,
            Collab_ErrMD5DoesNotMatch,
            Collab_ErrRemoteChanged,
            Collab_ErrCannotCreateTempDir,
            Collab_ErrCannotDownloadEntry,
            Collab_ErrCannotCreatePath,
            Collab_ErrCannotCreateFile,
            Collab_ErrCannotCopyFile,
            Collab_ErrCannotMoveFile,
            Collab_ErrCannotDeleteFile,
            Collab_ErrCannotGetProjects,
            Collab_ErrCannotRestoreSnapshot,
            Collab_ErrFileWasAddedLocally,
            Collab_ErrFileIsModified,
            Collab_ErrFileIsMissing,
            Collab_ErrFileAlreadyExists,
            Collab_ErrAutomaticMergeBaseIsMissing,
            Collab_ErrSmartMergeConflicts,
            Collab_ErrTextMergeConflicts,
            Collab_ErrAutomaticMerge,
            Collab_ErrSmartMerge,
            Collab_ErrTextMerge,
            Collab_ErrExternalDiff,
            Collab_ErrExternalMerge,
            Collab_ErrParseJson,
            Collab_ErrWrongSerializationMode,
            Collab_ErrNoDiffRevisions,
            Collab_ErrWorkspaceChanged,
            Collab_ErrRefreshChannelAccess,
            Collab_ErrUpdateInProgress,
            Collab_ErrSoftLocksJobRunning,
            Collab_ErrCannotGetSoftLocks,
            Collab_ErrPostSoftLocks,
            Collab_ErrRequestCancelled,
            Collab_ErrCollabInErrorState,
            Collab_ErrUsageExceeded,
            Collab_ErrRepositoryLocked,
            Collab_ErrJobWaitingForSubTasks,
            Collab_ErrBadRequest = 400,
            Collab_ErrNotAuthorized = 401,
            Collab_ErrInternalServerError = 500,
            Collab_ErrBadGateway = 502,
            Collab_ErrServerUnavailable = 503,
            Collab_ErrSmartMergeSetConflictState,
            Collab_ErrTextMergeSetConflictState,
            Collab_ErrExternalMergeSetConflictState,
            Collab_ErrNoDiffMergeToolsConfigured,
            Collab_ErrUnsupportedDiffMergeToolConfigured,
            Collab_ErrNoSeat,
            Collab_ErrNoSeatHidden
        }
    }
}
