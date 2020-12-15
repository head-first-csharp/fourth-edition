using System;
using System.Collections.Generic;
using Unity.Cloud.Collaborate.Assets;

namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal struct HistoryEntry : IHistoryEntry
    {
        public HistoryEntry(string revisionId = default, HistoryEntryStatus status = HistoryEntryStatus.Behind, string authorName = default, string message = default, DateTimeOffset time = default, IReadOnlyList<IChangeEntry> changes = default)
        {
            Status = status;
            RevisionId = revisionId;
            AuthorName = authorName;
            Message = message;
            Time = time;
            Changes = changes;
        }

        public HistoryEntryStatus Status { get; }
        public string RevisionId { get; }
        public string AuthorName { get; }
        public string Message { get; }
        public DateTimeOffset Time { get; }
        public IReadOnlyList<IChangeEntry> Changes { get; }
        public string GetGotoText()
        {
            switch (Status)
            {
                case HistoryEntryStatus.Ahead:
                    return StringAssets.update;
                case HistoryEntryStatus.Current:
                    return StringAssets.restore;
                case HistoryEntryStatus.Behind:
                    return StringAssets.goBackTo;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
