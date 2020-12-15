using System;
using Unity.Cloud.Collaborate.Models.Enums;

namespace Unity.Cloud.Collaborate.Models
{
    internal interface IStartModel : IModel
    {
        /// <summary>
        /// Event that is triggered when the project status changes.
        /// </summary>
        event Action<ProjectStatus> ProjectStatusChanged;

        /// <summary>
        /// Returns the current project status.
        /// </summary>
        ProjectStatus ProjectStatus { get; }

        /// <summary>
        /// Request to turn on the service.
        /// </summary>
        void RequestTurnOnService();

        /// <summary>
        /// Show the service page.
        /// </summary>
        void ShowServicePage();

        /// <summary>
        /// Show login page.
        /// </summary>
        void ShowLoginPage();

        /// <summary>
        /// Show no seat page.
        /// </summary>
        void ShowNoSeatPage();
    }
}
