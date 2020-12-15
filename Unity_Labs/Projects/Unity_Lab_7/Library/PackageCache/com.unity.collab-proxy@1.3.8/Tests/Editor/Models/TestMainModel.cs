using System;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.UserInterface;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class TestMainModel : IMainModel
    {
        public int clearErrorCount;
        public int requestSyncCount;
        public int requestCancelJobCount;

        public IHistoryModel historyModel = new TestHistoryModel();
        public IChangesModel changesModel = new TestChangesModel();

        public (string id, string text, Action backAction)? backNavigation;

        public event Action<bool> ConflictStatusChange = delegate { };
        public void TriggerConflictStatusChange(bool conflict)
        {
            ConflictStatusChange(conflict);
        }

        public event Action<bool> OperationStatusChange = delegate { };
        public void TriggerOperationStatusChange(bool inProgress)
        {
            OperationStatusChange(inProgress);
        }

        public event Action<IProgressInfo> OperationProgressChange = delegate { };
        public void TriggerOperationProgressChange(IProgressInfo progressInfo)
        {
            OperationProgressChange(progressInfo);
        }

        public event Action<IErrorInfo> ErrorOccurred = delegate { };
        public void TriggerErrorOccurred(IErrorInfo errorInfo)
        {
            ErrorOccurred(errorInfo);
        }

        public event Action ErrorCleared = delegate { };
        public void TriggerErrorCleared()
        {
            ErrorCleared();
        }

        public event Action<bool> RemoteRevisionsAvailabilityChange = delegate { };
        public void TriggerRemoteRevisionsAvailabilityChange(bool available)
        {
            RemoteRevisionsAvailabilityChange(available);
        }

        public event Action<string> BackButtonStateUpdated = delegate {  };
        public void TriggerBackButtonStateUpdated(string backText)
        {
            BackButtonStateUpdated(backText);
        }

        public event Action StateChanged = delegate { };
        public void TriggerStateChanged()
        {
            StateChanged();
        }

        public bool RemoteRevisionsAvailable { get; set; }
        public bool Conflicted { get; set; }
        public IProgressInfo ProgressInfo { get; set; }
        public IErrorInfo ErrorInfo { get; set; }
        public int CurrentTabIndex { get; set; }

        public IHistoryModel ConstructHistoryModel()
        {
            return historyModel;
        }

        public IChangesModel ConstructChangesModel()
        {
            return changesModel;
        }

        public void ClearError()
        {
            clearErrorCount++;
        }

        public void RequestSync()
        {
            requestSyncCount++;
        }

        public void RequestCancelJob()
        {
            requestCancelJobCount++;
        }

        public (string id, string text, Action backAction)? GetBackNavigation()
        {
            return backNavigation;
        }

        public void RegisterBackNavigation(string id, string text, Action backAction)
        {
            Assert.IsNull(backNavigation);
            backNavigation = (id, text, backAction);
        }

        public bool UnregisterBackNavigation(string id)
        {
            if (backNavigation == null || backNavigation.Value.id != id)
                return false;
            backNavigation = null;
            return true;
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
