using NUnit.Framework;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Tests.Models;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    internal class StartPresenterTests
    {
        TestStartView m_View;
        TestStartModel m_Model;
        StartPresenter m_Presenter;

        [SetUp]
        public void Setup()
        {
            m_View = new TestStartView();
            m_Model = new TestStartModel();;
            m_Presenter = new StartPresenter(m_View, m_Model);
        }

        [TearDown]
        public void TearDown()
        {
            m_Presenter.Stop();
            m_View = null;
            m_Model = null;
            m_Presenter = null;
        }

        [Test]
        public void TestRequestingStart()
        {
            m_Presenter.Start();

            m_Model.ProjectStatus = ProjectStatus.Bound;
            m_Presenter.RequestStart();
            Assert.AreEqual(1, m_Model.requestTurnOnServiceCount);

            m_Model.ProjectStatus = ProjectStatus.Unbound;
            m_Presenter.RequestStart();
            Assert.AreEqual(1, m_Model.showServicePageCount);

            m_Model.ProjectStatus = ProjectStatus.LoggedOut;
            m_Presenter.RequestStart();
            Assert.AreEqual(1, m_Model.showLoginPageCount);

            m_Model.ProjectStatus = ProjectStatus.NoSeat;
            m_Presenter.RequestStart();
            Assert.AreEqual(1, m_Model.showNoSeatPageCount);
        }

        [Test]
        public void TestUpdatingProjectStatus()
        {
            m_Presenter.Start();

            m_Model.TriggerProjectStatusChanged(ProjectStatus.Bound);
            Assert.IsTrue(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.Unbound);
            Assert.IsTrue(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.LoggedOut);
            Assert.IsTrue(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.NoSeat);
            Assert.IsTrue(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.Loading);
            Assert.IsFalse(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.Offline);
            Assert.IsFalse(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.Maintenance);
            Assert.IsFalse(m_View.buttonVisible);

            m_Model.TriggerProjectStatusChanged(ProjectStatus.Ready);
            Assert.IsFalse(m_View.buttonVisible);
        }

        [Test]
        public void TestStateChange()
        {
            m_Presenter.Start();

            m_Model.ProjectStatus = ProjectStatus.Bound;
            m_Model.TriggerStateChanged();

            Assert.IsTrue(m_View.buttonVisible);
        }
    }
}
