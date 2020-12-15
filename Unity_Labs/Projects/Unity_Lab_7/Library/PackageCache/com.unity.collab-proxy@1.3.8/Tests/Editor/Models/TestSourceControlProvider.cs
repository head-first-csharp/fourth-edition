using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class TestSourceControlProvider : ISourceControlProvider
    {
        public int RequestedHistoryRevisionCount;
        [CanBeNull]
        public string RequestedHistoryRevisionId;

        public int RequestedHistoryListCount;
        public int? RequestedHistoryListOffset;
        public int? RequestedHistoryListSize;

        public int RequestedHistoryCountCount;

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
        public int? RequestedRevertFileCount;

        public int RequestedChangeListCount;
        [CanBeNull]
        public Action<IReadOnlyList<IChangeEntry>> RequestedChangeListCallback;

        public int RequestedDiscardCount;
        [CanBeNull]
        public IChangeEntry RequestedDiscardEntry;

        public int RequestedBulkDiscardCount;
        [CanBeNull]
        public IReadOnlyList<IChangeEntry> RequestedBulkDiscardPaths;

        public int RequestedDiffChangesCount;
        [CanBeNull]
        public string RequestedDiffChangesPath;

        public int RequestedPublishCount;
        [CanBeNull]
        public string RequestedPublishMessage;
        [CanBeNull]
        public IReadOnlyList<IChangeEntry> RequestedPublishList;

        public bool RemoteRevisionAvailability = false;

        public bool ConflictedState = false;

        public event Action UpdatedChangeList = delegate { };
        public event Action UpdatedHistoryEntries = delegate { };
        public event Action<bool> UpdatedConflictState = delegate { };
        public event Action<bool> UpdatedRemoteRevisionsAvailability = delegate { };
        public event Action<ProjectStatus> UpdatedProjectStatus = delegate { };

        public event Action<bool> UpdatedOperationStatus = delegate { };
        public event Action<IProgressInfo> UpdatedOperationProgress = delegate { };

        public event Action<IErrorInfo> ErrorOccurred = delegate { };
        public event Action ErrorCleared = delegate { };
        public event Action<IReadOnlyList<string>> UpdatedSelectedChangeList = delegate { };

        public bool GetRemoteRevisionAvailability()
        {
            return RemoteRevisionAvailability;
        }

        public bool GetConflictedState()
        {
            return ConflictedState;
        }

        public IProgressInfo GetProgressState()
        {
            throw new NotImplementedException();
        }

        public IErrorInfo GetErrorState()
        {
            throw new NotImplementedException();
        }

        public ProjectStatus GetProjectStatus()
        {
            throw new NotImplementedException();
        }

        public void RequestChangeList(Action<IReadOnlyList<IChangeEntry>> callback)
        {
            RequestedChangeListCount++;
            RequestedChangeListCallback = callback;
        }

        public void RequestPublish(string message, IReadOnlyList<IChangeEntry> changes = null)
        {
            RequestedPublishCount++;
            RequestedPublishMessage = message;
            RequestedPublishList = changes;
        }

        public void TriggerUpdatedHistoryEntries()
        {
            Assert.NotNull(UpdatedHistoryEntries, "There should be a listener registered.");
            UpdatedHistoryEntries();
        }

        public void TriggerUpdatedChangeEntries()
        {
            Assert.NotNull(UpdatedHistoryEntries, "There should be a listener registered.");
            UpdatedChangeList();
        }

        public void RequestHistoryEntry(string revisionId, Action<IHistoryEntry> callback)
        {
            RequestedHistoryRevisionCount++;
            RequestedHistoryRevisionId = revisionId;
            callback(null);
        }

        public void RequestHistoryPage(int offset, int pageSize, Action<IReadOnlyList<IHistoryEntry>> callback)
        {
            RequestedHistoryListCount++;
            RequestedHistoryListOffset = offset;
            RequestedHistoryListSize = pageSize;
            callback(null);
        }

        public void RequestHistoryCount(Action<int?> callback)
        {
            RequestedHistoryCountCount++;
            callback(null);
        }

        public void RequestDiscard(IChangeEntry entry)
        {
            RequestedDiscardCount++;
            RequestedDiscardEntry = entry;
        }

        public void RequestBulkDiscard(IReadOnlyList<IChangeEntry> entries)
        {
            RequestedBulkDiscardCount++;
            RequestedBulkDiscardPaths = entries;
        }

        public void RequestDiffChanges(string path)
        {
            RequestedDiffChangesCount++;
            RequestedDiffChangesPath = path;
        }

        public bool SupportsRevert { get; } = false;
        public void RequestRevert(string revisionId, IReadOnlyList<string> files)
        {
            RequestedRevertCount++;
            RequestedRevertRevisionId = revisionId;
            RequestedRevertFileCount = files.Count;
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

        public void ClearError()
        {
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

        public void RequestSync()
        {
            throw new NotImplementedException();
        }

        public void RequestCancelJob()
        {
            throw new NotImplementedException();
        }

        public void RequestTurnOnService()
        {
            throw new NotImplementedException();
        }

        public void ShowServicePage()
        {
            throw new NotImplementedException();
        }

        public void ShowLoginPage()
        {
            throw new NotImplementedException();
        }

        public void ShowNoSeatPage()
        {
            throw new NotImplementedException();
        }
    }
}
