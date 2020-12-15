using System;
using System.Threading;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    class SearchBar : VisualElement
    {
        public const string UssClassName = "unity-search-bar";
        public const string SearchFieldUssClassName = UssClassName + "__search-field";
        public const string PlaceholderUssClassName = UssClassName + "__placeholder";

        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(SearchBar)}.uss";

        public const int timeoutMilliseconds = 1000;

        readonly ToolbarSearchField m_SearchField;
        readonly Label m_Placeholder;

        string m_LatestSearchValue;
        bool m_SearchEventFlag;
        readonly Timer m_SearchEventTimer;

        bool m_Focused;

        public event Action<string> Search = delegate { };

        public SearchBar() : this(null)
        {
        }

        public SearchBar([CanBeNull] Action<string> searchEvent = null)
        {
            // Setup delayed search event to throttle requests.
            m_SearchEventTimer = new Timer(_ => EditorApplication.delayCall += () =>
            {
                m_SearchEventFlag = false;
                Search(m_LatestSearchValue);
            });

            AddToClassList(UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_SearchField = new ToolbarSearchField();
            m_Placeholder = new Label { text = StringAssets.search, pickingMode = PickingMode.Ignore };

            m_SearchField.AddToClassList(SearchFieldUssClassName);
            m_Placeholder.AddToClassList(PlaceholderUssClassName);

            Add(m_SearchField);
            Add(m_Placeholder);

            // Setup search event
            if (searchEvent != null)
            {
                Search += searchEvent;
            }

            // Setup events to hide/show placeholder.
            var textField = m_SearchField.Q<TextField>(className: ToolbarSearchField.textUssClassName);
            textField.RegisterCallback<FocusInEvent>(e =>
            {
                m_Focused = true;
                UpdatePlaceholderVisibility();
            });
            textField.RegisterCallback<FocusOutEvent>(e =>
            {
                m_Focused = false;
                UpdatePlaceholderVisibility();
            });
            m_SearchField.RegisterValueChangedCallback(SearchEventThrottle);

            // Set initial placeholder hide/show status.
            ShowPlaceholder();
        }

        void SearchEventThrottle(ChangeEvent<string> evt)
        {
            UpdatePlaceholderVisibility();
            m_LatestSearchValue = evt.newValue;
            if (m_SearchEventFlag) return;
            m_SearchEventFlag = true;
            m_SearchEventTimer.Change(timeoutMilliseconds, Timeout.Infinite);
        }

        public string Value
        {
            get => m_SearchField.value;
            set
            {
                m_SearchField.value = value;
                UpdatePlaceholderVisibility();
            }
        }

        public void SetValueWithoutNotify(string value)
        {
            m_SearchField.SetValueWithoutNotify(value);
            UpdatePlaceholderVisibility();
        }

        void UpdatePlaceholderVisibility()
        {
            if (string.IsNullOrEmpty(m_SearchField.value) && !m_Focused)
            {

                ShowPlaceholder();
            }
            else
            {
                HidePlaceholder();
            }
        }

        void HidePlaceholder()
        {
            m_Placeholder.AddToClassList(UiConstants.ussHidden);
        }

        void ShowPlaceholder()
        {
            m_Placeholder.RemoveFromClassList(UiConstants.ussHidden);
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<SearchBar> { }
    }
}
