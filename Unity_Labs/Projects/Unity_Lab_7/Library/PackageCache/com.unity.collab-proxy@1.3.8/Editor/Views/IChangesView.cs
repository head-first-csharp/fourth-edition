using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;

namespace Unity.Cloud.Collaborate.Views
{
    internal interface IChangesView : IView<IChangesPresenter>
    {
        /// <summary>
        /// Set busy status in the view.
        /// </summary>
        /// <param name="busy">Whether or not the presenter is busy with a request.</param>
        void SetBusyStatus(bool busy);

        /// <summary>
        /// Set the search query in the view.
        /// </summary>
        /// <param name="query">Latest search query to set.</param>
        void SetSearchQuery([NotNull] string query);

        /// <summary>
        /// Set the revision summary in the view.
        /// </summary>
        /// <param name="message">Latest summary to set.</param>
        void SetRevisionSummary([NotNull] string message);

        /// <summary>
        /// Set the conflicts to be displayed.
        /// </summary>
        /// <param name="list">List of conflicts to display.</param>
        void SetConflicts([NotNull] IReadOnlyList<IChangeEntryData> list);

        /// <summary>
        /// Set the changes to be selected.
        /// </summary>
        void SetSelectedChanges();

        /// <summary>
        /// Set the changes to be displayed.
        /// </summary>
        /// <param name="list">List of changes to be displayed.</param>
        void SetChanges([NotNull] IReadOnlyList<IChangeEntryData> list);

        /// <summary>
        /// Set the count of toggled entries.
        /// </summary>
        /// <param name="count">Latest toggled count.</param>
        void SetToggledCount(int count);

        /// <summary>
        /// Enable or disable the publish button based on the provided values. The optional reason is to be used as a
        /// hint to users about why the functionality is blocked.
        /// </summary>
        /// <param name="enabled">Whether or not the publish is to be enabled.</param>
        /// <param name="reason">Reason for the publish to be disabled.</param>
        void SetPublishEnabled(bool enabled, [CanBeNull] string reason = null);

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
