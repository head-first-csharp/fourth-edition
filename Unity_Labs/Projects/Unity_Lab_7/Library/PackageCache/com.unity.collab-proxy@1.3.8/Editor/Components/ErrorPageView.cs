using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class ErrorPageView : PageComponent
    {
        public const string UssClassName = "error-page-view";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(ErrorPageView)}.uxml";

        bool m_Visible;

        public ErrorPageView()
        {
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
        }

        protected override void SetActive()
        {
            throw new NotImplementedException();
        }

        protected override void SetInactive()
        {
            throw new NotImplementedException();
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<ErrorPageView> { }
    }
}
