using System;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Views.Adapters {
    internal interface IAdapter
    {
        int Height { get; }

        Func<VisualElement> MakeItem { get; }

        Action<VisualElement, int> BindItem { get; }

        int GetEntryCount();

        void RegisterObserver(IAdapterObserver observer);

        void DeregisterObserver(IAdapterObserver observer);
    }
}
