namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal enum ChangeEntryStatus
    {
        None,
        Untracked,
        Ignored,
        Modified,
        Added,
        Deleted,
        Renamed,
        Copied,
        TypeChange,
        Unmerged,
        Unknown,
        Broken
    }

    internal interface IChangeEntry
    {
        string Path { get; }
        string OriginalPath { get; }
        ChangeEntryStatus Status { get; }
        bool Staged { get; }
        bool Unmerged { get; }
        object Tag { get; }

        /// <summary>
        /// Returns the string name of the status of this entry. Returns null if the status isn't used at present.
        /// </summary>
        /// <returns>String of used status. Null otherwise.</returns>
        string StatusToString();
    }
}
