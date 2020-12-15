using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Models
{
    internal class HistoryModel : IHistoryModel
    {
        [NotNull]
        readonly ISourceControlProvider m_Provider;

        [NotNull]
        readonly HashSet<string> m_Requests;
        const string k_RequestPage = "request-page";
        const string k_RequestEntry = "request-entry";
        const string k_RequestEntryNumber = "request-entry-number";

        /// <inheritdoc />
        public event Action HistoryListUpdated;

        /// <inheritdoc />
        public event Action<IReadOnlyList<IHistoryEntry>> HistoryListReceived;

        /// <inheritdoc />
        public event Action<IHistoryEntry> SelectedRevisionReceived;

        /// <inheritdoc />
        public event Action<bool> BusyStatusUpdated;

        /// <inheritdoc />
        public event Action<int?> EntryCountUpdated;

        /// <inheritdoc />
        public event Action StateChanged;

        public HistoryModel([NotNull] ISourceControlProvider provider)
        {
            m_Provider = provider;
            m_Requests = new HashSet<string>();
            SelectedRevisionId = string.Empty;
            SavedRevisionId = string.Empty;
        }

        /// <inheritdoc />
        public void OnStart()
        {
            // Setup events
            m_Provider.UpdatedHistoryEntries += OnUpdatedHistoryEntries;
        }

        /// <inheritdoc />
        public void OnStop()
        {
            // Clean up.
            m_Provider.UpdatedHistoryEntries -= OnUpdatedHistoryEntries;
        }

        /// <inheritdoc />
        public void RestoreState(IWindowCache cache)
        {
            // Populate data.
            PageNumber = cache.HistoryPageNumber;
            SavedRevisionId = cache.SelectedHistoryRevision;

            StateChanged?.Invoke();
        }

        /// <inheritdoc />
        public void SaveState(IWindowCache cache)
        {
            // Update cache.
            cache.HistoryPageNumber = PageNumber;
            cache.SelectedHistoryRevision = SelectedRevisionId;
        }

        /// <summary>
        /// Event handler for when the requested history entry count has been received.
        /// </summary>
        /// <param name="entryCount">Received entry count.</param>
        void OnReceivedHistoryEntryCount(int? entryCount)
        {
            RemoveRequest(k_RequestEntryNumber);
            EntryCountUpdated?.Invoke(entryCount);
        }

        /// <summary>
        /// Event handler for when the requested page of history entries has been received.
        /// </summary>
        /// <param name="list">Received list of entries.</param>
        void OnReceivedHistoryPage(IReadOnlyList<IHistoryEntry> list)
        {
            RemoveRequest(k_RequestPage);
            HistoryListReceived?.Invoke(list);
        }

        /// <summary>
        /// Event handler for when a requested single history entry has been received.
        /// </summary>
        /// <param name="entry">Received entry.</param>
        void OnReceivedHistoryEntry(IHistoryEntry entry)
        {
            RemoveRequest(k_RequestEntry);
            SelectedRevisionReceived?.Invoke(entry);
        }

        /// <summary>
        /// Event handler for when the provider has received an updated history list.
        /// </summary>
        void OnUpdatedHistoryEntries()
        {
            HistoryListUpdated?.Invoke();
        }

        /// <inheritdoc />
        public void RequestPageOfRevisions(int pageSize)
        {
            // Only one request at a time.
            if (!AddRequest(k_RequestPage)) return;

            SelectedRevisionId = string.Empty;
            m_Provider.RequestHistoryPage(PageNumber * pageSize, pageSize, OnReceivedHistoryPage);
        }

        /// <inheritdoc />
        public void RequestSingleRevision(string revisionId)
        {
            // Only one request at a time.
            if (!AddRequest(k_RequestEntry)) return;

            SavedRevisionId = string.Empty;
            SelectedRevisionId = revisionId;
            m_Provider.RequestHistoryEntry(revisionId, OnReceivedHistoryEntry);
        }

        /// <inheritdoc />
        public void RequestEntryNumber()
        {
            // Only one request at a time.
            if (!AddRequest(k_RequestEntryNumber)) return;

            m_Provider.RequestHistoryCount(OnReceivedHistoryEntryCount);
        }

        /// <inheritdoc />
        public void RequestUpdateTo(string revisionId)
        {
            m_Provider.RequestUpdateTo(revisionId);
        }

        /// <inheritdoc />
        public void RequestRestoreTo(string revisionId)
        {
            m_Provider.RequestRestoreTo(revisionId);
        }

        /// <inheritdoc />
        public void RequestGoBackTo(string revisionId)
        {
            m_Provider.RequestGoBackTo(revisionId);
        }

        /// <inheritdoc />
        public bool SupportsRevert => m_Provider.SupportsRevert;

        /// <inheritdoc />
        public void RequestRevert(string revisionId, IReadOnlyList<string> files)
        {
            m_Provider.RequestRevert(revisionId, files);
        }

        /// <summary>
        /// Add a started request.
        /// </summary>
        /// <param name="requestId">Id of the request to add.</param>
        /// <returns>False if the request already exists.</returns>
        bool AddRequest([NotNull] string requestId)
        {
            if (m_Requests.Contains(requestId)) return false;
            m_Requests.Add(requestId);
            // Signal background activity if this is the only thing running.
            if (m_Requests.Count == 1)
                BusyStatusUpdated?.Invoke(true);
            return true;
        }

        /// <summary>
        /// Remove a finished request.
        /// </summary>
        /// <param name="requestId">Id of the request to remove.</param>
        void RemoveRequest([NotNull] string requestId)
        {
            Assert.IsTrue(m_Requests.Contains(requestId), $"Expects request to have first been made for it to have been finished: {requestId}");
            m_Requests.Remove(requestId);
            // Signal no background activity if no requests in progress
            if (m_Requests.Count == 0)
                BusyStatusUpdated?.Invoke(false);
        }

        /// <inheritdoc />
        public bool Busy => m_Requests.Count != 0;

        /// <inheritdoc />
        public int PageNumber { get; set; }

        /// <inheritdoc />
        public string SelectedRevisionId { get; private set; }

        /// <inheritdoc />
        public string SavedRevisionId { get; private set; }

        /// <inheritdoc />
        public bool IsRevisionSelected => !string.IsNullOrEmpty(SelectedRevisionId);
    }
}
