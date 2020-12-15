using System;
using System.Collections.Generic;

namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal enum HistoryEntryStatus
    {
        Ahead,
        Current,
        Behind
    }

    internal interface IHistoryEntry
    {
        HistoryEntryStatus Status { get; }

        string RevisionId { get; }

        string AuthorName { get; }

        string Message { get; }

        DateTimeOffset Time { get; }

        IReadOnlyList<IChangeEntry> Changes { get; }

        string GetGotoText();
    }
}
