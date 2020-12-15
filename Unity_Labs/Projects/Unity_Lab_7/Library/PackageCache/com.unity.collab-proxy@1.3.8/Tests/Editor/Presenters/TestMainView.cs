using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.Cloud.Collaborate.Components;
using Unity.Cloud.Collaborate.Presenters;
using Unity.Cloud.Collaborate.Views;

namespace Unity.Cloud.Collaborate.Tests.Presenters
{
    internal class TestMainView : IMainView
    {
        public int? tabIndex;

        public bool inProgress;

        public (string title, string details, int percentage, int completed, int total, bool isPercentage, bool canCancel)? progress;

        [CanBeNull]
        public string backNavigation;

        public IMainPresenter Presenter { get; set; }

        public Dictionary<string, (string id, AlertBox.AlertLevel level, string message, (string text, Action action)? button)> alerts = new Dictionary<string, (string id, AlertBox.AlertLevel level, string message, (string text, Action action)? button)>();

        public void AddAlert(string id, AlertBox.AlertLevel level, string message, (string text, Action action)? button = null)
        {
            alerts[id] = (id, level, message, button);
        }

        public void RemoveAlert(string id)
        {
            alerts.Remove(id);
        }

        public void SetTab(int index)
        {
            tabIndex = index;
        }

        public void AddOperationProgress()
        {
            Assert.IsFalse(inProgress);
            inProgress = true;
        }

        public void RemoveOperationProgress()
        {
            Assert.IsTrue(inProgress);
            inProgress = false;
        }

        public void SetOperationProgress(string title, string details, int percentage, int completed, int total, bool isPercentage, bool canCancel)
        {
            progress = (title, details, percentage, completed, total, isPercentage, canCancel);
        }

        public void ClearBackNavigation()
        {
            backNavigation = null;
        }

        public void DisplayBackNavigation(string text)
        {
            backNavigation = text;
        }
    }
}
