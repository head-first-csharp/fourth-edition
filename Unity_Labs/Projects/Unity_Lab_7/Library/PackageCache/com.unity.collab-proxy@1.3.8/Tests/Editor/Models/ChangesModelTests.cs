using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Structures;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    public class ChangesModelTests
    {
        class TestableChangesModel : ChangesModel
        {
            public TestSourceControlProvider Provider => (TestSourceControlProvider)m_Provider;

            public TestableChangesModel() : base (new TestSourceControlProvider())
            {

            }

            public void SetToggled([CanBeNull] Dictionary<string, bool> toggled = null)
            {
                if (toggled != null)
                {
                    toggledEntries = toggled;
                }
            }

            internal override void UpdateChangeList(IReadOnlyList<IChangeEntry> list)
            {
                base.UpdateChangeList(list);
                ValidateData();
            }

            public override bool UpdateEntryToggle(string path, bool value)
            {
                var refresh = base.UpdateEntryToggle(path, value);
                ValidateData();
                return refresh;
            }

            void ValidateData()
            {
                var toggledCount = 0;
                foreach (var x in entryData.Select(entry => entry.Value))
                {
                    Assert.IsTrue(toggledEntries.TryGetValue(x.Entry.Path, out var toggled) && x.Toggled == toggled);
                    if (!x.All && toggled) toggledCount++;
                }
                Assert.AreEqual(toggledCount, ToggledCount);
            }
        }

        [Test]
        public void ChangesModel_NullSourceControlEntries_EmptyResultLists()
        {
            var model = new TestableChangesModel();
            model.OnStart();
            model.UpdateChangeList(new List<IChangeEntry>());

            var fullList = model.GetAllEntries();
            Assert.AreEqual(1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            Assert.AreEqual(0, model.GetToggledEntries().Count);
            Assert.AreEqual(0, model.GetUntoggledEntries().Count);

            Assert.AreEqual(0, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_EmptySourceControlEntries_EmptyResultLists()
        {
            var model = new TestableChangesModel();
            model.OnStart();
            model.UpdateChangeList(new List<IChangeEntry>());

            var fullList = model.GetAllEntries();
            Assert.AreEqual(1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            Assert.AreEqual(0, model.GetToggledEntries().Count);
            Assert.AreEqual(0, model.GetUntoggledEntries().Count);

            Assert.AreEqual(0, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_SingleSourceControlEntries_SingleUntoggledResult()
        {
            var model = new TestableChangesModel();
            model.OnStart();
            var changes = BuildChangesList(1);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(2, fullList.Count);
            Assert.IsTrue(fullList[0].All);
            Assert.IsFalse(fullList[0].Toggled);
            Assert.IsFalse(fullList[1].All);
            Assert.IsFalse(fullList[1].Toggled);

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(0, toggledList.Count);

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(1, untoggledList.Count);
            Assert.IsFalse(untoggledList[0].All);
            Assert.IsFalse(untoggledList[0].Toggled);

            Assert.AreEqual(0, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_MultipleSourceControlEntries_ToggleSingle()
        {
            const int entryCount = 5;

            var model = new TestableChangesModel();
            model.OnStart();
            var changes = BuildChangesList(entryCount);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount, model.TotalCount);
            Assert.AreEqual(entryCount + 1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            var toggledEntry = fullList[entryCount / 2 + 1];
            model.UpdateEntryToggle(toggledEntry.Entry.Path, true);
            Assert.IsTrue(toggledEntry.Toggled);

            fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            foreach (var entry in fullList)
            {
                if (entry == fullList[0])
                {
                    Assert.IsTrue(entry.All);
                }
                else
                {
                    Assert.IsFalse(entry.All);
                }

                if (entry == toggledEntry)
                {
                    Assert.IsTrue(entry.Toggled);
                }
                else
                {
                    Assert.IsFalse(entry.Toggled);
                }
            }

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(1, toggledList.Count);
            Assert.AreEqual(toggledEntry, toggledList[0]);

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(entryCount -1, untoggledList.Count);
            foreach (var entry in untoggledList)
            {
                Assert.IsFalse(entry.All);
                Assert.AreNotEqual(toggledEntry, entry);
            }

            Assert.AreEqual(1, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_MultipleSourceControlEntries_ToggleAll()
        {
            const int entryCount = 5;

            var model = new TestableChangesModel();
            model.OnStart();
            var changes = BuildChangesList(entryCount);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            model.UpdateEntryToggle(fullList[0].Entry.Path, true);

            fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            foreach (var entry in fullList)
            {
                if (entry == fullList[0])
                {
                    Assert.IsTrue(entry.All);
                }
                else
                {
                    Assert.IsFalse(entry.All);
                }
                Assert.IsTrue(entry.Toggled);
            }

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(entryCount, toggledList.Count);
            foreach (var entry in toggledList)
            {
                Assert.IsFalse(entry.All);
            }

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(0, untoggledList.Count);

            Assert.AreEqual(entryCount, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_MultipleSourceControlEntries_ToggleAllIndividually()
        {
            const int entryCount = 5;

            var model = new TestableChangesModel();
            model.OnStart();
            var changes = BuildChangesList(entryCount);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            fullList = model.GetAllEntries();
            foreach (var entry in fullList.Where(entry => !entry.All))
            {
                model.UpdateEntryToggle(entry.Entry.Path, true);
            }

            Assert.AreEqual(entryCount + 1, fullList.Count);
            foreach (var entry in fullList)
            {
                if (entry == fullList[0])
                {
                    Assert.IsTrue(entry.All);
                }
                else
                {
                    Assert.IsFalse(entry.All);
                }
                Assert.IsTrue(entry.Toggled);
            }

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(entryCount, toggledList.Count);
            foreach (var entry in toggledList)
            {
                Assert.IsFalse(entry.All);
            }

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(0, untoggledList.Count);

            Assert.AreEqual(entryCount, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_MultipleSourceControlEntries_UntoggleSingleFromAll()
        {
            const int entryCount = 5;

            var model = new TestableChangesModel();
            model.OnStart();
            var changes = BuildChangesList(entryCount);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            model.UpdateEntryToggle(fullList[0].Entry.Path, true);
            var untoggledEntry = fullList[entryCount / 2 + 1];
            model.UpdateEntryToggle(untoggledEntry.Entry.Path, false);
            Assert.IsFalse(untoggledEntry.Toggled);

            fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            foreach (var entry in fullList)
            {
                if (entry == fullList[0])
                {
                    Assert.IsTrue(entry.All);
                }
                else
                {
                    Assert.IsFalse(entry.All);
                }

                if (entry == untoggledEntry || entry.All)
                {
                    Assert.IsFalse(entry.Toggled);
                }
                else
                {
                    Assert.IsTrue(entry.Toggled);
                }
            }

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(entryCount - 1, toggledList.Count);
            foreach (var entry in toggledList)
            {
                Assert.IsFalse(entry.All);
                Assert.AreNotEqual(untoggledEntry, entry);
            }

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(1, untoggledList.Count);
            Assert.AreEqual(untoggledEntry, untoggledList[0]);

            Assert.AreEqual(entryCount - 1, model.ToggledCount);
        }

        [Test]
        public void ChangesModel_MultipleSourceControlEntries_SomeConflicted()
        {
            const string conflictedPrefix = "conflicted-path";

            var model = new TestableChangesModel();
            model.OnStart();
            var changes = new List<IChangeEntry>();
            AddEntry(changes, "path1", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "path2", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "path3", ChangeEntryStatus.Modified, false);
            AddEntry(changes, $"{conflictedPrefix}4", ChangeEntryStatus.Modified, false, true);
            AddEntry(changes, $"{conflictedPrefix}5", ChangeEntryStatus.Modified, false, true);
            model.UpdateChangeList(changes);

            var conflictedList = model.GetConflictedEntries();
            model.Provider.ConflictedState = true;
            Assert.IsTrue(model.Conflicted);
            Assert.AreEqual(2, model.ConflictedCount);
            Assert.AreEqual(2, conflictedList.Count);
            Assert.IsFalse(conflictedList[0].All);
            Assert.IsFalse(conflictedList[1].All);
            Assert.IsTrue(conflictedList[0].Conflicted);
            Assert.IsTrue(conflictedList[1].Conflicted);

            Assert.IsTrue(conflictedList[0].Entry.Path.StartsWith(conflictedPrefix));
            Assert.IsTrue(conflictedList[1].Entry.Path.StartsWith(conflictedPrefix));
        }

        [Test]
        public void ChangesModel_InitializeFromDictionary_TransfersToggledFlag()
        {
            const int entryCount = 5;
            const int toggledCount = 2;
            const int toggledIndex1 = 0;
            const int toggledIndex2 = entryCount / 2 + 1;
            const int untoggledIndex = entryCount - 1;

            var changes = BuildChangesList(entryCount);
            var dictionary = new Dictionary<string, bool>();
            changes.ForEach( (x) => dictionary[x.Path] = false );
            dictionary[changes[toggledIndex1].Path] = true;
            dictionary[changes[toggledIndex2].Path] = true;
            dictionary[changes[untoggledIndex].Path] = false;

            var model = new TestableChangesModel();
            model.OnStart();
            model.SetToggled(dictionary);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(entryCount + 1, fullList.Count);
            foreach (var entry in fullList)
            {
                if (entry == fullList[0])
                {
                    Assert.IsTrue(entry.All);
                }
                else
                {
                    Assert.IsFalse(entry.All);
                }

                if (entry.Entry.Path == changes[toggledIndex1].Path || entry.Entry.Path == changes[toggledIndex2].Path)
                {
                    Assert.IsTrue(entry.Toggled);
                }
                else
                {
                    Assert.IsFalse(entry.Toggled);
                }
            }

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(toggledCount, toggledList.Count);
            foreach (var entry in toggledList)
            {
                Assert.IsTrue(entry.Entry.Path == changes[toggledIndex1].Path || entry.Entry.Path == changes[toggledIndex2].Path);
                Assert.IsFalse(entry.All);
                Assert.IsTrue(entry.Toggled);
            }

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(entryCount - toggledCount, untoggledList.Count);
            foreach (var entry in untoggledList)
            {
                Assert.IsTrue(entry.Entry.Path != changes[toggledIndex1].Path && entry.Entry.Path != changes[toggledIndex2].Path);
                Assert.IsFalse(entry.All);
                Assert.IsFalse(entry.Toggled);
            }
        }

        [Test]
        public void ChangesModel_SearchFilters_CaseInsensitive()
        {
            var changes = new List<IChangeEntry>();
            AddEntry(changes, "alpha1", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "alpha2", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "bravo", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "charlie", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "Delta3", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "delta4", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "delta5", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "echo", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "Foxtrot6", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "Foxtrot7", ChangeEntryStatus.Modified, false);
            AddEntry(changes, "golf", ChangeEntryStatus.Modified, false);

            var dictionary = new Dictionary<string, bool>
            {
                ["delta5"] = true, ["Foxtrot6"] = true, ["Foxtrot7"] = true, ["golf"] = true
            };

            var model = new TestableChangesModel();
            model.OnStart();
            model.SetToggled(dictionary);
            model.UpdateChangeList(changes);

            var fullList = model.GetAllEntries();
            Assert.AreEqual(changes.Count, model.TotalCount);
            Assert.AreEqual(changes.Count + 1, fullList.Count);
            Assert.IsTrue(fullList[0].All);

            var searchFullList = model.GetAllEntries("alpha");
            Assert.AreEqual(2, searchFullList.Count);
            foreach (var entry in searchFullList)
            {
                Assert.IsFalse(entry.All);
                Assert.IsFalse(entry.Toggled);
            }

            var toggledList = model.GetToggledEntries();
            Assert.AreEqual(dictionary.Count, toggledList.Count);
            foreach (var entry in toggledList)
            {
                Assert.IsFalse(entry.All);
            }

            var searchToggledList = model.GetToggledEntries("fox");
            Assert.AreEqual(2, searchToggledList.Count);
            foreach (var entry in searchToggledList)
            {
                Assert.IsTrue(entry.Entry.Path.ToLower().Contains("fox"));
                Assert.IsFalse(entry.All);
                Assert.IsTrue(entry.Toggled);
            }

            var untoggledList = model.GetUntoggledEntries();
            Assert.AreEqual(changes.Count - dictionary.Count, untoggledList.Count);

            var searchUntoggledList = model.GetUntoggledEntries("Del");
            Assert.AreEqual(2, searchUntoggledList.Count);
            foreach (var entry in searchUntoggledList)
            {
                Assert.IsTrue(entry.Entry.Path.ToLower().Contains("del"));
                Assert.AreNotEqual("delta5", entry.Entry.Path);
                Assert.IsFalse(entry.All);
                Assert.IsFalse(entry.Toggled);
            }

            Assert.AreEqual(dictionary.Count, model.ToggledCount);
        }

        [Test]
        public void TestRequestInitialData()
        {
            var provider = new TestSourceControlProvider();
            var model = new ChangesModel(provider);
            model.OnStart();

            var callCount = 0;
            bool? callValue = null;
            model.BusyStatusUpdated += b =>
            {
                callCount++;
                callValue = b;
            };

            Assert.IsFalse(model.Busy);
            model.RequestInitialData();
            Assert.AreEqual(1, provider.RequestedChangeListCount);
            Assert.IsNotNull(provider.RequestedChangeListCallback);
            Assert.IsTrue(model.Busy);
            Assert.IsTrue(callValue);
            provider.RequestedChangeListCallback.Invoke(new List<IChangeEntry>());
            Assert.IsFalse(model.Busy);
            Assert.IsFalse(callValue);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void TestReceiveUpdatedChangeListEvent()
        {
            var provider = new TestSourceControlProvider();
            var model = new ChangesModel(provider);
            model.OnStart();

            var callCount = 0;
            bool? callValue = null;
            model.BusyStatusUpdated += b =>
            {
                callCount++;
                callValue = b;
            };

            Assert.IsFalse(model.Busy);
            provider.TriggerUpdatedChangeEntries();
            Assert.AreEqual(1, provider.RequestedChangeListCount);
            Assert.IsNotNull(provider.RequestedChangeListCallback);
            Assert.IsTrue(model.Busy);
            Assert.IsTrue(callValue);
            provider.RequestedChangeListCallback.Invoke(new List<IChangeEntry>());
            Assert.IsFalse(model.Busy);
            Assert.IsFalse(callValue);
            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void TestRequestDiff()
        {
            var provider = new TestSourceControlProvider();
            var model = new ChangesModel(provider);
            model.OnStart();

            const string path = "path";
            model.RequestDiffChanges(path);
            Assert.AreEqual(1, provider.RequestedDiffChangesCount);
            Assert.AreEqual(path, provider.RequestedDiffChangesPath);
        }

        [Test]
        public void TestRequestDiscard()
        {
            var provider = new TestSourceControlProvider();
            var model = new ChangesModel(provider);
            model.OnStart();

            const string path = "path";
            var entry = new ChangeEntry(path);
            model.RequestDiscard(entry);
            Assert.AreEqual(1, provider.RequestedDiscardCount);
            Assert.AreEqual(path, provider.RequestedDiscardEntry?.Path);
        }

        [Test]
        public void TestRequestPublish()
        {
            var provider = new TestSourceControlProvider();
            var model = new ChangesModel(provider);
            model.OnStart();

            const string message = "message";
            model.RequestPublish(message, new List<IChangeEntry> { new ChangeEntry("path1"), new ChangeEntry("path2")});
            Assert.AreEqual(1, provider.RequestedPublishCount);
            Assert.AreEqual(message, provider.RequestedPublishMessage);
            Assert.AreEqual(2, provider.RequestedPublishList?.Count);
        }

        static void AddEntry(ICollection<IChangeEntry> list, string pathTag, ChangeEntryStatus status, bool staged, bool unmerged = false)
        {
            list.Add(new ChangeEntry(pathTag, $"Original{pathTag}", status, staged, unmerged));
        }

        static List<IChangeEntry> BuildChangesList(int count)
        {
            var changes = new List<IChangeEntry>();

            for (var i = 0; i < count; i++)
            {
                AddEntry(changes, $"Path{i}", ChangeEntryStatus.Modified, false);
            }

            return changes;
        }
    }
}
