using System.Collections.Generic;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Tests.Models;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    public class ChangesPresenterTests
    {
        TestChangesView m_View;
        TestChangesModel m_Model;
        TestMainModel m_MainModel;
        ChangesPresenter m_Presenter;

        [SetUp]
        public void Setup()
        {
            m_View = new TestChangesView();
            m_Model = new TestChangesModel();
            m_MainModel = new TestMainModel();
            m_Presenter = new ChangesPresenter(m_View, m_Model, m_MainModel);
        }

        [TearDown]
        public void TearDown()
        {
            m_Presenter.Stop();
            m_View = null;
            m_Model = null;
            m_MainModel = null;
            m_Presenter = null;
        }

        [Test]
        public void TestToggledCountValue()
        {
            m_Presenter.Start();
            m_Model.ToggledEntries = new List<IChangeEntryData>
            {
                new TestChangesModel.ChangeEntryData { Toggled = true, Entry = new ChangeEntry("path") },
                new TestChangesModel.ChangeEntryData { Toggled = true, Entry = new ChangeEntry("path2") }
            };
            Assert.AreEqual(2, m_Presenter.ToggledCount);
        }

        [Test]
        public void TestTotalCountValue()
        {
            m_Presenter.Start();
            m_Model.AllEntries = new List<IChangeEntryData>
            {
                new TestChangesModel.ChangeEntryData { Toggled = true, Entry = new ChangeEntry("path") },
                new TestChangesModel.ChangeEntryData { Toggled = false, Entry = new ChangeEntry("path2") }
            };
            Assert.AreEqual(2, m_Presenter.TotalCount);
        }

        [Test]
        public void TestConflictedCountValue()
        {
            m_Presenter.Start();
            m_Model.ConflictedEntries = new List<IChangeEntryData>
            {
                new TestChangesModel.ChangeEntryData
                {
                    Conflicted = true,
                    Entry = new ChangeEntry("path", null, ChangeEntryStatus.Unmerged, false, true)
                },
                new TestChangesModel.ChangeEntryData
                {
                    Conflicted = true,
                    Entry = new ChangeEntry("path2", null, ChangeEntryStatus.Unmerged, false, true)
                }
            };
            Assert.AreEqual(2, m_Presenter.ConflictedCount);
        }

        [Test]
        public void TestSearchingValue()
        {
            m_Presenter.Start();
            m_Model.SavedSearchQuery = "test";
            Assert.IsTrue(m_Presenter.Searching);
            m_Model.SavedSearchQuery = "";
            Assert.IsFalse(m_Presenter.Searching);
        }

        [Test]
        public void TestSettingEntryToggle()
        {
            m_Presenter.Start();
            const string path = "path";
            const bool value = true;
            m_Presenter.UpdateEntryToggle(path, value);
            Assert.AreEqual(1, m_Model.UpdateEntryToggleCount);
            Assert.AreEqual(path, m_Model.UpdateEntryTogglePath);
            Assert.AreEqual(value, m_Model.UpdateEntryToggleValue);
        }

        [Test]
        public void TestRequestPublish()
        {
            m_Presenter.Start();
            m_Model.SavedSearchQuery = "";
            m_Model.ToggledEntries = new List<IChangeEntryData>
            {
                new TestChangesModel.ChangeEntryData { Toggled = true, Entry = new ChangeEntry("path") }
            };
            m_Presenter.RequestPublish();
            Assert.AreEqual(1, m_Model.GetToggledEntriesCount);
            Assert.AreEqual(1, m_Model.RequestPublishCount);
            Assert.AreEqual(1, m_Model.RequestPublishList.Count);
        }

        [Test]
        public void TestRequestPublishFailWhenSearching()
        {
            m_Presenter.Start();
            m_Model.SavedSearchQuery = "some query";
            Assert.Catch(() => m_Presenter.RequestPublish());
        }

        [Test]
        public void TestRequestDiscard()
        {
            m_Presenter.Start();
            const string path = "path";
            var entry = new ChangeEntry(path);
            m_Presenter.RequestDiscard(entry);
            Assert.AreEqual(1, m_Model.RequestDiscardCount);
            Assert.AreEqual(path, m_Model.RequestDiscardEntry.Path);
            // Ensure it created a dialogue
            Assert.AreEqual(1, m_View.DisplayDialogueCount);
        }

        [Test]
        public void TestRequestDiffChanges()
        {
            m_Presenter.Start();
            const string path = "path";
            m_Presenter.RequestDiffChanges(path);
            Assert.AreEqual(1, m_Model.RequestDiffCount);
            Assert.AreEqual(path, m_Model.RequestDiffPath);
        }

        [Test]
        public void TestSetSearchQuery()
        {
            m_Presenter.Start();
            Assert.AreEqual(1, m_View.SetSearchQueryCount);
            const string query = "path Path   ";
            m_Presenter.SetSearchQuery(query);
            Assert.AreEqual(query.Trim().ToLower(), m_Model.SavedSearchQuery);
            Assert.AreEqual(2, m_View.SetSearchQueryCount);
            Assert.AreEqual(query, m_View.SetSearchQueryValue);
        }

        [Test]
        public void TestHavingSearchQueryDisablesPublish()
        {
            m_Presenter.Start();
            m_Model.ToggledEntries = new List<IChangeEntryData>
            {
                new TestChangesModel.ChangeEntryData { Toggled = true, Entry = new ChangeEntry("path") }
            };
            // Base case
            m_Presenter.SetSearchQuery("");
            Assert.AreEqual(1, m_View.SetPublishEnabledCount);
            Assert.AreEqual(true, m_View.SetPublishEnabledValue);
            // Base to disabled case
            m_Presenter.SetSearchQuery("query");
            Assert.AreEqual(2, m_View.SetPublishEnabledCount);
            Assert.AreEqual(false, m_View.SetPublishEnabledValue);
            Assert.IsNotNull(m_View.SetPublishEnabledReason);
            // Disabled back to base case.
            m_Presenter.SetSearchQuery("");
            Assert.AreEqual(3, m_View.SetPublishEnabledCount);
            Assert.AreEqual(true, m_View.SetPublishEnabledValue);
        }

        [Test]
        public void TestHavingConflictsDisablesPublish()
        {
            m_Model.ToggledEntries = new List<IChangeEntryData>()
            {
                new TestChangesModel.ChangeEntryData
                {
                    Entry = new ChangeEntry("path", null, ChangeEntryStatus.Modified)
                }
            };
            // Base case
            m_Presenter.Start();
            m_Model.ConflictedEntries = new List<IChangeEntryData>();
            m_Model.TriggerUpdatedChangeList();
            Assert.AreEqual(1, m_View.SetPublishEnabledCount);
            Assert.AreEqual(true, m_View.SetPublishEnabledValue);

            // Disable
            m_Model.ConflictedEntries = new List<IChangeEntryData>
            {
                new TestChangesModel.ChangeEntryData
                {
                    Conflicted = true,
                    Entry = new ChangeEntry("path", null, ChangeEntryStatus.Unmerged, false, true)
                }
            };
            m_Model.TriggerUpdatedChangeList();
            Assert.AreEqual(2, m_View.SetPublishEnabledCount);
            Assert.AreEqual(false, m_View.SetPublishEnabledValue);
            Assert.IsNotNull(m_View.SetPublishEnabledReason);

            // Re enabled
            m_Model.ConflictedEntries = new List<IChangeEntryData>();
            m_Model.TriggerUpdatedChangeList();
            Assert.AreEqual(3, m_View.SetPublishEnabledCount);
            Assert.AreEqual(true, m_View.SetPublishEnabledValue);
        }

        [Test]
        public void TestSetRevisionService()
        {
            m_Presenter.Start();
            Assert.AreEqual(1, m_View.SetRevisionSummaryCount);
            const string summary = "summary";
            m_Presenter.SetRevisionSummary(summary);
            Assert.AreEqual(summary, m_Model.SavedRevisionSummary);
            Assert.AreEqual(2, m_View.SetRevisionSummaryCount);
            Assert.AreEqual(summary, m_View.SetRevisionSummaryValue);
        }

        [Test]
        public void TestReceivingBusyMessage()
        {
            m_Presenter.Start();
            // Sent initial status on start
            Assert.AreEqual(1, m_View.SetBusyStatusCount);
            Assert.AreEqual(false, m_View.SetBusyStatusValue);

            // Test values once events called:
            m_Model.TriggerBusyStatusUpdated(true);
            Assert.AreEqual(2, m_View.SetBusyStatusCount);
            Assert.AreEqual(true, m_View.SetBusyStatusValue);

            m_Model.TriggerBusyStatusUpdated(false);
            Assert.AreEqual(3, m_View.SetBusyStatusCount);
            Assert.AreEqual(false, m_View.SetBusyStatusValue);
        }
    }
}
