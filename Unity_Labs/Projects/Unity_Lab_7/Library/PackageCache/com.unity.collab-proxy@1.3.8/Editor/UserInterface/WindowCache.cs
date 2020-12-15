using System;
using System.Collections.Generic;
using Unity.Cloud.Collaborate.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cloud.Collaborate.UserInterface
{
    internal interface IWindowCache
    {
        void Clear();

        SelectedItemsDictionary SimpleSelectedItems { get; set; }

        string RevisionSummary { get; set; }

        string ChangesSearchValue { get; set; }

        string SelectedHistoryRevision { get; set; }

        int HistoryPageNumber { get; set; }

        int TabIndex { get; set; }
    }

    [Location("Cache/Window.yml", LocationAttribute.Location.LibraryFolder)]
    internal class WindowCache : ScriptableObjectSingleton<WindowCache>, IWindowCache
    {
        public event Action<IWindowCache> BeforeSerialize;

        public void Serialize()
        {
            BeforeSerialize?.Invoke(this);
            Save();
        }

        public void Clear()
        {
            SimpleSelectedItems = default;
            RevisionSummary = default;
            ChangesSearchValue = default;
            SelectedHistoryRevision = default;
            HistoryPageNumber = default;
            TabIndex = default;
        }

        SelectedItemsDictionary IWindowCache.SimpleSelectedItems
        {
            get => SimpleSelectedItems;
            set => SimpleSelectedItems = value;
        }

        string IWindowCache.RevisionSummary
        {
            get => RevisionSummary;
            set => RevisionSummary = value;
        }

        string IWindowCache.ChangesSearchValue
        {
            get => ChangesSearchValue;
            set => ChangesSearchValue = value;
        }

        string IWindowCache.SelectedHistoryRevision
        {
            get => SelectedHistoryRevision;
            set => SelectedHistoryRevision = value;
        }

        int IWindowCache.HistoryPageNumber
        {
            get => HistoryPageNumber;
            set => HistoryPageNumber = value;
        }

        int IWindowCache.TabIndex
        {
            get => TabIndex;
            set => TabIndex = value;
        }

        [SerializeField]
        public SelectedItemsDictionary SimpleSelectedItems = new SelectedItemsDictionary();

        [FormerlySerializedAs("CommitMessage")]
        [SerializeField]
        public string RevisionSummary;

        [SerializeField]
        public string ChangesSearchValue;

        [SerializeField]
        public string SelectedHistoryRevision;

        [SerializeField]
        public int HistoryPageNumber;

        [SerializeField]
        public int TabIndex;
    }

    [Serializable]
    internal class SelectedItemsDictionary : SerializableDictionary<string, bool>
    {
        public SelectedItemsDictionary() { }

        public SelectedItemsDictionary(IDictionary<string, bool> dictionary) : base(dictionary) { }
    }
}


