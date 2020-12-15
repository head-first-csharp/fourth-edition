using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components.Menus;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Utilities;
using Unity.Cloud.Collaborate.Views;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Presenters
{
    internal class ChangesPresenter : IChangesPresenter
    {
        [NotNull]
        readonly IChangesView m_View;
        [NotNull]
        readonly IChangesModel m_Model;
        [NotNull]
        readonly IMainModel m_MainModel;

        bool m_IsStarted;

        public ChangesPresenter([NotNull] IChangesView view, [NotNull] IChangesModel model, [NotNull] IMainModel mainModel)
        {
            m_View = view;
            m_Model = model;
            m_MainModel = mainModel;
        }

        /// <inheritdoc />
        public void Start()
        {
            Assert.IsFalse(m_IsStarted, "The presenter has already been started.");
            m_IsStarted = true;

            m_Model.UpdatedChangeList += OnUpdatedChangeList;
            m_Model.OnUpdatedSelectedChanges += OnUpdatedPartiallySelectedChanges;
            m_Model.BusyStatusUpdated += OnBusyStatusUpdated;
            m_Model.StateChanged += OnStateChanged;
            m_MainModel.RemoteRevisionsAvailabilityChange += OnRemoteRevisionsAvailabilityChange;
            m_MainModel.ConflictStatusChange += OnConflictStatusChange;

            PopulateInitialData();
        }

        /// <inheritdoc />
        public void Stop()
        {
            Assert.IsTrue(m_IsStarted, "The presenter has already been stopped.");
            m_IsStarted = false;

            m_Model.UpdatedChangeList -= OnUpdatedChangeList;
            m_Model.OnUpdatedSelectedChanges -= OnUpdatedPartiallySelectedChanges;
            m_Model.BusyStatusUpdated -= OnBusyStatusUpdated;
            m_Model.StateChanged -= OnStateChanged;
            m_MainModel.RemoteRevisionsAvailabilityChange -= OnRemoteRevisionsAvailabilityChange;
            m_MainModel.ConflictStatusChange -= OnConflictStatusChange;
        }

        /// <summary>
        /// Refresh state from the model.
        /// </summary>
        void OnStateChanged()
        {
            PopulateInitialData();
        }

        /// <summary>
        /// Populate the view with the initial data from the model.
        /// </summary>
        void PopulateInitialData()
        {
            m_View.SetRevisionSummary(m_Model.SavedRevisionSummary);
            m_View.SetSearchQuery(m_Model.SavedSearchQuery);
            m_View.SetBusyStatus(m_Model.Busy);
            m_Model.RequestInitialData();
        }

        /// <summary>
        /// Event handler to receive updated remote changes available status.
        /// </summary>
        /// <param name="available">Whether or not remote changes are available.</param>
        void OnRemoteRevisionsAvailabilityChange(bool available)
         {
             UpdatePublishButton();
         }

        /// <summary>
        /// Event handler to receive updated busy status.
        /// </summary>
        /// <param name="busy">New busy status.</param>
        void OnBusyStatusUpdated(bool busy)
        {
            m_View.SetBusyStatus(busy);
        }

        /// <summary>
        /// Event handler for when the model reports an updated change list.
        /// </summary>
        void OnUpdatedChangeList()
        {
            UpdatePublishButton();
            UpdateChangeList();
        }

        /// <summary>
        /// Request the change or conflict list depending on the state of the model. The result is then given to the
        /// view to populate itself. Fire and forget method -- must be run on main thread.
        /// </summary>
        void UpdateChangeList()
        {
            Assert.IsTrue(Threading.IsMainThread, "Updating the change lists must be done from the main thread.");

            // Fetch and send data to the UI depending on what's the current display mode.
            if (m_Model.Conflicted)
            {
                Task.Run(() => m_Model.GetConflictedEntries(m_Model.SavedSearchQuery))
                    .ContinueWith(r => m_View.SetConflicts(r.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                Task.Run(() => m_Model.GetAllEntries(m_Model.SavedSearchQuery))
                    .ContinueWith(r => m_View.SetChanges(r.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Inform view to refresh its selections.
        /// </summary>
        void OnUpdatedPartiallySelectedChanges()
        {
            m_View.SetSelectedChanges();
        }

        /// <summary>
        /// Update changelist display in response to the conflict status changing.
        /// </summary>
        /// <param name="conflicted">New conflicted status.</param>
        void OnConflictStatusChange(bool conflicted)
        {
            UpdatePublishButton();
            UpdateChangeList();
        }

        /// <inheritdoc />
        public bool UpdateEntryToggle(string path, bool toggled)
        {
            var result = m_Model.UpdateEntryToggle(path, toggled);
            m_View.SetToggledCount(ToggledCount);
            UpdatePublishButton();
            return result;
        }

        /// <inheritdoc />
        public int ToggledCount => m_Model.ToggledCount;

        /// <inheritdoc />
        public int TotalCount => m_Model.TotalCount;

        /// <inheritdoc />
        public int ConflictedCount => m_Model.ConflictedCount;

        /// <inheritdoc />
        public bool Searching => !string.IsNullOrEmpty(m_Model.SavedSearchQuery);

        /// <inheritdoc />
        public void RequestPublish()
        {
            Assert.IsFalse(Searching, "Cannot publish while searching");
            m_Model.RequestPublish(m_Model.SavedRevisionSummary, m_Model.GetToggledEntries().Select(i => i.Entry).ToList());
        }

        /// <inheritdoc />
        public void RequestDiscard(IChangeEntry entry)
        {
            if (m_View.DisplayDialogue(StringAssets.confirmDiscardChangesTitle,
                StringAssets.confirmDiscardChangeMessage, StringAssets.discardChanges,
                StringAssets.cancel))
            {
                m_Model.RequestDiscard(entry);
            }
        }

        /// <summary>
        /// Discard all toggled entries. Fire and forget method -- must be called on main thread.
        /// </summary>
        void RequestDiscardToggled()
        {
            var entries = m_Model.GetToggledEntries(m_Model.SavedSearchQuery).Select(e => e.Entry).ToList();
            if (m_View.DisplayDialogue(StringAssets.confirmDiscardChangesTitle,
                string.Format(StringAssets.confirmDiscardChangesMessage, entries.Count), StringAssets.discardChanges,
                StringAssets.cancel))
            {
                m_Model.RequestBulkDiscard(entries);
            }
        }

        /// <summary>
        /// Update the state of the publish button in the view based on the state of the model.
        /// </summary>
        void UpdatePublishButton()
        {
            if (m_Model.Conflicted)
            {
                m_View.SetPublishEnabled(false, StringAssets.cannotPublishWhileConflicted);
            }
            else if (m_MainModel.RemoteRevisionsAvailable)
            {
                m_View.SetPublishEnabled(false, StringAssets.cannotPublishWithIncomingChanges);
            }
            else if (m_Model.ToggledCount < 1)
            {
                m_View.SetPublishEnabled(false, StringAssets.cannotPublishWithoutFiles);
            }
            else if (Searching)
            {
                m_View.SetPublishEnabled(false, StringAssets.cannotPublishWhileSearching);
            }
            else
            {
                m_View.SetPublishEnabled(true);
            }
        }

        /// <inheritdoc />
        public void RequestDiffChanges(string path)
        {
            m_Model.RequestDiffChanges(path);
        }

        /// <inheritdoc />
        public void SetSearchQuery(string query)
        {
            var value = StringUtility.TrimAndToLower(query);
            m_Model.SavedSearchQuery = value;
            m_View.SetSearchQuery(query);
            UpdateChangeList();
            UpdatePublishButton();
        }

        /// <inheritdoc />
        public void SetRevisionSummary(string message)
        {
            m_View.SetRevisionSummary(message);
            m_Model.SavedRevisionSummary = message;
        }

        /// <inheritdoc />
        public int GroupOverflowEntryCount => 1;

        /// <inheritdoc />
        public void OnClickGroupOverflow(float x, float y)
        {
            new FloatingMenu()
                .AddEntry(StringAssets.menuDiscardToggledChanges, RequestDiscardToggled, ToggledCount > 0)
                .SetOpenDirection(MenuUtilities.OpenDirection.DownLeft)
                .Open(x, y);
        }

        /// <inheritdoc />
        public int ConflictGroupOverflowEntryCount => 2;

        /// <inheritdoc />
        public void OnClickConflictGroupOverflow(float x, float y)
        {
            new FloatingMenu()
                .AddEntry(StringAssets.useMyChanges, OnBulkUseMine, true)
                .AddEntry(StringAssets.useRemoteChanges, OnBulkUseRemote, true)
                .SetOpenDirection(MenuUtilities.OpenDirection.DownLeft)
                .Open(x, y);
        }

        /// <summary>
        /// Perform bulk choose mine on all conflicted entries.
        /// </summary>
        void OnBulkUseMine()
        {
            m_Model.RequestChooseMine(m_Model.GetConflictedEntries().Select(e => e.Entry.Path).ToArray());
        }

        /// <summary>
        /// Perform bulk choose theirs on all conflicted entries.
        /// </summary>
        void OnBulkUseRemote()
        {
            m_Model.RequestChooseRemote(m_Model.GetConflictedEntries().Select(e => e.Entry.Path).ToArray());
        }

        /// <inheritdoc />
        public void RequestShowConflictedDifferences(string path)
        {
            m_Model.RequestShowConflictedDifferences(path);
        }

        /// <inheritdoc />
        public void RequestChooseMerge(string path)
        {
            m_Model.RequestChooseMerge(path);
        }

        /// <inheritdoc />
        public void RequestChooseMine(string path)
        {
            m_Model.RequestChooseMine(new [] { path });
        }

        /// <inheritdoc />
        public void RequestChooseRemote(string path)
        {
            m_Model.RequestChooseRemote(new [] { path });
        }
    }
}
