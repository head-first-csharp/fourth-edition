using System;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    // Hopefully these features will eventually be in the default TextField eventually.
    internal class BetterTextField : TextField
    {
        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public const string UssClassName = "unity-better-text-field";

        /// <summary>
        /// USS class name of placeholder elements of this type.
        /// </summary>
        public const string PlaceholderUssClassName = UssClassName + "__placeholder";

        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(BetterTextField)}.uss";

        readonly Label m_PlaceholderLabel;

        /// <summary>
        /// Notify external subscribers that value of text property changed.
        /// </summary>
        public Action<string> OnValueChangedHandler;

        public BetterTextField()
        {
            AddToClassList(UssClassName);

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Add and configure placeholder
            m_PlaceholderLabel = new Label { pickingMode = PickingMode.Ignore };
            m_PlaceholderLabel.AddToClassList(PlaceholderUssClassName);
            Add(m_PlaceholderLabel);

            RegisterCallback<FocusInEvent>(e => HidePlaceholder());

            RegisterCallback<FocusOutEvent>(e =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    ShowPlaceholder();
                }
            });

            this.RegisterValueChangedCallback(e => OnValueChangedHandler?.Invoke(e.newValue));
        }

        void UpdatePlaceholderVisibility()
        {
            if (string.IsNullOrEmpty(value))
            {
                // Value can be set before the focus control is initialised.
                if (focusController?.focusedElement != this)
                {
                    ShowPlaceholder();
                }
            }
            else
            {
                HidePlaceholder();
            }
        }

        void HidePlaceholder()
        {
            m_PlaceholderLabel?.AddToClassList(UiConstants.ussHidden);
        }

        void ShowPlaceholder()
        {
            m_PlaceholderLabel?.RemoveFromClassList(UiConstants.ussHidden);
        }

        public override string value
        {
            get => base.value;
            set
            {
                // Catch case of value being set programatically.
                base.value = value;
                UpdatePlaceholderVisibility();
            }
        }

        public string Placeholder
        {
            get => m_PlaceholderLabel.text;
            set => m_PlaceholderLabel.text = value;
        }

        public override void SetValueWithoutNotify(string newValue)
        {
            base.SetValueWithoutNotify(newValue);
            UpdatePlaceholderVisibility();
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<BetterTextField, UxmlTraits> { }
        public new class UxmlTraits : TextField.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Hint = new UxmlStringAttributeDescription { name = "placeholder" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var field = (BetterTextField)ve;
                field.Placeholder = m_Hint.GetValueFromBag(bag, cc);
            }
        }

    }
}
