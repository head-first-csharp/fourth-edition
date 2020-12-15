using System;
using Unity.Cloud.Collaborate.Models;
using Unity.Cloud.Collaborate.Models.Enums;
using Unity.Cloud.Collaborate.UserInterface;

namespace Unity.Cloud.Collaborate.Tests.Models
{
    internal class TestStartModel : IStartModel
    {
        public int requestTurnOnServiceCount;
        public int showServicePageCount;
        public int showLoginPageCount;
        public int showNoSeatPageCount;
        
        public event Action StateChanged = delegate {  };
        public void TriggerStateChanged()
        {
            StateChanged();
        }

        public event Action<ProjectStatus> ProjectStatusChanged = delegate {  };
        public void TriggerProjectStatusChanged(ProjectStatus status)
        {
            ProjectStatusChanged(status);
        }

        public ProjectStatus ProjectStatus { get; set; }

        public void OnStart()
        {
            throw new NotImplementedException();
        }

        public void OnStop()
        {
            throw new NotImplementedException();
        }

        public void RestoreState(IWindowCache cache)
        {
            throw new NotImplementedException();
        }

        public void SaveState(IWindowCache cache)
        {
            throw new NotImplementedException();
        }

        public void RequestTurnOnService()
        {
            requestTurnOnServiceCount++;
        }

        public void ShowServicePage()
        {
            showServicePageCount++;
        }

        public void ShowLoginPage()
        {
            showLoginPageCount++;
        }

        public void ShowNoSeatPage()
        {
            showNoSeatPageCount++;
        }
    }
}
