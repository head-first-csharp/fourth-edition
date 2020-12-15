using System;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.Menus
{
    internal class FloatingMenuItem : VisualElement
    {
        const string k_UssClassName = "unity-floating-menu-item";

        /// <summary>
        /// Location the uss file for this element is stored.
        /// </summary>
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(FloatingMenuItem)}.uss";

        public override bool canGrabFocus { get; } = true;

        readonly Action m_Action;

        public FloatingMenuItem(string text, Action action, bool enabled, float height)
        {
            AddToClassList(k_UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
            m_Action = action;
            focusable = true;
            this.AddManipulator(new Clickable(OnExec));
            SetEnabled(enabled);
            Add(new Label(text));
            style.height = height;
        }

        void OnExec()
        {
            m_Action();
        }

        /// <summary>
        /// Catch the enter key event to allow for tab & enter UI navigation.
        /// </summary>
        /// <param name="evt">Event to check.</param>
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            // Catch enter key being pressed.
            if (!(evt is KeyDownEvent downEvent)) return;
            if (downEvent.keyCode != KeyCode.KeypadEnter && downEvent.keyCode != KeyCode.Return) return;

            OnExec();
            downEvent.StopPropagation();
        }
    }
}
