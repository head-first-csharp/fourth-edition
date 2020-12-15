using System;
using Unity.Cloud.Collaborate.Views;

namespace Unity.Cloud.Collaborate.Presenters
{
    internal interface IMainPresenter : IPresenter
    {
        /// <summary>
        /// Create the history presenter and wire it up to the history view.
        /// </summary>
        /// <param name="view">View to wire to.</param>
        /// <returns>The created presenter.</returns>
        IHistoryPresenter AssignHistoryPresenter(IHistoryView view);

        /// <summary>
        /// Create the changes presenter and wire it up to the changes view.
        /// </summary>
        /// <param name="view">View to wire to.</param>
        /// <returns>The created presenter.</returns>
        IChangesPresenter AssignChangesPresenter(IChangesView view);

        /// <summary>
        /// Request cancel current job.
        /// </summary>
        void RequestCancelJob();

        /// <summary>
        /// Update value of current tab index.
        /// </summary>
        /// <param name="index">New tab index.</param>
        void UpdateTabIndex(int index);

        /// <summary>
        /// Request a back navigation.
        /// </summary>
        void NavigateBack();
    }
}
