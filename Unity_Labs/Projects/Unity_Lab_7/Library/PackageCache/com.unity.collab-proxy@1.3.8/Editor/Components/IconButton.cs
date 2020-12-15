using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class IconButton : Image
    {
        public const string UssClassName = "unity-icon-button";
        public const string UndoUssClassName = "btn-undo";
        public const string ShowUssClassName = "btn-show";
        public const string MergeUssClassName = "btn-merge";
        public const string ChooseMineUssClassName = "btn-choose-mine";
        public const string ChooseRemoteUssClassName = "btn-choose-remote";
        public const string DiffUssCLassName = "btn-diff";

        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(IconButton)}.uss";

        public event Action Clicked;

        public IconButton() : this(null)
        {
        }

        public IconButton([CanBeNull] Action clickEvent = null)
        {
            AddToClassList(UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Setup Clickable
            Clicked += clickEvent;
            this.AddManipulator(new Clickable(OnClick));
        }

        void OnClick()
        {
            Clicked?.Invoke();
            Blur();
        }

        /// <summary>
        /// Remove all event handlers for the Clicked event.
        /// </summary>
        public void UnregisterClickEvents()
        {
            Clicked = null;
        }

        public Texture2D Image
        {
            get => style.backgroundImage.value.texture;
            set => style.backgroundImage = value;
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
        public new class UxmlFactory : UxmlFactory<IconButton> {}
    }
}
