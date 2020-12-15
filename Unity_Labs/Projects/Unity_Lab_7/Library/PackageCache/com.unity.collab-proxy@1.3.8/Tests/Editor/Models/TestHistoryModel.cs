using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.UserInterface;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class TestHistoryModel : IHistoryModel
    {
        public event Action HistoryListUpdated = delegate { };
        public event Action<IReadOnlyList<IHistoryEntry>> HistoryListReceived = delegate { };
        public event Action<IHistoryEntry> SelectedRevisionReceived = delegate { };
        public event Action<bool> BusyStatusUpdated = delegate { };
        public event Action<int?> EntryCountUpdated = delegate { };
        public event Action StateChanged = delegate { };

        public int RequestedPageOfRevisionsCount;
        public int RequestedPageSize;

        public int RequestedSingleRevisionCount;
        [CanBeNull]
        public string RequestedRevisionId;

        public int RequestedEntryCountCount;

        public int RequestedUpdateToCount;
        [CanBeNull]
        public string RequestedUpdateToRevisionId;

        public int RequestedRestoreToCount;
        [CanBeNull]
        public string RequestedRestoreToRevisionId;

        public int RequestedGoBackToCount;
        [CanBeNull]
        public string RequestedGoBackToRevisionId;

        public int RequestedRevertCount;
        [CanBeNull]
        public string RequestedRevertRevisionId;
        public int RequestedRevertFileCount;

        public void SetNumberOfEntries(int count)
        {
            Assert.NotNull(EntryCountUpdated, "There should be an receiver for the entry number count event.");
            EntryCountUpdated.Invoke(count);
        }

        public void TriggerUpdatedEntryListEvent()
        {
            Assert.NotNull(HistoryListUpdated, "There should be an receiver for the history list updated event.");
            HistoryListUpdated();
        }

        public bool Busy { get; set; }
        public int PageNumber { get; set; }
        public string SelectedRevisionId { get; set; }
        public string SavedRevisionId { get; set; }
        public bool IsRevisionSelected => !string.IsNullOrEmpty(SelectedRevisionId);

        public void RequestPageOfRevisions(int pageSize)
        {
            RequestedPageSize = pageSize;
            RequestedPageOfRevisionsCount++;
        }

        public void RequestSingleRevision(string revisionId)
        {
            RequestedRevisionId = revisionId;
            RequestedSingleRevisionCount++;
        }

        public void RequestEntryNumber()
        {
            RequestedEntryCountCount++;
        }

        public void RequestUpdateTo(string revisionId)
        {
            RequestedUpdateToCount++;
            RequestedUpdateToRevisionId = revisionId;
        }

        public void RequestRestoreTo(string revisionId)
        {
            RequestedRestoreToCount++;
            RequestedRestoreToRevisionId = revisionId;
        }

        public void RequestGoBackTo(string revisionId)
        {
            RequestedGoBackToCount++;
            RequestedGoBackToRevisionId = revisionId;
        }

        public bool SupportsRevert { get; } = false;
        public void RequestRevert(string revisionId, IReadOnlyList<string> files)
        {
            RequestedRevertCount++;
            RequestedRevertRevisionId = revisionId;
            RequestedRevertFileCount = files.Count;
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
