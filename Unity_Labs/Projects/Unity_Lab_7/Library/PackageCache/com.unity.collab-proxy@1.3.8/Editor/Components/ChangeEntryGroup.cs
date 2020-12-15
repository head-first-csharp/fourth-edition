using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class ChangeEntryGroup : VisualElement
    {
        readonly AdapterListView m_ListView;
        readonly ChangesGroupHeader m_GroupHeader;
        readonly ListNotice m_ListNotice;

        [NotNull]
        string m_Title = string.Empty;
        bool m_Searching;
        int m_SelectedEntryCount;
        int m_EntryCount;

        public ChangeEntryGroup([NotNull] AdapterListView adapterListView)
        {
            m_ListView = adapterListView;
            m_GroupHeader = new ChangesGroupHeader();
            m_ListNotice = new ListNotice();
            m_ListNotice.AddToClassList(UiConstants.ussHidden);

            Add(m_GroupHeader);
            Add(m_ListView);
            Add(m_ListNotice);
        }

        [NotNull]
        public string Title
        {
            set
            {
                m_Title = value;
                UpdateTitle();
            }
            get => m_Title;
        }

        public bool Searching
        {
            set
            {
                m_Searching = value;
                UpdateListNoticeText();
            }
            get => m_Searching;
        }

        public int NumberMenuItems
        {
            set => m_GroupHeader.SetEnableOverflowMenu(value != 0);
        }

        public int EntryCount
        {
            set
            {
                m_EntryCount = value;
                UpdateListNotice();
            }
            get => m_EntryCount;
        }

        public int SelectedEntryCount
        {
            set
            {
                m_SelectedEntryCount = value;
                UpdateTitle();
            }
            get => m_SelectedEntryCount;
        }

        void UpdateListNoticeText()
        {
            m_ListNotice.Text = Searching ? StringAssets.noticeNoResultsForQuery : StringAssets.noticeNoChangesToDisplay;
        }

        void UpdateTitle()
        {
            m_GroupHeader.UpdateGroupName(Searching
                ? StringAssets.searchResults
                : string.Format(StringAssets.changeGroupHeaderFormat, Title, SelectedEntryCount));
        }

        void UpdateListNotice()
        {
            if (m_EntryCount != 0)
            {
                m_ListNotice.AddToClassList(UiConstants.ussHidden);
                m_ListView.RemoveFromClassList(UiConstants.ussHidden);
            }
            else
            {
                m_ListNotice.RemoveFromClassList(UiConstants.ussHidden);
                m_ListView.AddToClassList(UiConstants.ussHidden);
            }
        }

        public void SetOverflowCallback(Action<float, float> callback)
        {
            m_GroupHeader.OnOverflowButtonClicked += callback;
        }

        public void ScrollTo(int idx)
        {
            m_ListView.ScrollToIndex(idx);
        }
    }
}
