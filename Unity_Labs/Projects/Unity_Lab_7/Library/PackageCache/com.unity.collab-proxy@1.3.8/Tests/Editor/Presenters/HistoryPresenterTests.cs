using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Tests.Models;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    public class HistoryPresenterTests
    {
        TestHistoryView m_View;
        TestHistoryModel m_HistoryModel;
        TestMainModel m_MainModel;
        HistoryPresenter m_Presenter;

        [SetUp]
        public void Setup()
        {
            m_View = new TestHistoryView();
            m_HistoryModel = new TestHistoryModel();
            m_MainModel = new TestMainModel();
            m_Presenter = new HistoryPresenter(m_View, m_HistoryModel, m_MainModel);
        }

        [TearDown]
        public void TearDown()
        {
            m_Presenter.Stop();
            m_View = null;
            m_HistoryModel = null;
            m_Presenter = null;
        }

        [Test]
        public void TestStartCall()
        {
            m_Presenter.Start();

            // Clear box testing here
            Assert.AreEqual(m_HistoryModel.Busy, m_View.BusyStatus);
            Assert.AreEqual(1, m_HistoryModel.RequestedEntryCountCount);
            Assert.AreEqual(1, m_HistoryModel.RequestedPageOfRevisionsCount);
            Assert.AreEqual(0, m_HistoryModel.RequestedSingleRevisionCount);
        }

        [Test]
        public void TestStartWithSavedRevisionCall()
        {
            const string savedRevisionId = "123";

            m_HistoryModel.SavedRevisionId = savedRevisionId;
            m_Presenter.Start();

            // Clear box testing here
            Assert.AreEqual(m_HistoryModel.Busy, m_View.BusyStatus);
            Assert.AreEqual(1, m_HistoryModel.RequestedEntryCountCount);
            Assert.AreEqual(0, m_HistoryModel.RequestedPageOfRevisionsCount);
            Assert.AreEqual(1, m_HistoryModel.RequestedSingleRevisionCount);
            Assert.AreEqual(savedRevisionId, m_HistoryModel.RequestedRevisionId);
        }

        [Test]
        public void TestReceivingUpdateEvent()
        {
            m_Presenter.Start();

            // Clear box testing here
            m_HistoryModel.TriggerUpdatedEntryListEvent();
            Assert.AreEqual(2, m_HistoryModel.RequestedEntryCountCount);
            Assert.AreEqual(2, m_HistoryModel.RequestedPageOfRevisionsCount);
            Assert.AreEqual(0, m_HistoryModel.RequestedSingleRevisionCount);
        }

        [Test]
        public void TestReceivingUpdateEventWithSelection()
        {
            const string selectedRevisionId = "123";

            m_HistoryModel.SelectedRevisionId = selectedRevisionId;
            m_Presenter.Start();

            // Clear box testing here
            m_HistoryModel.TriggerUpdatedEntryListEvent();
            Assert.AreEqual(2, m_HistoryModel.RequestedEntryCountCount);
            Assert.AreEqual(2, m_HistoryModel.RequestedSingleRevisionCount);
        }

        [Test]
        public void TestReceivingEntryCount()
        {
            m_Presenter.Start();

            m_HistoryModel.PageNumber = 1;
            m_HistoryModel.SetNumberOfEntries(HistoryPresenter.pageSize * 2 + 1);
            Assert.AreEqual(2, m_View.MaxPage);
            Assert.AreEqual(1, m_View.Page);
        }

        [Test]
        public void TestPreviousPageCall()
        {
            m_Presenter.Start();

            m_HistoryModel.PageNumber = 1;
            m_Presenter.PrevPage();
            Assert.AreEqual(HistoryPresenter.pageSize, m_HistoryModel.RequestedPageSize, "Page size should match given value");
            Assert.AreEqual(0, m_HistoryModel.PageNumber, "Requesting previous page should request previous page.");
        }

        [Test]
        public void TestPreviousPageCallOnZero()
        {
            m_Presenter.Start();

            m_HistoryModel.PageNumber = 0;
            m_Presenter.PrevPage();
            Assert.AreEqual(HistoryPresenter.pageSize, m_HistoryModel.RequestedPageSize, "Page size should match given value");
            Assert.AreEqual(0, m_HistoryModel.PageNumber, "Requesting previous page on page zero shouldn't stay at zero.");
        }

        [Test]
        public void TestNextPageCall()
        {
            m_Presenter.Start();

            m_HistoryModel.PageNumber = 0;
            m_HistoryModel.SetNumberOfEntries(HistoryPresenter.pageSize * 2);
            m_Presenter.NextPage();
            Assert.AreEqual(HistoryPresenter.pageSize, m_HistoryModel.RequestedPageSize, "Page size should match given value");
            Assert.AreEqual(1, m_HistoryModel.PageNumber, "Requesting previous page should request next page.");
        }

        [Test]
        public void TestNextPageCallOnZero()
        {
            m_Presenter.Start();

            m_HistoryModel.PageNumber = 1;
            m_HistoryModel.SetNumberOfEntries(HistoryPresenter.pageSize * 2);
            m_Presenter.NextPage();
            Assert.AreEqual(HistoryPresenter.pageSize, m_HistoryModel.RequestedPageSize, "Page size should match given value");
            Assert.AreEqual(1, m_HistoryModel.PageNumber, "Requesting next page on max page should not change the page.");
        }

        [Test]
        public void TestSetSelectedRevisionId()
        {
            const string selectedRevisionId = "123";
            m_Presenter.Start();

            m_Presenter.SelectedRevisionId = selectedRevisionId;
            Assert.AreEqual(selectedRevisionId, m_HistoryModel.RequestedRevisionId);
        }

        [Test]
        public void TestGotoCall()
        {
            const string revisionId = "123";
            m_Presenter.Start();

            var status = HistoryEntryStatus.Ahead;
            m_Presenter.RequestGoto(revisionId, status);
            Assert.AreEqual(1, m_HistoryModel.RequestedUpdateToCount);
            Assert.AreEqual(revisionId, m_HistoryModel.RequestedUpdateToRevisionId);

            status = HistoryEntryStatus.Current;
            m_Presenter.RequestGoto(revisionId, status);
            Assert.AreEqual(1, m_HistoryModel.RequestedRestoreToCount);
            Assert.AreEqual(revisionId, m_HistoryModel.RequestedRestoreToRevisionId);

            status = HistoryEntryStatus.Behind;
            m_Presenter.RequestGoto(revisionId, status);
            Assert.AreEqual(1, m_HistoryModel.RequestedGoBackToCount);
            Assert.AreEqual(revisionId, m_HistoryModel.RequestedGoBackToRevisionId);
        }

        [Test]
        public void TestRevertCall()
        {
            const string revisionId = "123";
            m_Presenter.Start();

            m_Presenter.RequestRevert(revisionId, new List<string> { "a", "b", "c" });
            Assert.AreEqual(1, m_HistoryModel.RequestedRevertCount);
            Assert.AreEqual(revisionId, m_HistoryModel.RequestedRevertRevisionId);
            Assert.AreEqual(3, m_HistoryModel.RequestedRevertFileCount);
        }
    }
}
