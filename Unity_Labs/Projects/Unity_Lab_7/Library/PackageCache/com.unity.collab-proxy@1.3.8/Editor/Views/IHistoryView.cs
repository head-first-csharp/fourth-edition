using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;

namespace Unity.Cloud.Collaborate.Views
{
    internal interface IHistoryView : IView<IHistoryPresenter>
    {
        /// <summary>
        /// Set busy status in the view.
        /// </summary>
        /// <param name="busy">Whether or not the presenter is busy with a request.</param>
        void SetBusyStatus(bool busy);

        /// <summary>
        /// Set the history list to be displayed. If the history list is not the current display, the view should switch
        /// to it upon receiving this call.
        /// </summary>
        /// <param name="list">List of entries to display.</param>
        void SetHistoryList(IReadOnlyList<IHistoryEntry> list);

        /// <summary>
        /// Set the current page and max page so that the paginator can be populated.
        /// </summary>
        /// <param name="page">Current page.</param>
        /// <param name="max">Max page.</param>
        void SetPage(int page, int max);

        /// <summary>
        /// Set the single history entry to display the provided entry. If the history single entry view is not the
        /// current display, the view should switch to it upon receiving this call.
        /// </summary>
        /// <param name="entry">Entry to display.</param>
        void SetSelection([NotNull] IHistoryEntry entry);

        /// <summary>
        /// Display a dialogue to the user.
        /// </summary>
        /// <param name="title">Title for the dialogue.</param>
        /// <param name="message">Message inside the dialogue.</param>
        /// <param name="affirmative">Affirmative button text.</param>
        /// <returns>True if affirmative is clicked.</returns>
        bool DisplayDialogue([NotNull] string title, [NotNull] string message, [NotNull] string affirmative);

        /// <summary>
        /// Display a dialogue to the user.
        /// </summary>
        /// <param name="title">Title for the dialogue.</param>
        /// <param name="message">Message inside the dialogue.</param>
        /// <param name="affirmative">Affirmative button text.</param>
        /// <param name="negative">Negative button text.</param>
        /// <returns>True if affirmative is clicked.</returns>
        bool DisplayDialogue([NotNull] string title, [NotNull] string message, [NotNull] string affirmative, [NotNull] string negative);
    }
}
