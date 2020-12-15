using Unity.Cloud.Collaborate.UserInterface;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class TestWindowCache : IWindowCache
    {
        public void Clear()
        {
            SimpleSelectedItems = default;
            RevisionSummary = default;
            ChangesSearchValue = default;
            SelectedHistoryRevision = default;
            HistoryPageNumber = default;
            TabIndex = default;
        }

        public SelectedItemsDictionary SimpleSelectedItems { get; set; }
        public string RevisionSummary { get; set; }
        public string ChangesSearchValue { get; set; }
        public string SelectedHistoryRevision { get; set; }
        public int HistoryPageNumber { get; set; }
        public int TabIndex { get; set; }
    }
}
