using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Models
{
    internal interface IChangesModel : IModel
    {
        /// <summary>
        /// Event triggered when an updated change list is available.
        /// </summary>
        event Action UpdatedChangeList;

        /// <summary>
        /// Event triggered when an updated selection of change list is available.
        /// </summary>
        event Action OnUpdatedSelectedChanges;

        /// <summary>
        /// Event triggered when the busy status changes.
        /// </summary>
        event Action<bool> BusyStatusUpdated;

        /// <summary>
        /// Stored revision summary.
        /// </summary>
        [NotNull]
        string SavedRevisionSummary { get; set; }

        /// <summary>
        /// Stored search query.
        /// </summary>
        [NotNull]
        string SavedSearchQuery { get; set; }

        /// <summary>
        /// Number of toggled entries.
        /// </summary>
        int ToggledCount { get; }

        /// <summary>
        /// Total number of entries.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Number of conflicted entries.
        /// </summary>
        int ConflictedCount { get; }

        /// <summary>
        /// Whether or not conflicts exist.
        /// </summary>
        bool Conflicted { get; }

        /// <summary>
        /// Whether or not the model is busy with a request.
        /// </summary>
        bool Busy { get; }

        /// <summary>
        /// Request initial data to populate the change list.
        /// </summary>
        void RequestInitialData();

        /// <summary>
        /// Set the value of the toggle for the given path.
        /// </summary>
        /// <param name="path">Path to modify the toggle for.</param>
        /// <param name="toggled">Value to set the toggle to.</param>
        /// <returns>True if more than one entry has had its value change.</returns>
        bool UpdateEntryToggle([NotNull] string path, bool toggled);

        /// <summary>
        /// Get the list of toggled entries. Can be long running in the case of a large change list.
        /// </summary>
        /// <param name="query">Query to filter the entries via.</param>
        /// <returns>The filtered toggled list.</returns>
        [NotNull]
        IReadOnlyList<IChangeEntryData> GetToggledEntries([CanBeNull] string query = null);

        /// <summary>
        /// Get the list of untoggled entries. Can be long running in the case of a large change list.
        /// </summary>
        /// <param name="query">Query to filter the entries via.</param>
        /// <returns>The filtered untoggled list.</returns>
        [NotNull]
        IReadOnlyList<IChangeEntryData> GetUntoggledEntries([CanBeNull] string query = null);

        /// <summary>
        /// Get full list of changes. Can be long running in the case of a large change list.
        /// </summary>
        /// <param name="query">Query to filter the changes with</param>
        /// <returns>The filtered change list.</returns>
        [NotNull]
        IReadOnlyList<IChangeEntryData> GetAllEntries([CanBeNull] string query = null);

        /// <summary>
        /// Get the list of conflicted entries. Can be long running in the case of a large change list.
        /// </summary>
        /// <param name="query">Query to filter the entries via.</param>
        /// <returns>The filtered conflicted list.</returns>
        [NotNull]
        IReadOnlyList<IChangeEntryData> GetConflictedEntries([CanBeNull] string query = null);

        /// <summary>
        /// Request diff of the file at the given path.
        /// </summary>
        /// <param name="path">Path to file to diff.</param>
        void RequestDiffChanges([NotNull] string path);

        /// <summary>
        /// Request discard of the file at the given path.
        /// </summary>
        /// <param name="entry">Entry to discard.</param>
        void RequestDiscard([NotNull] IChangeEntry entry);

        /// <summary>
        /// Request discard of the given list of files.
        /// </summary>
        /// <param name="entries">List of entries to discard.</param>
        void RequestBulkDiscard([NotNull] IReadOnlyList<IChangeEntry> entries);

        /// <summary>
        /// Request publish with the given message and list of files.
        /// </summary>
        /// <param name="message">Message for the revision.</param>
        /// <param name="changes">Changes to publish.</param>
        void RequestPublish([NotNull] string message, [NotNull] IReadOnlyList<IChangeEntry> changes);

        /// <summary>
        /// Show the difference between both version of a conflicted file.
        /// </summary>
        /// <param name="path">Path of the file to show.</param>
        void RequestShowConflictedDifferences([NotNull] string path);

        /// <summary>
        /// Request to choose merge for the provided conflict.
        /// </summary>
        /// <param name="path">Path of the file to choose merge for.</param>
        void RequestChooseMerge([NotNull] string path);

        /// <summary>
        /// Request to choose mine for the provided conflict.
        /// </summary>
        /// <param name="paths">Paths of the files to choose mine for.</param>
        void RequestChooseMine([NotNull] string[] paths);

        /// <summary>
        /// Request to choose remote for the provided conflict.
        /// </summary>
        /// <param name="paths">Paths of the files to choose remote for.</param>
        void RequestChooseRemote([NotNull] string[] paths);
    }
}
