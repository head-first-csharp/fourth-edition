using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Components.ChangeListEntries;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Views.Adapters.ListAdapters
{
    internal class ToggleableChangeListAdapter : BaseListAdapter<ToggleableChangeListElement>
    {
        [CanBeNull]
        IReadOnlyList<IChangeEntryData> m_List;
        public IReadOnlyList<IChangeEntryData> List
        {
            set
            {
                m_List = value;
                NotifyDataSetChanged();
            }
        }

        readonly IChangesPresenter m_Presenter;
        int m_LastBoundElementIndex;

        public ToggleableChangeListAdapter(IChangesPresenter presenter)
        {
            m_Presenter = presenter;
        }

        public override int Height { get; } = UiConstants.ChangesListViewItemHeight;

        protected override ToggleableChangeListElement MakeItem()
        {
            return new ToggleableChangeListElement();
        }

        protected override void BindItem(ToggleableChangeListElement element, int index)
        {
            Assert.IsNotNull(m_List, "List should not be null at this point.");
            m_LastBoundElementIndex = index;
            element.ClearData();
            var changesEntry = m_List[index];
            var path = changesEntry.All ? StringAssets.all : changesEntry.Entry.Path;
            element.UpdateFilePath(path);

            // Setup callbacks
            element.SetToggleCallback(c => OnItemToggleChanged(index, c));
            element.diffButton.Clicked += () => OnDiffClicked(index);
            element.discardButton.RemoveFromClassList(UiConstants.ussHidden);
            element.discardButton.Clicked += () => OnDiscardClicked(index);

            // Update the toggle and tooltips.
            if (changesEntry.ToggleReadOnly)
            {
                element.toggle.SetValueWithoutNotify(true);
                element.toggle.SetEnabled(false);
                element.toggle.parent.tooltip = StringAssets.includedToPublishByAnotherGitTool;
            }
            else
            {
                element.toggle.SetValueWithoutNotify(changesEntry.Toggled);
                element.toggle.SetEnabled(true);
                element.toggle.parent.tooltip = string.Empty;
            }

            // Update the visibility of the icon and discard button.
            if (changesEntry.All)
            {
                element.buttons.AddToClassList(UiConstants.ussHidden);
                element.statusIcon.AddToClassList(UiConstants.ussHidden);
            }
            else
            {
                element.buttons.RemoveFromClassList(UiConstants.ussHidden);
                // TODO: make status icon an object to handle this logic
                element.statusIcon.ClearClassList();
                element.statusIcon.AddToClassList(BaseChangeListElement.IconUssClassName);
                element.statusIcon.AddToClassList(ToggleableChangeListElement.StatusIconUssClassName);
                element.statusIcon.AddToClassList(changesEntry.Entry.StatusToString());
            }
        }

        public override int GetEntryCount()
        {
            return m_List?.Count ?? 0;
        }

        void OnItemToggleChanged(int index, bool toggled)
        {
            Assert.IsNotNull(m_List, "List should not be null at this point.");
            var changeEntry = m_List[index];
            var refresh = m_Presenter.UpdateEntryToggle(changeEntry.Entry.Path, toggled);
            if (refresh) NotifyDataSetChanged();
        }

        void OnDiscardClicked(int index)
        {
            Assert.IsNotNull(m_List, "List should not be null at this point.");
            var changeEntry = m_List[index];
            m_Presenter.RequestDiscard(changeEntry.Entry);
        }

        public int GetLastBoundElementIndex()
        {
            return m_LastBoundElementIndex;
        }

        public int GetFirstToggledIndex()
        {
            Assert.IsNotNull(m_List, "List should not be null at this point.");
            for (var i=0; i < m_List.Count; i++)
            {
                if(m_List[i].Toggled)
                {
                    return i;
                }
            }

            return -1;
        }

        void OnDiffClicked(int index)
        {
            Assert.IsNotNull(m_List, "List should not be null at this point.");
            var changeEntry = m_List[index];
            m_Presenter.RequestDiffChanges(changeEntry.Entry.Path);
        }
    }
}
