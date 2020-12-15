using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Presenters;

namespace Unity.Cloud.Collaborate.Views
{
    internal interface IMainView : IView<IMainPresenter>
    {
        /// <summary>
        /// Add or update an alert with the provided id.
        /// </summary>
        /// <param name="id">Id of the alert to add or update.</param>
        /// <param name="level">Level of severity.</param>
        /// <param name="message">Message for the alert.</param>
        /// <param name="button">Optional button with text and a callback.</param>
        void AddAlert([NotNull] string id, AlertBox.AlertLevel level, [NotNull] string message, (string text, Action action)? button = null);

        /// <summary>
        /// Remove alert with the provided id.
        /// </summary>
        /// <param name="id">Id of the alert to remove.</param>
        void RemoveAlert([NotNull] string id);

        /// <summary>
        /// Switch to the given tab index.
        /// </summary>
        /// <param name="index">Index of tab to switch to.</param>
        void SetTab(int index);

        /// <summary>
        /// Display progress view. Should only be called once, so only call when progress starts.
        /// </summary>
        void AddOperationProgress();

        /// <summary>
        /// Hide progress view. Should only be called once, so only call when progress finishes.
        /// </summary>
        void RemoveOperationProgress();

        /// <summary>
        /// Update the progress value displayed in the progress view.
        /// </summary>
        /// <param name="title">Title of the job in progress.</param>
        /// <param name="details">Description/details/status of the job in progress.</param>
        /// <param name="percentage">Current percentage completion of the job in progress. Used for percentage display.</param>
        /// <param name="completed">Current number of job items completed. Used for discrete display.</param>
        /// <param name="total">Total number of job items completed. Used for discrete display.</param>
        /// <param name="isPercentage">True if the progress bar uses percentage, false if its discrete completed-of-total.</param>
        /// <param name="canCancel">True if the job in progress can be cancelled.</param>
        void SetOperationProgress(string title, string details, int percentage, int completed, int total, bool isPercentage, bool canCancel);

        /// <summary>
        /// Clear the current back navigation.
        /// </summary>
        void ClearBackNavigation();

        /// <summary>
        /// Set back navigation to be displayed with the provided text.
        /// </summary>
        /// <param name="text">Destination of the back navigation</param>
        void DisplayBackNavigation([NotNull] string text);
    }
}
