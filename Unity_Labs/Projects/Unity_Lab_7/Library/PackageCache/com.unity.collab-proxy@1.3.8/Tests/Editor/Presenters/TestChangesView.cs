using System.Collections.Generic;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Views;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    internal class TestChangesView : IChangesView
    {
        public int SetBusyStatusCount;
        public bool? SetBusyStatusValue;

        public int SetSearchQueryCount;
        public string SetSearchQueryValue;

        public int SetRevisionSummaryCount;
        public string SetRevisionSummaryValue;

        public int SetConflictsCount;
        public IReadOnlyList<IChangeEntryData> SetConflictsValue;

        public int SetChangesCount;
        public IReadOnlyList<IChangeEntryData> SetChangesValue;

        public int SetToggledCountCount;
        public int? SetToggledCountValue;

        public int SetPublishEnabledCount;
        public bool? SetPublishEnabledValue;
        public string SetPublishEnabledReason;

        public int DisplayDialogueCount;

        public IChangesPresenter Presenter { get; set; }
        public void SetBusyStatus(bool busy)
        {
            SetBusyStatusCount++;
            SetBusyStatusValue = busy;
        }

        public void SetSearchQuery(string query)
        {
            SetSearchQueryCount++;
            SetSearchQueryValue = query;
        }

        public void SetRevisionSummary(string message)
        {
            SetRevisionSummaryCount++;
            SetRevisionSummaryValue = message;
        }

        public void SetConflicts(IReadOnlyList<IChangeEntryData> list)
        {
            SetConflictsCount++;
            SetConflictsValue = list;
        }

        public void SetChanges(IReadOnlyList<IChangeEntryData> list)
        {
            SetChangesCount++;
            SetChangesValue = list;
        }

        public void SetToggledCount(int count)
        {
            SetToggledCountCount++;
            SetToggledCountValue = count;
        }

        public void SetPublishEnabled(bool enabled, string reason = null)
        {
            SetPublishEnabledCount++;
            SetPublishEnabledValue = enabled;
            SetPublishEnabledReason = reason;
        }

        public bool DisplayDialogue(string title, string message, string affirmative)
        {
            DisplayDialogueCount++;
            return true;
        }

        public bool DisplayDialogue(string title, string message, string affirmative, string negative)
        {
            DisplayDialogueCount++;
            return true;
        }

        public void SetSelectedChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
