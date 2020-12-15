using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Components.ChangeListEntries;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Views.Adapters.ListAdapters
{
    internal class HistoryEntryChangeListAdapter : BaseListAdapter<HistoryChangeListElement>
    {
        string m_RevisionId;
        IList<IChangeEntry> m_List;
        readonly IHistoryPresenter m_Presenter;

        public HistoryEntryChangeListAdapter([NotNull] IHistoryPresenter presenter, [NotNull] string revisionId, [NotNull] IList<IChangeEntry> list)
        {
            m_Presenter = presenter;
            m_RevisionId = revisionId;
            m_List = list;
        }

        public override int Height => UiConstants.HistoryListViewItemHeight;

        protected override HistoryChangeListElement MakeItem()
        {
            return new HistoryChangeListElement();
        }

        protected override void BindItem(HistoryChangeListElement element, int index)
        {
            element.ClearData();
            var entry = m_List[index];
            element.UpdateFilePath(entry.Path);

            // TODO: make status icon an object to handle this logic
            element.statusIcon.ClearClassList();
            element.statusIcon.AddToClassList(BaseChangeListElement.IconUssClassName);
            element.statusIcon.AddToClassList(HistoryChangeListElement.StatusIconUssClassName);
            element.statusIcon.AddToClassList(entry.StatusToString());

            if (m_Presenter.SupportsRevert)
            {
                element.revertButton.Clicked += () => m_Presenter.RequestRevert(m_RevisionId, new List<string> { entry.Path });
            }
            else
            {
                element.revertButton.AddToClassList(UiConstants.ussHidden);
            }
        }

        public override int GetEntryCount()
        {
            return m_List.Count;
        }
    }
}
