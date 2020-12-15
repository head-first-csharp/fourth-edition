using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Models.Api;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Models
{
    internal class StartModel : IStartModel
    {
        [NotNull]
        readonly ISourceControlProvider m_Provider;

        /// <inheritdoc />
        public event Action<ProjectStatus> ProjectStatusChanged;

        /// <inheritdoc />
        public event Action StateChanged;

        public StartModel([NotNull] ISourceControlProvider provider)
        {
            m_Provider = provider;
        }

        /// <inheritdoc />
        public void OnStart()
        {
            m_Provider.UpdatedProjectStatus += OnUpdatedProjectStatus;
        }

        /// <inheritdoc />
        public void OnStop()
        {
            m_Provider.UpdatedProjectStatus -= OnUpdatedProjectStatus;
        }

        /// <inheritdoc />
        public void RestoreState(IWindowCache cache)
        {
            StateChanged?.Invoke();
        }

        /// <inheritdoc />
        public void SaveState(IWindowCache cache)
        {
        }

        /// <inheritdoc />
        public ProjectStatus ProjectStatus => m_Provider.GetProjectStatus();

        /// <inheritdoc />
        public void RequestTurnOnService()
        {
            m_Provider.RequestTurnOnService();
        }

        /// <inheritdoc />
        public void ShowServicePage()
        {
            m_Provider.ShowServicePage();
        }

        /// <inheritdoc />
        public void ShowLoginPage()
        {
            m_Provider.ShowLoginPage();
        }

        /// <inheritdoc />
        public void ShowNoSeatPage()
        {
            m_Provider.ShowNoSeatPage();
        }

        void OnUpdatedProjectStatus(ProjectStatus status)
        {
            ProjectStatusChanged?.Invoke(status);
        }
    }
}
