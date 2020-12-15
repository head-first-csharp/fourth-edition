using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Views.Adapters;
using Unity.Cloud.Collaborate.Views.Adapters.ListAdapters;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class AdapterListView : VisualElement, IAdapterObserver
    {
        public const string UssClassName = "unity-adapter-list-view";
        public const string ListViewUssClassName = UssClassName + "__list-view";

        IAdapter m_Adapter;
        readonly ListView m_ListView;

        public AdapterListView()
        {
            AddToClassList(UssClassName);
            m_ListView = new ListView();
            m_ListView.style.flexGrow = new StyleFloat(1);
            m_ListView.AddToClassList(ListViewUssClassName);
            Add(m_ListView);
        }

        /// <summary>
        /// Set the adapter for the list.
        /// </summary>
        /// <param name="adapter">Adapter for the list to use.</param>
        /// <typeparam name="T">The type of the list entries.</typeparam>
        public void SetAdapter<T>(BaseListAdapter<T> adapter) where T : VisualElement
        {
            Assert.IsNull(m_Adapter, "There cannot be more than one adapter at a time.");
            m_Adapter = adapter;
            m_Adapter.RegisterObserver(this);
            m_ListView.makeItem = m_Adapter.MakeItem;
            m_ListView.bindItem = m_Adapter.BindItem;
            m_ListView.itemHeight = m_Adapter.Height;
            NotifyDataSetChanged();
        }

        /// <summary>
        /// Remove adapter from the list.
        /// </summary>
        public void RemoveAdapter()
        {
            Assert.IsNotNull(m_Adapter, "Cannot remove a non-existent adapter.");
            m_Adapter.DeregisterObserver(this);
            m_ListView.makeItem = null;
            m_ListView.bindItem = null;
            m_ListView.itemHeight = 0;
            m_ListView.itemsSource = null;
            m_Adapter = null;
        }

        /// <summary>
        /// Set the selection type of the list.
        /// </summary>
        public SelectionType SelectionType
        {
            set => m_ListView.selectionType = value;
            get => m_ListView.selectionType;
        }

        /// <summary>
        /// Notify that the data in this list has changed.
        /// </summary>
        public void NotifyDataSetChanged()
        {
            // TODO: pagination support would be done here if it happens.
            // Feed the ListView a dummy list of the correct length.
            m_ListView.itemsSource = new bool[m_Adapter.GetEntryCount()];
        }

        public void ScrollToIndex(int idx)
        {
            m_ListView.ScrollToItem(idx);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<AdapterListView> { }
    }
}
