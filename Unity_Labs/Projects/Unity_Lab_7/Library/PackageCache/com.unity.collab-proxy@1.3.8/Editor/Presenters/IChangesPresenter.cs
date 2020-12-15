using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Presenters
{
    internal interface IChangesPresenter : IPresenter
    {
        /// <summary>
        /// Count of the number of toggled entries.
        /// </summary>
        int ToggledCount { get; }

        /// <summary>
        /// Count of the total number of entries.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Count of the number of conflicted entries.
        /// </summary>
        int ConflictedCount { get; }

        /// <summary>
        /// Whether or no a search is occuring.
        /// </summary>
        bool Searching { get; }

        /// <summary>
        /// Toggles or untoggles the entry associated with the give path. If the toggle impacts more than the existing
        /// entry, then this will return true to indicate that the toggled states must be refreshed.
        /// </summary>
        /// <param name="path">Path of the associated entry.</param>
        /// <param name="toggled">Whether or not the entry is now toggled.</param>
        /// <returns>True if the list must be refreshed.</returns>
        bool UpdateEntryToggle([NotNull] string path, bool toggled);

        /// <summary>
        /// Request a publish with the existing values known by the presenter.
        /// </summary>
        void RequestPublish();

        /// <summary>
        /// Request a discard for the file at the given path.
        /// </summary>
        /// <param name="entry">Entry to discard.</param>
        void RequestDiscard([NotNull] IChangeEntry entry);

        /// <summary>
        /// Request a diff of the file at the given path.
        /// </summary>
        /// <param name="path">Path of the file to diff.</param>
        void RequestDiffChanges([NotNull] string path);

        /// <summary>
        /// Provide the presenter with the latest search query.
        /// </summary>
        /// <param name="query">Latest search query.</param>
        void SetSearchQuery([NotNull] string query);

        /// <summary>
        /// Provide the latest revision summary to the presenter.
        /// </summary>
        /// <param name="message">Latest commit message.</param>
        void SetRevisionSummary([NotNull] string message);

        /// <summary>
        /// Number of entries in the group overflow menu.
        /// </summary>
        int GroupOverflowEntryCount { get; }

        /// <summary>
        /// Event handler for clicking the overflow button in the changes list.
        /// </summary>
        /// <param name="x">X position of the click.</param>
        /// <param name="y">Y position of the click.</param>
        void OnClickGroupOverflow(float x, float y);

        /// <summary>
        /// Number of entries in the conflict group overflow menu.
        /// </summary>
        int ConflictGroupOverflowEntryCount { get; }

        /// <summary>
        /// Event handler for clicking the overflow button in the conflicts list.
        /// </summary>
        /// <param name="x">X position of the click.</param>
        /// <param name="y">Y position of the click.</param>
        void OnClickConflictGroupOverflow(float x, float y);

        /// <summary>
        /// Show the difference between both version of a conlicted file.
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
        /// <param name="path">Path of the file to choose mine for.</param>
        void RequestChooseMine([NotNull] string path);

        /// <summary>
        /// Request to choose remote for the provided conflict.
        /// </summary>
        /// <param name="path">Path of the file to choose remote for.</param>
        void RequestChooseRemote([NotNull] string path);
    }
}
