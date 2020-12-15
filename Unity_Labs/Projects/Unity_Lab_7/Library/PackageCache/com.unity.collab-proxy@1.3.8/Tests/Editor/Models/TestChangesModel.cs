using System;
using System.Collections.Generic;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.UserInterface;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class TestChangesModel : IChangesModel
    {
        public int UpdateEntryToggleCount;
        public string UpdateEntryTogglePath;
        public bool? UpdateEntryToggleValue;

        public int GetToggledEntriesCount;
        public string GetToggledEntriesQuery;

        public int GetUntoggledEntriesCount;
        public string GetUntoggledEntriesQuery;

        public int GetAllEntriesCount;
        public string GetAllEntriesQuery;

        public int GetConflictedEntriesCount;
        public string GetConflictedEntriesQuery;

        public int RequestInitialDataCount;

        public int RequestDiscardCount;
        public IChangeEntry RequestDiscardEntry;

        public int RequestBulkDiscardCount;
        public IReadOnlyList<IChangeEntry> RequestBulkDiscardPaths;

        public int RequestDiffCount;
        public string RequestDiffPath;

        public int RequestPublishCount;
        public IReadOnlyList<IChangeEntry> RequestPublishList;

        public IReadOnlyList<IChangeEntryData> UntoggledEntries = new List<IChangeEntryData>();
        public IReadOnlyList<IChangeEntryData> ToggledEntries = new List<IChangeEntryData>();
        public IReadOnlyList<IChangeEntryData> AllEntries = new List<IChangeEntryData>();
        public IReadOnlyList<IChangeEntryData> ConflictedEntries = new List<IChangeEntryData>();

        public event Action UpdatedChangeList = delegate { };

        public event Action<bool> BusyStatusUpdated = delegate { };

        public event Action OnUpdatedSelectedChanges = delegate { };

        public event Action StateChanged = delegate { };

        public string SavedRevisionSummary { get; set; } = "";

        public string SavedSearchQuery { get; set; } = "";

        public int ToggledCount => ToggledEntries.Count;

        public int TotalCount => AllEntries.Count;

        public int ConflictedCount => ConflictedEntries.Count;

        public bool Conflicted => ConflictedCount != 0;

        public bool Busy { get; set; }

        public void TriggerUpdatedChangeList()
        {
            UpdatedChangeList();
        }

        public void TriggerBusyStatusUpdated(bool value)
        {
            BusyStatusUpdated(value);
        }

        public bool UpdateEntryToggle(string path, bool toggled)
        {
            UpdateEntryToggleCount++;
            UpdateEntryTogglePath = path;
            UpdateEntryToggleValue = toggled;
            return false;
        }

        public IReadOnlyList<IChangeEntryData> GetToggledEntries(string query = null)
        {
            GetToggledEntriesCount++;
            GetToggledEntriesQuery = query;
            return ToggledEntries;
        }

        public IReadOnlyList<IChangeEntryData> GetUntoggledEntries(string query = null)
        {
            GetUntoggledEntriesCount++;
            GetUntoggledEntriesQuery = query;
            return UntoggledEntries;
        }

        public IReadOnlyList<IChangeEntryData> GetAllEntries(string query = null)
        {
            GetAllEntriesCount++;
            GetAllEntriesQuery = query;
            return AllEntries;
        }

        public IReadOnlyList<IChangeEntryData> GetConflictedEntries(string query = null)
        {
            GetConflictedEntriesCount++;
            GetConflictedEntriesQuery = query;
            return ConflictedEntries;
        }

        public void RequestInitialData()
        {
            RequestInitialDataCount++;
        }

        public void RequestDiffChanges(string path)
        {
            RequestDiffCount++;
            RequestDiffPath = path;
        }

        public void RequestDiscard(IChangeEntry entry)
        {
            RequestDiscardCount++;
            RequestDiscardEntry = entry;
        }

        public void RequestBulkDiscard(IReadOnlyList<IChangeEntry> paths)
        {
            RequestBulkDiscardCount++;
            RequestBulkDiscardPaths = paths;
        }

        public void RequestPublish(string message, IReadOnlyList<IChangeEntry> changes = null)
        {
            RequestPublishCount++;
            RequestPublishList = changes;
        }

        public void RequestShowConflictedDifferences(string path)
        {
            throw new NotImplementedException();
        }

        public void RequestChooseMerge(string path)
        {
            throw new NotImplementedException();
        }

        public void RequestChooseMine(string[] paths)
        {
            throw new NotImplementedException();
        }

        public void RequestChooseRemote(string[] paths)
        {
            throw new NotImplementedException();
        }

        internal class ChangeEntryData : IChangeEntryData
        {
            public IChangeEntry Entry { get; set; }
            public bool Toggled { get; set; }
            public bool All { get; set; }
            public bool ToggleReadOnly { get; set; }
            public bool Conflicted { get; set; }
        }

        public void OnStart()
        {
            throw new NotImplementedException();
        }

        public void OnStop()
        {
            throw new NotImplementedException();
        }

        public void RestoreState(IWindowCache cache)
        {
            throw new NotImplementedException();
        }

        public void SaveState(IWindowCache cache)
        {
            throw new NotImplementedException();
        }
    }
}
