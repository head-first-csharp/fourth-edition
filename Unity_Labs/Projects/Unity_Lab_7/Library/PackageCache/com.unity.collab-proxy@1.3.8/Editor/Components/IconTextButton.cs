using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    internal class IconTextButton : VisualElement
    {
        public const string ussClassName = "unity-icon-text-button";
        public const string imageUssClassName = ussClassName + "__image";
        public const string textUssClassName = ussClassName + "__text";

        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(IconTextButton)}.uss";

        readonly TextElement m_TextElement;
        readonly VisualElement m_Image;

        public event Action Clicked;

        public IconTextButton() : this(null)
        {
        }

        public IconTextButton([CanBeNull] Action clickEvent = null)
        {
            AddToClassList(ussClassName);
            AddToClassList(Button.ussClassName);

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            Add(m_Image = new Image());
            Add(m_TextElement = new TextElement());
            m_Image.AddToClassList(imageUssClassName);
            m_TextElement.AddToClassList(textUssClassName);

            // Setup Clickable
            Clicked += clickEvent;
            this.AddManipulator(new Clickable(OnClick));
        }

        void OnClick()
        {
            Clicked?.Invoke();
            Blur();
        }

        public string Text
        {
            get => m_TextElement.text;
            set => m_TextElement.text = value;
        }

        public Texture2D Image
        {
            get => m_Image.style.backgroundImage.value.texture;
            set => m_Image.style.backgroundImage = value;
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
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var field = (IconTextButton)ve;
                field.Text = m_Text.GetValueFromBag(bag, cc);
            }
        }
    }
}
