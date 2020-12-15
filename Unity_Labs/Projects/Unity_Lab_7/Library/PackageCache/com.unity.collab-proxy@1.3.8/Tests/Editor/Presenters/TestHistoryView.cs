using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Views;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    internal class TestHistoryView : IHistoryView
    {
        public IHistoryPresenter Presenter { get; set; }

        public bool DialogueReturnValue = true;

        public bool? BusyStatus;
        public int? Page;
        public int? MaxPage;
        [CanBeNull]
        public IHistoryEntry ReceivedEntry;
        [CanBeNull]
        public string DialogueTitle;
        [CanBeNull]
        public string DialogueMessage;
        [CanBeNull]
        public string DialogueAffirmative;
        [CanBeNull]
        public string DialogueNegative;
        [CanBeNull]
        public IReadOnlyList<IHistoryEntry> ReceivedList;

        public void SetBusyStatus(bool busy)
        {
            BusyStatus = busy;
        }

        public void SetHistoryList(IReadOnlyList<IHistoryEntry> list)
        {
            ReceivedList = list;
            ReceivedEntry = null;
        }

        public void SetPage(int page, int max)
        {
            Page = page;
            MaxPage = max;
        }

        public void SetSelection(IHistoryEntry entry)
        {
            ReceivedEntry = entry;
            ReceivedList = null;
        }

        public bool DisplayDialogue(string title, string message, string affirmative)
        {
            DialogueTitle = title;
            DialogueMessage = message;
            DialogueAffirmative = affirmative;
            DialogueNegative = null;
            return DialogueReturnValue;
        }

        public bool DisplayDialogue(string title, string message, string affirmative, string negative)
        {
            DialogueTitle = title;
            DialogueMessage = message;
            DialogueAffirmative = affirmative;
            DialogueNegative = negative;
            return DialogueReturnValue;
        }
    }
}
