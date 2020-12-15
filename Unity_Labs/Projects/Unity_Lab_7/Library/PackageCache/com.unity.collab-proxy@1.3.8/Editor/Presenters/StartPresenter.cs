using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.Views;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Cloud.Collaborate.Presenters
{
    internal class StartPresenter : IStartPresenter
    {
        [NotNull]
        readonly IStartView m_View;
        [NotNull]
        readonly IStartModel m_Model;

        bool m_IsStarted;

        public StartPresenter([NotNull] IStartView view, [NotNull] IStartModel model)
        {
            m_View = view;
            m_Model = model;
        }

        /// <inheritdoc />
        public void Start()
        {
            Assert.IsFalse(m_IsStarted, "The presenter has already been started.");
            m_IsStarted = true;

            m_Model.ProjectStatusChanged += OnProjectStatusChanged;
            m_Model.StateChanged += OnStateChanged;

            PopulateInitialData();
        }

        /// <inheritdoc />
        public void Stop()
        {
            Assert.IsTrue(m_IsStarted, "The presenter has already been stopped.");
            m_IsStarted = false;

            m_Model.ProjectStatusChanged -= OnProjectStatusChanged;
            m_Model.StateChanged -= OnStateChanged;
        }

        /// <summary>
        /// Refresh state from the model.
        /// </summary>
        void OnStateChanged()
        {
            PopulateInitialData();
        }

        /// <summary>
        /// Populate the view with the initial data from the model.
        /// </summary>
        void PopulateInitialData()
        {
            OnProjectStatusChanged(m_Model.ProjectStatus);
        }

        void OnProjectStatusChanged(ProjectStatus status)
        {
            switch (status) {
                case ProjectStatus.Offline:
                    m_View.Text = StringAssets.projectStatusTitleOffline;
                    m_View.ButtonText = string.Empty;
                    m_View.SetButtonVisible(false);
                    break;
                case ProjectStatus.Maintenance:
                    m_View.Text = StringAssets.projectStatusTitleMaintenance;
                    m_View.ButtonText = string.Empty;
                    m_View.SetButtonVisible(false);
                    break;
                case ProjectStatus.LoggedOut:
                    m_View.Text = StringAssets.projectStatusTitleLoggedOut;
                    m_View.ButtonText = StringAssets.projectStatusButtonLoggedOut;
                    m_View.SetButtonVisible(true);
                    break;
                case ProjectStatus.Unbound:
                    m_View.Text = StringAssets.projectStatusTitleUnbound;
                    m_View.ButtonText = StringAssets.projectStatusButtonUnbound;
                    m_View.SetButtonVisible(true);
                    break;
                case ProjectStatus.NoSeat:
                    m_View.Text = StringAssets.projectStatusTitleNoSeat;
                    m_View.ButtonText = StringAssets.projectStatusButtonNoSeat;
                    m_View.SetButtonVisible(true);
                    break;
                case ProjectStatus.Bound:
                    m_View.Text = StringAssets.projectStatusTitleBound;
                    m_View.ButtonText = StringAssets.projectStatusButtonBound;
                    m_View.SetButtonVisible(true);
                    break;
                case ProjectStatus.Loading:
                    m_View.Text = StringAssets.projectStatusTitleLoading;
                    m_View.ButtonText = string.Empty;
                    m_View.SetButtonVisible(false);
                    break;
                case ProjectStatus.Ready:
                    m_View.Text = string.Empty;
                    m_View.ButtonText = string.Empty;
                    m_View.SetButtonVisible(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, "Unexpected project status.");
            }
        }

        /// <inheritdoc />
        public void RequestStart()
        {
            var status = m_Model.ProjectStatus;
            switch (status) {
                case ProjectStatus.Unbound:
                    m_Model.ShowServicePage();
                    break;
                case ProjectStatus.LoggedOut:
                    m_Model.ShowLoginPage();
                    break;
                case ProjectStatus.NoSeat:
                    m_Model.ShowNoSeatPage();
                    break;
                case ProjectStatus.Bound:
                    // Turn on collab Service. This is where we do a Genesis request apparently.
                    m_Model.RequestTurnOnService();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, "Unexpected project status.");
            }
        }
    }
}
