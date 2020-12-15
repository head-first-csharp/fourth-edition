using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Models.Structures;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Models.Providers
{
    [UsedImplicitly]
    internal class MockSourceControlProvider : ISourceControlProvider
    {
        public event Action UpdatedChangeList = delegate { };
        public event Action<IReadOnlyList<string>> UpdatedSelectedChangeList = delegate { };
        public event Action UpdatedHistoryEntries = delegate { };
        public event Action<bool> UpdatedConflictState = delegate { };
        public event Action<bool> UpdatedRemoteRevisionsAvailability = delegate { };
        public event Action<ProjectStatus> UpdatedProjectStatus = delegate { };

        public event Action<bool> UpdatedOperationStatus = delegate { };
        public event Action<IProgressInfo> UpdatedOperationProgress = delegate { };

        public event Action<IErrorInfo> ErrorOccurred = delegate { };
        public event Action ErrorCleared = delegate { };

        List<IChangeEntry> m_Changes;
        readonly List<IHistoryEntry> m_History;

        public MockSourceControlProvider()
        {
            m_Changes = new List<IChangeEntry>
            {
                new ChangeEntry("SomePath", "SomeOriginalPath", ChangeEntryStatus.Modified),
                new ChangeEntry("SomeConflictedPath2", "SomeOriginalPath", ChangeEntryStatus.Modified, unmerged: true),
                new ChangeEntry("SomePath3", "SomeOriginalPath", ChangeEntryStatus.Modified),
                new ChangeEntry("SomeConflictedPath4", "SomeOriginalPath", ChangeEntryStatus.Modified, unmerged: true),
                new ChangeEntry("SomeConflictedPath5", "SomeOriginalPath", ChangeEntryStatus.Modified, unmerged: true)
            };

            var historyChanges = new List<IChangeEntry>
            {
                new ChangeEntry("HistoryItemPath", "HistoryItemOriginalPath", ChangeEntryStatus.Modified, true)
            };
            m_History = new List<IHistoryEntry>
            {
                new HistoryEntry("HistoryItemRevisionId", HistoryEntryStatus.Current,"HistoryItemAuthorName", "HistoryItemMessage", DateTimeOffset.Now, historyChanges)
            };
        }

        /// <summary>
        /// Safely delays a call onto the main thread.
        /// </summary>
        /// <param name="call">Call to delay.</param>
        static void SafeDelayCall(Action call)
        {
            Task.Delay(2000).ContinueWith(_ => EditorApplication.delayCall += () => call());
        }

        public void SetChanges(List<IChangeEntry> changes)
        {
            m_Changes = changes;
            NotifyChanges();
        }

        IReadOnlyList<IChangeEntry> CloneChanges()
        {
            return m_Changes.Select(item => item).ToList();
        }

        IReadOnlyList<IHistoryEntry> CloneHistory()
        {
            return m_History.Select(item => item).ToList();
        }

        void NotifyChanges()
        {
            UpdatedChangeList?.Invoke();
        }

        void NotifyHistory()
        {
            UpdatedHistoryEntries?.Invoke();
        }

        public bool GetRemoteRevisionAvailability()
        {
            return false;
        }

        public bool GetConflictedState()
        {
            return m_Changes.Any(e => e.Unmerged);
        }

        public IProgressInfo GetProgressState()
        {
            return new ProgressInfo();
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
            callback(CloneChanges());
        }

        public void RequestPublish(string message, IReadOnlyList<IChangeEntry> changes = null)
        {
            throw new NotImplementedException();
        }

        public void RequestHistoryEntry(string revisionId, Action<IHistoryEntry> callback)
        {
            SafeDelayCall(() =>
            {
                try
                {
                    callback(CloneHistory().FirstOrDefault(e => e.RevisionId == revisionId));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    callback(CloneHistory().FirstOrDefault(i => i.RevisionId == revisionId));
                }
            });
        }

        public void RequestHistoryPage(int offset, int pageSize, Action<IReadOnlyList<IHistoryEntry>> callback)
        {
            var safeSize = Math.Min(m_History.Count - offset, pageSize);
            SafeDelayCall(() =>
            {
                try
                {
                    Assert.Greater(m_History.Count, offset, "The offset should never be greater than the total number of history entries.");
                    callback(CloneHistory().ToList().GetRange(offset, safeSize));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    callback(null);
                }
            });
        }

        public void RequestHistoryCount(Action<int?> callback)
        {
            SafeDelayCall(() => callback(m_History.Count));
        }

        public void RequestDiscard(IChangeEntry entry)
        {
            throw new NotImplementedException();
        }

        public void RequestBulkDiscard(IReadOnlyList<IChangeEntry> entries)
        {
            throw new NotImplementedException();
        }

        public void RequestDiffChanges(string path)
        {
            throw new NotImplementedException();
        }

        public bool SupportsRevert { get; } = false;
        public void RequestRevert(string revisionId, IReadOnlyList<string> files)
        {
            throw new NotImplementedException();
        }

        public void RequestUpdateTo(string revisionId)
        {
            throw new NotImplementedException();
        }

        public void RequestRestoreTo(string revisionId)
        {
            throw new NotImplementedException();
        }

        public void RequestGoBackTo(string revisionId)
        {
            throw new NotImplementedException();
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
