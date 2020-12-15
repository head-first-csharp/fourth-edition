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
    internal class ConflictedChangeListAdapter : BaseListAdapter<ConflictedChangeListElement>
    {
        IChangesPresenter m_Presenter;

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

        public ConflictedChangeListAdapter([NotNull] IChangesPresenter presenter)
        {
            m_Presenter = presenter;
        }

        public override int Height { get; } = UiConstants.ChangesListViewItemHeight;

        protected override ConflictedChangeListElement MakeItem()
        {
            return new ConflictedChangeListElement();
        }

        protected override void BindItem(ConflictedChangeListElement element, int index)
        {
            Assert.IsNotNull(m_List, "List should not be null at this point.");
            element.ClearData();
            var changesEntry = m_List[index];
            var path = changesEntry.All ? StringAssets.all : changesEntry.Entry.Path;
            element.UpdateFilePath(path);

            // Update status icon
            element.statusIcon.ClearClassList();
            element.statusIcon.AddToClassList(BaseChangeListElement.IconUssClassName);
            element.statusIcon.AddToClassList(ToggleableChangeListElement.StatusIconUssClassName);
            element.statusIcon.AddToClassList(changesEntry.Entry.StatusToString());

            // Wire up buttons
            element.showButton.Clicked += () => m_Presenter.RequestShowConflictedDifferences(changesEntry.Entry.Path);
            element.chooseMergeButton.Clicked += () => m_Presenter.RequestChooseMerge(changesEntry.Entry.Path);
            element.chooseMineButton.Clicked += () => m_Presenter.RequestChooseMine(changesEntry.Entry.Path);
            element.chooseRemoteButton.Clicked += () => m_Presenter.RequestChooseRemote(changesEntry.Entry.Path);
        }

        public override int GetEntryCount()
        {
            return m_List?.Count ?? 0;
        }
    }
}
