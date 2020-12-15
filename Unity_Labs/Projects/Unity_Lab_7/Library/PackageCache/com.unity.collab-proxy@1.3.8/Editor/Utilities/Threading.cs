using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Utilities
{
    internal static class Threading
    {
        /// <summary>
        /// Returns true if the current thread is the main thread, false otherwise.
        /// </summary>
        public static bool IsMainThread => InternalEditorUtility.CurrentThreadIsMainThread();

        /// <summary>
        /// Ensure that the provided action is executed on the UI/main thread.
        /// </summary>
        /// <param name="action">Action to perform on the UI/main thread.</param>
        public static void EnsureUiThread(Action action)
        {
            if (IsMainThread)
            {
                action();
            }
            else
            {
                EditorApplication.delayCall += () => action();
            }
        }
    }
}
