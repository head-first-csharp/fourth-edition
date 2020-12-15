using System;

namespace Unity.Cloud.Collaborate.Views.Adapters
{
    internal interface IAdapterObserver
    {
        void NotifyDataSetChanged();
    }
}
