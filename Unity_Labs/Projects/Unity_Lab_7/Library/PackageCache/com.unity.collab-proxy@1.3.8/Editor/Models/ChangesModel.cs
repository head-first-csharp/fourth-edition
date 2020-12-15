using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.UserInterface;
using Unity.Cloud.Collaborate.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Models
{
    internal class ChangesModel : IChangesModel
    {
        protected readonly ISourceControlProvider m_Provider;

        /// <inheritdoc />
        public event Action UpdatedChangeList;

        /// <inheritdoc />
        public event Action OnUpdatedSelectedChanges;

        /// <inheritdoc />
        public event Action<bool> BusyStatusUpdated;

        /// <inheritdoc />
        public event Action StateChanged;

        internal Dictionary<string, IChangeEntryData> entryData;

        internal Dictionary<string, bool> toggledEntries;

        IReadOnlyList<IChangeEntryData> m_Conflicted;

        readonly ChangeEntryData m_AllItem;

        readonly HashSet<string> m_Requests;

        const string k_RequestNewList = "request-new-list";

        /// <inheritdoc />
        public string SavedRevisionSummary { get; set; }

        /// <inheritdoc />
        public string SavedSearchQuery { get; set; }

        /// <inheritdoc />
        public int ToggledCount { get; private set; }

        /// <inheritdoc />
        public int TotalCount { get; private set; }

        /// <inheritdoc />
        public int ConflictedCount => m_Conflicted.Count;

        /// <inheritdoc />
        public bool Conflicted => m_Provider.GetConflictedState();

        /// <inheritdoc />
        public bool Busy => m_Requests.Count != 0;

        public ChangesModel(ISourceControlProvider provider)
        {
            m_Provider = provider;
            m_Requests = new HashSet<string>();
            m_AllItem = new ChangeEntryData { Entry = new ChangeEntry(string.Empty), All = true };
            entryData = new Dictionary<string, IChangeEntryData>();
            m_Conflicted = new List<IChangeEntryData>();
            toggledEntries = new Dictionary<string, bool>();
            SavedSearchQuery = string.Empty;
            SavedRevisionSummary = string.Empty;
        }

        /// <inheritdoc />
        public void OnStart()
        {
            // Setup events.
            m_Provider.UpdatedChangeList += OnUpdatedChangeList;
            m_Provider.UpdatedSelectedChangeList += OnUpdatedSelectedChangesList;
        }

        /// <inheritdoc />
        public void OnStop()
        {
            // Clean up.
            m_Provider.UpdatedChangeList -= OnUpdatedChangeList;
            m_Provider.UpdatedSelectedChangeList -= OnUpdatedSelectedChangesList;
        }

        /// <inheritdoc />
        public void RestoreState(IWindowCache cache)
        {
            // Populate data from cache.
            SavedRevisionSummary = cache.RevisionSummary;
            SavedSearchQuery = cache.ChangesSearchValue;
            toggledEntries = cache.SimpleSelectedItems ?? new Dictionary<string, bool>();

            StateChanged?.Invoke();
        }

        /// <inheritdoc />
        public void SaveState(IWindowCache cache)
        {
            // Save data.
            cache.RevisionSummary = SavedRevisionSummary;
            cache.ChangesSearchValue = SavedSearchQuery;
            cache.SimpleSelectedItems = new SelectedItemsDictionary(toggledEntries);
        }

        /// <summary>
        /// Event handler for when the source control provider receives an updated history list.
        /// </summary>
        void OnUpdatedChangeList()
        {
            // Only one request at a time.
            if (!AddRequest(k_RequestNewList)) return;
            m_Provider.RequestChangeList(OnReceivedChangeList);
        }

        void OnUpdatedSelectedChangesList(IReadOnlyList<string> list)
        {
            ToggleAllEntries(false);
            foreach (var path in list)
            {
                UpdateEntryToggle(path, true);
            }

            OnUpdatedSelectedChanges?.Invoke();
        }

        /// <summary>
        /// Event handler to receive changes from the provider.
        /// </summary>
        /// <param name="list">Change list received.</param>
        void OnReceivedChangeList([CanBeNull] IReadOnlyList<IChangeEntry> list)
        {
            if (list != null)
            {
                UpdateChangeList(list);
                UpdatedChangeList?.Invoke();
            }
            else
            {
                Debug.LogError("Failed to fetch latest change list.");
            }

            RemoveRequest(k_RequestNewList);
        }

        /// <summary>
        /// Convert and cache new list of changes.
        /// </summary>
        /// <param name="list">New list of changes.</param>
        internal virtual void UpdateChangeList([NotNull] IReadOnlyList<IChangeEntry> list)
        {
            TotalCount = list.Count;

            // Create a new set of containers.
            var newEntryData = new Dictionary<string, IChangeEntryData> { [string.Empty] = m_AllItem };
            var newToggledEntries = new Dictionary<string, bool>();
            var conflicted = new List<IChangeEntryData>();

            var all = m_AllItem.Toggled;
            var toggledCount = 0;
            foreach (var entry in list)
            {
                // Transfer toggled state from old lookup into new.
                toggledEntries.TryGetValue(entry.Path, out var toggled);
                toggled = toggled || all || entry.Staged;
                newToggledEntries[entry.Path] = toggled;

                // Create a new data item for the entry.
                var item = new ChangeEntryData { Entry = entry, Toggled = toggled };
                newEntryData.Add(entry.Path, item);

                // Update counts.
                if (toggled)
                {
                    toggledCount++;
                }
                if (entry.Unmerged)
                {
                    conflicted.Add(item);
                }
            }

            // Store the new containers.
            entryData = newEntryData;
            toggledEntries = newToggledEntries;
            ToggledCount = toggledCount;
            m_Conflicted = conflicted;

            UpdateAllItemToggle();
        }

        /// <inheritdoc />
        public virtual bool UpdateEntryToggle(string path, bool toggled)
        {
            var entry = (ChangeEntryData)entryData[path];

            // Toggle all items if needed.
            if (entry.All)
            {
                return ToggleAllEntries(toggled);
            }

            // Update the toggled count.
            if (entry.Toggled && !toggled)
            {
                ToggledCount--;
            }
            else if (!entry.Toggled && toggled)
            {
                ToggledCount++;
            }

            // Store the value in the dictionary and data item.
            toggledEntries[entry.Entry.Path] = toggled;
            entry.Toggled = toggled;

            // Update the "All" option if needed.
            return UpdateAllItemToggle();
        }

        /// <inheritdoc />
        public IReadOnlyList<IChangeEntryData> GetToggledEntries(string query = null)
        {
            // Filter items by search query
            query = StringUtility.TrimAndToLower(query);
            return entryData.Values.Where(e => !e.All && e.Toggled && e.Entry.Path.ToLower().Contains(query)).ToList();
        }

        /// <inheritdoc />
        public IReadOnlyList<IChangeEntryData> GetUntoggledEntries(string query = null)
        {
            // Filter items by search query
            query = StringUtility.TrimAndToLower(query);
            return entryData.Values.Where(e => !e.All && !e.Toggled && e.Entry.Path.ToLower().Contains(query)).ToList();
        }

        /// <inheritdoc />
        public IReadOnlyList<IChangeEntryData> GetAllEntries(string query = null)
        {
            // Filter items by search query
            query = StringUtility.TrimAndToLower(query);
            return entryData.Values.Where(e => e.Entry.Path.ToLower().Contains(query)).ToList();
        }

        /// <inheritdoc />
        public IReadOnlyList<IChangeEntryData> GetConflictedEntries(string query = null)
        {
            // Filter items by search query
            query = StringUtility.TrimAndToLower(query);
            return entryData.Values.Where(e => !e.All && e.Conflicted && e.Entry.Path.ToLower().Contains(query))
                .ToList();
        }

        /// <summary>
        /// Update the state of the "All" entry. If all entries are toggled, then "All" should be toggled too;
        /// otherwise, "All" should be untoggled.
        /// </summary>
        /// <returns>True if the "All" entry was modified.</returns>
        bool UpdateAllItemToggle()
        {
            // Update state of the "All" option
            var allItemToggled = m_AllItem.Toggled;

            if (entryData.Count == 0) return false;

            if (ToggledCount == entryData.Count - 1)
            {
                // If every entry is toggled, then set AllItem as toggled.
                toggledEntries[m_AllItem.Entry.Path] = true;
                m_AllItem.Toggled = true;
                return !allItemToggled;
            }

            // Otherwise, set AllItem as not toggled.
            toggledEntries[m_AllItem.Entry.Path] = false;
            m_AllItem.Toggled = false;
            return allItemToggled;
        }

        /// <summary>
        /// Toggle on or off all entries in the list.
        /// </summary>
        /// <param name="toggled">Whether to toggle off or on.</param>
        /// <returns>True if the list has been modified.</returns>
        bool ToggleAllEntries(bool toggled)
        {
            // Update all values in the dictionary.
            toggledEntries.Keys.ToList().ForEach(x => toggledEntries[x] = toggled);

            // Compute the number of toggled items (excluding the single All).
            if (toggled)
            {
                ToggledCount = entryData.Count - 1;
            }
            else
            {
                ToggledCount = 0;
            }

            // Update all values in the list.
            foreach (var kv in entryData)
            {
                ((ChangeEntryData)kv.Value).Toggled = toggled;
            }

            return true;
        }

        /// <summary>
        /// Add a started request.
        /// </summary>
        /// <param name="requestId">Id of the request to add.</param>
        /// <returns>False if the request already exists.</returns>
        bool AddRequest(string requestId)
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
        void RemoveRequest(string requestId)
        {
            Assert.IsTrue(m_Requests.Contains(requestId), $"Expects request to have first been made for it to have been finished: {requestId}");
            m_Requests.Remove(requestId);
            // Signal no background activity if no requests in progress
            if (m_Requests.Count == 0)
                BusyStatusUpdated?.Invoke(false);
        }

        /// <inheritdoc />
        public void RequestInitialData()
        {
            // Only one request at a time.
            if (!AddRequest(k_RequestNewList)) return;
            m_Provider.RequestChangeList(OnReceivedChangeList);
        }

        /// <inheritdoc />
        public void RequestDiffChanges(string path)
        {
            m_Provider.RequestDiffChanges(path);
        }

        /// <inheritdoc />
        public void RequestDiscard(IChangeEntry entry)
        {
            m_Provider.RequestDiscard(entry);
        }

        /// <inheritdoc />
        public void RequestBulkDiscard(IReadOnlyList<IChangeEntry> entries)
        {
            m_Provider.RequestBulkDiscard(entries);
        }

        /// <inheritdoc />
        public void RequestPublish(string message, IReadOnlyList<IChangeEntry> changes)
        {
            m_Provider.RequestPublish(message, changes);
        }

        /// <inheritdoc />
        public void RequestShowConflictedDifferences(string path)
        {
            m_Provider.RequestShowConflictedDifferences(path);
        }

        /// <inheritdoc />
        public void RequestChooseMerge(string path)
        {
            m_Provider.RequestChooseMerge(path);
        }

        /// <inheritdoc />
        public void RequestChooseMine(string[] paths)
        {
            m_Provider.RequestChooseMine(paths);
        }

        /// <inheritdoc />
        public void RequestChooseRemote(string[] paths)
        {
            m_Provider.RequestChooseRemote(paths);
        }

        /// <summary>
        /// Implementation of IChangeEntryData with each field given a setter so that the data can be updated.
        /// </summary>
        class ChangeEntryData : IChangeEntryData
        {
            /// <inheritdoc />
            public IChangeEntry Entry { get; set; }

            /// <inheritdoc />
            public bool Toggled { get; set; }

            /// <inheritdoc />
            public bool All { get; set; }

            /// <inheritdoc />
            public bool ToggleReadOnly => Entry.Staged;

            /// <inheritdoc />
            public bool Conflicted => Entry.Unmerged;
        }
    }
}
