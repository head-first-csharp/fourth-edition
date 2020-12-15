using System.Collections.Generic;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    public class HistoryModelTests
    {
        TestSourceControlProvider m_Provider;
        HistoryModel m_Model;

        int m_BusyStatusUpdatedCount;
        int m_EntryCountUpdatedCount;
        int m_HistoryListReceivedCount;
        int m_HistoryListUpdatedCount;
        int m_SelectedRevisionReceivedCount;

        [SetUp]
        public void Setup()
        {
            m_Provider = new TestSourceControlProvider();
            m_Model = new HistoryModel(m_Provider);
            m_Model.OnStart();

            m_BusyStatusUpdatedCount = 0;
            m_EntryCountUpdatedCount = 0;
            m_HistoryListReceivedCount = 0;
            m_HistoryListUpdatedCount = 0;
            m_SelectedRevisionReceivedCount = 0;

            m_Model.BusyStatusUpdated += _ => m_BusyStatusUpdatedCount++;
            m_Model.EntryCountUpdated += _ => m_EntryCountUpdatedCount++;
            m_Model.HistoryListReceived += _ => m_HistoryListReceivedCount++;
            m_Model.HistoryListUpdated += () => m_HistoryListUpdatedCount++;
            m_Model.SelectedRevisionReceived += _ => m_SelectedRevisionReceivedCount++;
        }

        [TearDown]
        public void TearDown()
        {
            m_Model = null;
            m_Provider = null;
        }

        [Test]
        public void TestHistoryListUpdated()
        {
            m_Provider.TriggerUpdatedHistoryEntries();
            Assert.AreEqual(1, m_HistoryListUpdatedCount);
        }

        [Test]
        public void TestIsBusyAndEntryCount()
        {
            m_Model.RequestEntryNumber();
            Assert.AreEqual(1, m_Provider.RequestedHistoryCountCount);
            Assert.AreEqual(false, m_Model.Busy);
            Assert.AreEqual(2, m_BusyStatusUpdatedCount);
            Assert.AreEqual(1, m_EntryCountUpdatedCount);
        }

        [Test]
        public void TestRequestingPageOfRevisions()
        {
            m_Model.PageNumber = 2;
            m_Model.RequestPageOfRevisions(10);
            Assert.AreEqual(1, m_Provider.RequestedHistoryListCount);
            Assert.AreEqual(20, m_Provider.RequestedHistoryListOffset);
            Assert.AreEqual(10, m_Provider.RequestedHistoryListSize);
            Assert.AreEqual(2, m_BusyStatusUpdatedCount);
            Assert.AreEqual(1, m_HistoryListReceivedCount);
        }

        [Test]
        public void TestRequestingSingleRevision()
        {
            const string revisionId = "123";

            Assert.AreEqual(false, m_Model.IsRevisionSelected);
            Assert.AreEqual(string.Empty, m_Model.SelectedRevisionId);
            m_Model.RequestSingleRevision(revisionId);
            Assert.AreEqual(true, m_Model.IsRevisionSelected);
            Assert.AreEqual(1, m_Provider.RequestedHistoryRevisionCount);
            Assert.AreEqual(revisionId, m_Provider.RequestedHistoryRevisionId);
            Assert.AreEqual(2, m_BusyStatusUpdatedCount);
            Assert.AreEqual(1, m_SelectedRevisionReceivedCount);
        }

        [Test]
        public void TestRequestUpdateTo()
        {
            const string revisionId = "123";

            m_Model.RequestUpdateTo(revisionId);
            Assert.AreEqual(1, m_Provider.RequestedUpdateToCount);
            Assert.AreEqual(revisionId, m_Provider.RequestedUpdateToRevisionId);
        }

        [Test]
        public void TestRequestRestoreTo()
        {
            const string revisionId = "123";

            m_Model.RequestRestoreTo(revisionId);
            Assert.AreEqual(1, m_Provider.RequestedRestoreToCount);
            Assert.AreEqual(revisionId, m_Provider.RequestedRestoreToRevisionId);
        }

        [Test]
        public void TestRequestGoBackTo()
        {
            const string revisionId = "123";

            m_Model.RequestGoBackTo(revisionId);
            Assert.AreEqual(1, m_Provider.RequestedGoBackToCount);
            Assert.AreEqual(revisionId, m_Provider.RequestedGoBackToRevisionId);
        }

        [Test]
        public void TestRevert()
        {
            const string revisionId = "123";

            m_Model.RequestRevert(revisionId, new List<string> { "a", "b", "c" });
            Assert.AreEqual(1, m_Provider.RequestedRevertCount);
            Assert.AreEqual(revisionId, m_Provider.RequestedRevertRevisionId);
            Assert.AreEqual(3, m_Provider.RequestedRevertFileCount);
        }
    }
}
