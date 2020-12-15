using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Presenters;

namespace Unity.Cloud.Collaborate.Views
{
    /// <summary>
    /// Interface for all views in the UI.
    /// </summary>
    /// <typeparam name="T">Type of presenter this view takes.</typeparam>
    interface IView<in T> where T : IPresenter
    {
        /// <summary>
        /// Presenter for this view.
        /// </summary>
        [NotNull]
        T Presenter { set; }
    }
}
