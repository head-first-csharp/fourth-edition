namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal interface IProgressInfo
    {
        string Title { get; }

        string Details { get; }

        int CurrentCount { get; }

        int TotalCount { get; }

        string LastErrorString { get; }

        ulong LastError { get; }

        bool CanCancel { get; }

        bool PercentageProgressType { get; }

        int PercentageComplete { get; }
    }
}
