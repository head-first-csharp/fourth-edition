using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.UserInterface;
using Unity.Cloud.Collaborate.Utilities;
using Unity.Cloud.Collaborate.Views.Adapters.ListAdapters;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Views
{
    internal class HistoryTabPageView : TabPageComponent, IHistoryView
    {
        [CanBeNull]
        IHistoryPresenter m_Presenter;

        public const string UssClassName = "history-page";
        public const string PaginatorUssClassName = UssClassName + "__paginator";
        public const string ContentUssClassName = UssClassName + "__content";
        public const string NoticeUssClassName = UssClassName + "__notice";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(HistoryTabPageView)}.uxml";

        readonly ScrollView m_Content;
        readonly ListNotice m_ListNotice;
        readonly Paginator m_Paginator;

        bool m_Active;

        public HistoryTabPageView()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);

            m_Paginator = this.Q<Paginator>(className: PaginatorUssClassName);
            m_Paginator.AddToClassList(UiConstants.ussHidden);
            m_Paginator.ClickedMovePage += OnClickedMovePage;

            m_Content = this.Q<ScrollView>(className: ContentUssClassName);

            // Add loading notice
            m_ListNotice = this.Q<ListNotice>(className: NoticeUssClassName);
            m_ListNotice.Text = StringAssets.loadingRevisions;
        }

        /// <inheritdoc />
        public IHistoryPresenter Presenter
        {
            set
            {
                m_Presenter = value;
                // If tab active before presenter has been added, call start once we have it.
                if (Active)
                {
                    m_Presenter.Start();
                }
            }
        }

        /// <inheritdoc />
        public void SetBusyStatus(bool busy)
        {
            m_Paginator.SetEnabled(!busy);
            m_Content.SetEnabled(!busy);
        }

        /// <inheritdoc />
        public void SetHistoryList(IReadOnlyList<IHistoryEntry> list)
        {
            // Clear out old content
            m_ListNotice.AddToClassList(UiConstants.ussHidden);
            m_Content.Clear();

            // Show paginator
            m_Paginator.RemoveFromClassList(UiConstants.ussHidden);

            // Handle empty list case
            if (list.Count == 0)
            {
                m_ListNotice.Text = StringAssets.noticeNoRevisionsToDisplay;
                m_Content.RemoveFromClassList(UiConstants.ussHidden);
                return;
            }

            foreach (var entry in list)
            {
                // Add entry to the list
                m_Content.Add(CreateHistoryEntry(entry, false));
            }
        }

        /// <summary>
        /// Event handler for receiving page change requests.
        /// </summary>
        /// <param name="pageChange">
        /// Delta to change the page by: Paginator.MoveForwards, Paginator.MoveBackwards. Mapped to +1, -1 respectively.
        /// </param>
        void OnClickedMovePage(int pageChange)
        {
            Assert.IsNotNull(m_Presenter, "Invalid state when requesting page change.");
            if (pageChange == Paginator.MoveBackwards)
            {
                m_Presenter.PrevPage();
            }
            else
            {
                m_Presenter.NextPage();
            }
        }

        /// <inheritdoc />
        public void SetPage(int page, int max)
        {
            m_Paginator.SetPage(page, max);
        }

        /// <inheritdoc />
        public void SetSelection(IHistoryEntry entry)
        {
            // Hide paginator
            m_Paginator.AddToClassList(UiConstants.ussHidden);

            // Clear out old content
            m_ListNotice.AddToClassList(UiConstants.ussHidden);
            m_Content.Clear();

            // Add new content
            m_Content.Add(CreateHistoryEntry(entry, true));
        }

        /// <summary>
        /// Takes a IHistoryEntry and binds it to a created HistoryEntryComponent to be used in the history list.
        /// </summary>
        /// <param name="entry">History entry to bind</param>
        /// <param name="expanded">Whether or not to show its list of changed entries.</param>
        /// <returns>Inflated and bound component.</returns>
        HistoryEntryComponent CreateHistoryEntry([NotNull] IHistoryEntry entry, bool expanded)
        {
            Assert.IsNotNull(m_Presenter, "Invalid state when creating history entry");
            var comp = new HistoryEntryComponent();

            // Handle expanded vs compact layout
            if (expanded)
            {
                // Hide fields used for compact view
                comp.showFilesButton.AddToClassList(UiConstants.ussHidden);
                comp.cloudStatusText.AddToClassList(UiConstants.ussHidden);

                comp.changedFilesCount.text = $"Changes ( {entry.Changes.Count} )";

                var listAdapter = new HistoryEntryChangeListAdapter(m_Presenter, entry.RevisionId, entry.Changes.ToList());
                comp.changedFiles.SetAdapter(listAdapter);
                listAdapter.NotifyDataSetChanged();

                // Configure button
                comp.gotoButton.text = entry.GetGotoText();
                comp.gotoButton.clickable.clicked += () => m_Presenter.RequestGoto(entry.RevisionId, entry.Status);
            }
            else
            {
                // Hide fields used for expanded view
                comp.changedFilesCount.AddToClassList(UiConstants.ussHidden);
                comp.changedFiles.AddToClassList(UiConstants.ussHidden);
                comp.gotoButton.text = string.Empty;
                comp.gotoButton.AddToClassList(UiConstants.ussHidden);

                // Setup show button
                comp.showFilesButton.text = entry.Changes.Count == 1
                    ? StringAssets.showChange
                    : string.Format(StringAssets.showChanges, entry.Changes.Count);
                comp.showFilesButton.clickable.clicked += () => m_Presenter.SelectedRevisionId = entry.RevisionId;

                // TODO: cloud status text
            }

            // Trim whitespace on either side and grab initial for profile circle
            var trimmedAuthorName = entry.AuthorName.Trim();
            comp.profileInitial.text = trimmedAuthorName.Substring(0, 1).ToUpper();
            comp.authorName.text = trimmedAuthorName;

            // Display relative or absolute timestamp. If relative, show absolute as a tooltip.
            comp.timestamp.text = TimeStamp.GetTimeStamp(entry.Time);
            if (TimeStamp.UseRelativeTimeStamps)
            {
                comp.timestamp.tooltip = TimeStamp.GetLocalisedTimeStamp(entry.Time);
            }

            // Display revision id and show full length id as a tooltip
            comp.revisionId.text = $"ID: {entry.RevisionId.Substring(0, 10)}";
            comp.revisionId.tooltip = entry.RevisionId;

            comp.commitMessage.text = entry.Message;

            return comp;
        }

        /// <inheritdoc />
        public bool DisplayDialogue(string title, string message, string affirmative)
        {
            return EditorUtility.DisplayDialog(title, message, affirmative);
        }

        /// <inheritdoc />
        public bool DisplayDialogue(string title, string message, string affirmative, string negative)
        {
            return EditorUtility.DisplayDialog(title, message, affirmative, negative);
        }

        /// <inheritdoc />
        protected override void SetActive()
        {
            Assert.IsFalse(m_Active, "The view is already active.");
            m_Active = true;

            m_Presenter?.Start();
        }

        /// <inheritdoc />
        protected override void SetInactive()
        {
            Assert.IsTrue(m_Active, "The view is already inactive.");
            m_Active = false;

            m_Presenter?.Stop();
        }
    }
}
