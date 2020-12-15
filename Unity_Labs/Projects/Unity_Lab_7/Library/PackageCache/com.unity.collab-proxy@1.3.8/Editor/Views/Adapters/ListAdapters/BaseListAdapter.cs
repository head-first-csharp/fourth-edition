using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Views.Adapters.ListAdapters
{
    /// <summary>
    /// Adapter used to provide entries to the AdapterListView. Allows the data to be kept separately to the layout
    /// and visual elements.
    /// </summary>
    /// <typeparam name="T">Type of list element the adapter provides</typeparam>
    internal abstract class BaseListAdapter<T> : IAdapter where T : VisualElement
    {
        readonly List<IAdapterObserver> m_AdapterObservers = new List<IAdapterObserver>();

        #region PrivateInterfaceFields

        Func<VisualElement> IAdapter.MakeItem => MakeItem;

        Action<VisualElement, int> IAdapter.BindItem => (v, i) => BindItem((T)v, i);

        #endregion

        #region UserOverrides

        /// <summary>
        /// Provides the static height for each element.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// Creates and returns the layout for the entry.
        /// </summary>
        /// <returns>Created entry layout.</returns>
        protected abstract T MakeItem();

        /// <summary>
        /// Binds data to the entry at the given index.
        /// </summary>
        /// <param name="element">Entry to bind to.</param>
        /// <param name="index">Index in the data.</param>
        protected abstract void BindItem(T element, int index);

        /// <summary>
        /// Gets the count of the number of entries in the list.
        /// </summary>
        /// <returns>The entry count.</returns>
        public abstract int GetEntryCount();

        #endregion

        /// <summary>
        /// Register an observer for this adapter.
        /// </summary>
        /// <param name="observer">Observer to register.</param>
        public void RegisterObserver(IAdapterObserver observer)
        {
            m_AdapterObservers.Add(observer);
        }

        /// <summary>
        /// Deregister an observer for this adapter.
        /// </summary>
        /// <param name="observer">Observer to deregister.</param>
        public void DeregisterObserver(IAdapterObserver observer)
        {
            m_AdapterObservers.Remove(observer);
        }

        /// <summary>
        /// Notify that the data set in this adapter has changed.
        /// </summary>
        public void NotifyDataSetChanged()
        {
            foreach (var observer in m_AdapterObservers)
            {
                observer.NotifyDataSetChanged();
            }
        }
    }
}
