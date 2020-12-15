using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;

namespace Unity.Cloud.Collaborate.Models
{
    internal interface IModel
    {
        /// <summary>
        /// Inform the presenter that the state of the model has changed.
        /// </summary>
        event Action StateChanged;

        /// <summary>
        /// Called when the model is started and the model should setup events and fetch data.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Called when the model should be stopped and data and events should closed.
        /// </summary>
        void OnStop();

        /// <summary>
        /// Restores the state of the model from the provide cache. Must be called after OnStart.
        /// </summary>
        /// <param name="cache">Cache to read the state from.</param>
        void RestoreState([NotNull] IWindowCache cache);

        /// <summary>
        /// Saves the state of the model into the cache. Must be called before OnStop.
        /// </summary>
        /// <param name="cache">Cache to save the state into.</param>
        void SaveState([NotNull] IWindowCache cache);
    }
}
