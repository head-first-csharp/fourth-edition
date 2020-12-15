using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components {
    internal class TextButton : TextElement
    {
        public const string UssClassName = "unity-text-button";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(TextButton)}.uss";

        public event Action Clicked;

        [UsedImplicitly]
        public TextButton() : this(null)
        {
        }

        public TextButton([CanBeNull] Action clickEvent = null)
        {
            AddToClassList(UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            Clicked += clickEvent;
            this.AddManipulator(new Clickable(OnClick));
        }

        void OnClick()
        {
            Clicked?.Invoke();
            Blur();
        }

        public override bool canGrabFocus { get; } = true;

        /// <summary>
        /// Catch the enter key event to allow for tab & enter UI navigation.
        /// </summary>
        /// <param name="evt">Event to check.</param>
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            // Catch enter key being pressed.
            if (!(evt is KeyDownEvent downEvent)) return;
            if ((downEvent.keyCode != KeyCode.KeypadEnter) && (downEvent.keyCode != KeyCode.Return)) return;

            Clicked?.Invoke();
            downEvent.StopPropagation();
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<IconTextButton, UxmlTraits> {}
    }
}
