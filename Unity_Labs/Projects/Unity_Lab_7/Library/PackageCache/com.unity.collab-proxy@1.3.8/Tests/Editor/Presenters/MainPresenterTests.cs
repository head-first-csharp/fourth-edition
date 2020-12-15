using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Structures;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Tests.Models;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    public class MainPresenterTests
    {
        TestMainView m_View;
        TestMainModel m_MainModel;
        MainPresenter m_Presenter;

        [SetUp]
        public void Setup()
        {
            m_View = new TestMainView();
            m_MainModel = new TestMainModel();
            m_Presenter = new MainPresenter(m_View, m_MainModel);
        }

        [TearDown]
        public void TearDown()
        {
            m_Presenter.Stop();
            m_View = null;
            m_MainModel = null;
            m_Presenter = null;
        }

        [Test]
        public void TestBackNavigation()
        {
            m_Presenter.Start();

            var called = false;
            m_MainModel.backNavigation = ("test", "test-text", () => called = true);
            m_Presenter.NavigateBack();

            Assert.IsTrue(called);
            Assert.IsNull(m_MainModel.backNavigation);
        }

        [Test]
        public void TestBackNavigationWithNull()
        {
            m_Presenter.Start();

            m_MainModel.backNavigation = null;
            m_Presenter.NavigateBack();

            Assert.IsNull(m_MainModel.backNavigation);
        }

        [Test]
        public void TestAssigningPresenters()
        {
            m_Presenter.Start();

            var changesView = new TestChangesView();
            m_Presenter.AssignChangesPresenter(changesView);
            Assert.IsNotNull(changesView.Presenter);

            var historyView = new TestHistoryView();
            m_Presenter.AssignHistoryPresenter(historyView);
            Assert.IsNotNull(historyView.Presenter);
        }

        [Test]
        public void TestCancellingJob()
        {
            m_Presenter.Start();

            m_Presenter.RequestCancelJob();

            Assert.AreEqual(1, m_MainModel.requestCancelJobCount);
        }

        [Test]
        public void TestSettingTabIndex()
        {
            m_Presenter.Start();

            const int newVal = 5;
            m_Presenter.UpdateTabIndex(newVal);

            Assert.AreEqual(newVal, m_MainModel.CurrentTabIndex);
        }

        [Test]
        public void TestStartingWithJobInProgress()
        {
            m_MainModel.ProgressInfo = new ProgressInfo("test", "test", 50, 20);
            m_Presenter.Start();

            Assert.IsTrue(m_View.inProgress);
            Assert.IsNotNull(m_View.progress);
        }

        [Test]
        public void TestStartingWithError()
        {
            const string message = "test message";
            m_MainModel.ErrorInfo = new ErrorInfo(20, 1, (int)ErrorInfoBehavior.Alert, message, "test", "20");
            m_Presenter.Start();

            Assert.AreEqual(1, m_View.alerts.Count);
            Assert.AreEqual(message, m_View.alerts.First().Value.message);
        }

        [Test]
        public void TestReceivingStateChange()
        {
            const string message = "test message";
            const int tabIndex = 67;

            m_Presenter.Start();
            m_MainModel.backNavigation = ("id", message, () => { });
            m_MainModel.CurrentTabIndex = tabIndex;
            m_MainModel.TriggerStateChanged();

            Assert.AreEqual(message, m_View.backNavigation);
            Assert.AreEqual(tabIndex, m_View.tabIndex);
        }
    }
}
