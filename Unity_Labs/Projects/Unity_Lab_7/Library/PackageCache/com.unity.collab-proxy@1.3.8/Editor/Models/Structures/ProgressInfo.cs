using System;

namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal struct ProgressInfo : IProgressInfo
    {
        public ProgressInfo(string title = default, string details = default, int currentCount = default, int totalCount = default, string lastErrorString = default, ulong lastError = default, bool canCancel = false, bool percentageProgressType = false, int percentageComplete = default)
        {
            Title = title;
            Details = details;
            CurrentCount = currentCount;
            TotalCount = totalCount;
            LastErrorString = lastErrorString;
            LastError = lastError;
            CanCancel = canCancel;
            PercentageProgressType = percentageProgressType;
            PercentageComplete = percentageComplete;
        }

        public string Title { get; }

        public string Details { get; }

        public int CurrentCount { get; }

        public int TotalCount { get; }

        public string LastErrorString { get; }

        public ulong LastError { get; }

        public bool CanCancel { get; }

        public bool PercentageProgressType { get; }

        public int PercentageComplete { get; }
    }
}
