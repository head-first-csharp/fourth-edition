using UnityEditor;
using UnityEditor.Collaboration;
using UnityEngine;

using Unity.Cloud.Collaborate.UserInterface;

namespace CollabProxy.UI
{
    [InitializeOnLoad]
    public class Bootstrap
    {
        static Bootstrap()
        {
            var toolbar = new ToolbarButton { Width = 32f };
            Toolbar.AddSubToolbar(toolbar);
            toolbar.Update();

            Collab.ShowHistoryWindow += () =>
            {
                CollaborateWindow.Init(CollaborateWindow.FocusTarget.History);
            };

            Collab.ShowChangesWindow += () =>
            {
                CollaborateWindow.Init(CollaborateWindow.FocusTarget.Changes);
            };
        }
    }
}
