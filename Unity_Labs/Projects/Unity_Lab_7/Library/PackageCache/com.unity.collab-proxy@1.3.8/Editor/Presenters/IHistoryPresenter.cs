using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Presenters
{
    internal interface IHistoryPresenter : IPresenter
    {
        /// <summary>
        /// Request a move to the previous page. Ensures page number never goes below 0.
        /// </summary>
        void PrevPage();

        /// <summary>
        /// Request a move to the next page. Ensures page number doesn't go beyond the max number of pages.
        /// </summary>
        void NextPage();

        /// <summary>
        /// Set the revision id to request.
        /// </summary>
        [NotNull]
        string SelectedRevisionId { set; }

        /// <summary>
        /// Request to update the state of the project to a provided revision. If revision is in the past, then the
        /// state of the project at that point simply will be applied on top of the current without impacting history.
        /// </summary>
        /// <param name="revisionId">Revision id of the project to update to.</param>
        /// <param name="status">Status of the revision.</param>
        void RequestGoto([NotNull] string revisionId, HistoryEntryStatus status);

        /// <summary>
        /// Returns true if revert is supported.
        /// </summary>
        bool SupportsRevert { get; }

        /// <summary>
        /// Request to revert the specified files to the given revision.
        /// </summary>
        /// <param name="revisionId">Revision to revert the files back to.</param>
        /// <param name="files">Files to revert back.</param>
        void RequestRevert([NotNull] string revisionId, [NotNull] IReadOnlyList<string> files);
    }
}
