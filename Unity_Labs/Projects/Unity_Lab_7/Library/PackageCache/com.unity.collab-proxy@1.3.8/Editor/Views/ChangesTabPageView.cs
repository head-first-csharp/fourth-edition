using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Views.Adapters.ListAdapters;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Views
{
    [UsedImplicitly]
    internal class ChangesTabPageView : TabPageComponent, IChangesView
    {
        [CanBeNull]
        IChangesPresenter m_Presenter;

        public const string UssClassName = "changes-tab-page-view";
        public const string SearchBarUssClassName = UssClassName + "__search-bar";
        public const string EntryGroupsUssClassName = UssClassName + "__entry-groups";
        public const string PublishButtonUssClassName = UssClassName + "__publish-button";
        public const string TextFieldUssClassName = UssClassName + "__text-field";
        public const string ListViewUssClassName = UssClassName + "__list-view";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(ChangesTabPageView)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(ChangesTabPageView)}.uss";

        readonly IconTextButton m_PublishButton;
        readonly BetterTextField m_RevisionSummaryBox;
        readonly SearchBar m_SearchBar;
        readonly VisualElement m_EntryGroupsContainer;

        bool m_Active;

        [CanBeNull]
        ConflictedChangeListAdapter m_ConflictedChangeListAdapter;
        [CanBeNull]
        ToggleableChangeListAdapter m_ToggleableChangeListAdapter;

        [CanBeNull]
        ChangeEntryGroup m_EntryToggleableGroup;
        [CanBeNull]
        ChangeEntryGroup m_EntryConflictsGroup;

        [CanBeNull]
        VisualElement m_ActiveGroup;

        public ChangesTabPageView()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Get the components defined in the style / layout files.
            m_SearchBar = this.Q<SearchBar>(className: SearchBarUssClassName);
            m_RevisionSummaryBox = this.Q<BetterTextField>(className: TextFieldUssClassName);
            m_PublishButton = this.Q<IconTextButton>(className: PublishButtonUssClassName);
            m_EntryGroupsContainer = this.Q<VisualElement>(className: EntryGroupsUssClassName);

            // Initialize the text strings.
            m_PublishButton.Text = StringAssets.publishButton;
            m_RevisionSummaryBox.Placeholder = StringAssets.publishSummaryPlaceholder;
        }

        /// <inheritdoc />
        public IChangesPresenter Presenter
        {
            set
            {
                m_Presenter = value;
                SetupEvents();
                // If tab active before presenter has been added, call start once we have it.
                if (Active)
                {
                    value.Start();
                }
            }
        }

        /// <summary>
        /// Setup events to communicate with the presenter. Must be called after presenter is set.
        /// </summary>
        void SetupEvents()
        {
            Assert.IsNotNull(m_Presenter, "Invalid changes page state.");

            // Set up publish invocation.
            m_PublishButton.Clicked += m_Presenter.RequestPublish;

            // Send text values to the presenter.
            m_SearchBar.Search += m_Presenter.SetSearchQuery;
            m_RevisionSummaryBox.OnValueChangedHandler += s => m_Presenter.SetRevisionSummary(s);
        }

        /// <inheritdoc />
        public void SetBusyStatus(bool busy)
        {
            m_EntryGroupsContainer.SetEnabled(!busy);
            m_RevisionSummaryBox.SetEnabled(!busy);
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

        /// <inheritdoc />
        public void SetSearchQuery(string query)
        {
            Assert.IsNotNull(m_Presenter, "Invalid state when setting search query.");
            m_SearchBar.SetValueWithoutNotify(query);
            var isSearching = m_Presenter.Searching;
            if (m_EntryConflictsGroup != null) m_EntryConflictsGroup.Searching = isSearching;
            if (m_EntryToggleableGroup != null) m_EntryToggleableGroup.Searching = isSearching;
        }

        /// <inheritdoc />
        public void SetRevisionSummary(string message)
        {
            m_RevisionSummaryBox.SetValueWithoutNotify(message);
        }

        /// <inheritdoc />
        public void SetConflicts(IReadOnlyList<IChangeEntryData> list)
        {
            Assert.IsNotNull(m_Presenter, "Invalid state while creating conflict list.");

            // Initialise conflicts group
            if (m_EntryConflictsGroup == null)
            {
                var conflictsList = new AdapterListView { name = StringAssets.changeListConflictedList, SelectionType = SelectionType.None };
                conflictsList.AddToClassList(ListViewUssClassName);
                m_ConflictedChangeListAdapter = new ConflictedChangeListAdapter(m_Presenter);
                conflictsList.SetAdapter(m_ConflictedChangeListAdapter);
                m_EntryConflictsGroup = new ChangeEntryGroup(conflictsList) { Title = StringAssets.changeListConflictedHeader };
                m_EntryConflictsGroup.SetOverflowCallback(m_Presenter.OnClickConflictGroupOverflow);
                m_EntryConflictsGroup.Searching = m_Presenter.Searching;
            }
            Assert.IsTrue(m_ConflictedChangeListAdapter != null && m_EntryConflictsGroup != null, "Invalid state while setting conflicted list.");

            // Ensure conflict list is displayed
            if (m_ActiveGroup != m_EntryConflictsGroup)
            {
                m_ActiveGroup?.RemoveFromHierarchy();
                m_EntryGroupsContainer.Add(m_EntryConflictsGroup);
                m_ActiveGroup = m_EntryConflictsGroup;
            }

            m_ConflictedChangeListAdapter.List = list;
            var count = m_Presenter.ConflictedCount;
            m_EntryConflictsGroup.NumberMenuItems = m_Presenter.ConflictGroupOverflowEntryCount;
            m_EntryConflictsGroup.SelectedEntryCount = count;
            m_EntryConflictsGroup.EntryCount = count;
        }

        /// <inheritdoc />
        public void SetSelectedChanges()
        {
            Assert.IsNotNull(m_Presenter, "Invalid state while setting selected items from toggleable list.");
            if(m_ToggleableChangeListAdapter == null)
            {
                // we might be Selecting partial changes before the view loads the first time,
                // so we just ignore it ....
                return;
            }
            
            Assert.IsTrue(m_ToggleableChangeListAdapter != null && m_EntryToggleableGroup != null, "Invalid state while setting selected items in toggleable list");
            var scrollToIndex = m_ToggleableChangeListAdapter.GetFirstToggledIndex();
            m_ToggleableChangeListAdapter.NotifyDataSetChanged();
            if (scrollToIndex != -1)
            {
                scrollToIndex = Math.Min(scrollToIndex, m_ToggleableChangeListAdapter.GetEntryCount() - 1);

                m_EntryToggleableGroup.ScrollTo(scrollToIndex);
                if(m_ToggleableChangeListAdapter.GetLastBoundElementIndex() < scrollToIndex + 3)
                {
                    // the pool of the list is 14 elements .. but the list actually shows only 12 ..
                    // so the normal scrollTo call of the list view may stop 1 element short of the selected
                    // index if the scrolled to index is greater than the currently selected index.
                    m_EntryToggleableGroup.ScrollTo(scrollToIndex + 3);
                }
            }
        }

        /// <inheritdoc />
        public void SetChanges(IReadOnlyList<IChangeEntryData> list)
        {
            Assert.IsNotNull(m_Presenter, "Invalid state while creating toggleable list.");

            // Initialise the toggleable list if not already initialised.
            if (m_EntryToggleableGroup == null)
            {
                var toggleableListView = new AdapterListView { SelectionType = SelectionType.None };
                toggleableListView.AddToClassList(ListViewUssClassName);
                m_ToggleableChangeListAdapter = new ToggleableChangeListAdapter(m_Presenter);
                toggleableListView.SetAdapter(m_ToggleableChangeListAdapter);
                m_EntryToggleableGroup = new ChangeEntryGroup(toggleableListView)
                    { Title = StringAssets.changeListFullHeader };
                m_EntryToggleableGroup.SetOverflowCallback(m_Presenter.OnClickGroupOverflow);
                m_EntryToggleableGroup.Searching = m_Presenter.Searching;
            }
            Assert.IsTrue(m_ToggleableChangeListAdapter != null && m_EntryToggleableGroup != null, "Invalid state while setting toggleable list");

            // Ensure single list is displayed
            if (m_ActiveGroup != m_EntryToggleableGroup)
            {
                m_ActiveGroup?.RemoveFromHierarchy();
                m_EntryGroupsContainer.Add(m_EntryToggleableGroup);
                m_ActiveGroup = m_EntryToggleableGroup;
            }

            // Can use list.Count here since searching hides "All".
            m_EntryToggleableGroup.EntryCount = m_Presenter.Searching ? list.Count : m_Presenter.TotalCount;
            m_ToggleableChangeListAdapter.List = list;
            m_EntryToggleableGroup.NumberMenuItems = m_Presenter.GroupOverflowEntryCount;
            m_EntryToggleableGroup.SelectedEntryCount = m_Presenter.ToggledCount;
        }

        /// <inheritdoc />
        public void SetToggledCount(int count)
        {
            if (m_EntryToggleableGroup != null)
            {
                m_EntryToggleableGroup.SelectedEntryCount = count;
            }
        }

        /// <inheritdoc />
        public void SetPublishEnabled(bool enabled, string reason = null)
        {
            m_PublishButton.SetEnabled(enabled);

            // Disabled elements cannot have a tooltip so apply to a empty/dummy parent instead.
            m_PublishButton.parent.tooltip = reason;
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

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ChangesTabPageView> { }
    }
}
