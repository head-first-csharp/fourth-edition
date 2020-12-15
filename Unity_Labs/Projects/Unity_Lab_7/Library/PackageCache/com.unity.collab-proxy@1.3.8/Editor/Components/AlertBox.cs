using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    [UsedImplicitly]
    internal class AlertBox : VisualElement
    {
        /// <summary>
        /// Describes the severity of the alert. Used to set the icon.
        /// </summary>
        public enum AlertLevel
        {
            Info,
            Warning,
            Alert
        }

        public const string UssClassName = "alert-box";
        public const string IconUssClassName = UssClassName + "__icon";
        public const string TextUssClassName = UssClassName + "__text";
        public const string ButtonUssClassName = UssClassName + "__button";

        static readonly string k_LayoutPath = $"{CollaborateWindow.LayoutPath}/{nameof(AlertBox)}.uxml";
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(AlertBox)}.uss";

        readonly Button m_Button;
        readonly VisualElement m_Icon;
        readonly TextElement m_Text;

        // Uss classes to set which icon is displayed.
        const string k_UssIconInfo = "icon-info";
        const string k_UssIconWarning = "icon-warning";
        const string k_UssIconAlert = "icon-alert";

        /// <summary>
        /// Queue of alerts to be displayed.
        /// </summary>
        readonly SortedSet<AlertEntry> m_AlertEntryList;

        /// <summary>
        /// Initialize the box and hide it.
        /// </summary>
        public AlertBox()
        {
            // Get the layout
            AddToClassList(UssClassName);
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_LayoutPath).CloneTree(this);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            // Initialise fields
            m_Icon = this.Q<VisualElement>(className: IconUssClassName);
            m_Text = this.Q<TextElement>(className: TextUssClassName);
            m_Button = this.Q<Button>(className: ButtonUssClassName);

            m_AlertEntryList = new SortedSet<AlertEntry>();

            // No alerts to display, so this hides the box.
            UpdateAlertBox();
        }

        /// <summary>
        /// Queue an alert to be displayed. Displayed immediately if there is currently none. If there is an existing
        /// alert with the same name, it will be replaced with the latest one.
        /// </summary>
        /// <param name="id">Unique ID for the queued alert.</param>
        /// <param name="level">Level of important of the alert.</param>
        /// <param name="message">Message to be displayed.</param>
        /// <param name="button">Info to populate optional button.</param>
        public void QueueAlert([NotNull] string id, AlertLevel level, [NotNull] string message, (string text, Action action)? button = null)
        {
            // Search for existing alert.
            var oldActive = m_AlertEntryList.Count == 0 ? (AlertEntry?)null : m_AlertEntryList.Max;

            // Create new alert entry.
            var entry = new AlertEntry(id, level, message, button == null
                ? (AlertEntry.AlertButton?)null
                : new AlertEntry.AlertButton { Text = button.Value.text, Action = button.Value.action });

            m_AlertEntryList.Add(entry);
            UpdateAlertBox(oldActive?.Button?.Action);
        }

        /// <summary>
        /// Remove existing alert. If current active one, switch to next one, or hide if none queued.
        /// </summary>
        /// <param name="id">Unique ID for the alert.</param>
        public void DequeueAlert([NotNull] string id)
        {
            var oldAlert = m_AlertEntryList.Max;

            m_AlertEntryList.RemoveWhere(e => e.Id == id);

            UpdateAlertBox(oldAlert.Button?.Action);
        }

        /// <summary>
        /// Display alert at the front of the queue, or hide if there are none.
        /// </summary>
        void UpdateAlertBox(Action previousButtonAction = null)
        {
            // Remove old event if it exists.
            if (previousButtonAction != null)
            {
                m_Button.clickable.clicked -= previousButtonAction;
            }

            if (m_AlertEntryList.Count == 0)
            {
                m_Button.text = string.Empty;
                m_Button.AddToClassList(UiConstants.ussHidden);
                AddToClassList(UiConstants.ussHidden);
            }
            else
            {
                var activeAlert = m_AlertEntryList.Max;

                m_Text.text = activeAlert.Message;
                // Update state of optional button
                if (activeAlert.Button?.Action != null)
                {
                    m_Button.clickable.clicked += activeAlert.Button.Value.Action;
                    m_Button.text = activeAlert.Button.Value.Text;
                    m_Button.RemoveFromClassList(UiConstants.ussHidden);
                }
                else
                {
                    m_Button.text = string.Empty;
                    m_Button.AddToClassList(UiConstants.ussHidden);
                }

                SetAlertLevel(activeAlert.Level);
                RemoveFromClassList(UiConstants.ussHidden);
            }
        }

        /// <summary>
        /// Set the icon to the given severity level.
        /// </summary>
        /// <param name="level">Level of severity to make the icon.</param>
        void SetAlertLevel(AlertLevel level)
        {
            // Remove old level
            m_Icon.RemoveFromClassList(k_UssIconInfo);
            m_Icon.RemoveFromClassList(k_UssIconWarning);
            m_Icon.RemoveFromClassList(k_UssIconAlert);

            // Set new one
            switch (level)
            {
                case AlertLevel.Info:
                    m_Icon.AddToClassList(k_UssIconInfo);
                    break;
                case AlertLevel.Warning:
                    m_Icon.AddToClassList(k_UssIconWarning);
                    break;
                case AlertLevel.Alert:
                    m_Icon.AddToClassList(k_UssIconAlert);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        struct AlertEntry : IComparable<AlertEntry>
        {
            public readonly string Id;
            public readonly AlertLevel Level;
            public readonly string Message;
            public AlertButton? Button;

            public AlertEntry(string id, AlertLevel level, string message, AlertButton? button)
            {
                Id = id;
                Level = level;
                Message = message;
                Button = button;
            }

            public struct AlertButton
            {
                public string Text;
                public Action Action;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is AlertEntry other && Id == other.Id;
            }

            public int CompareTo(AlertEntry other)
            {
                var value = Level.CompareTo(other.Level);
                // If same level, compare by id.
                return value != 0
                    ? value
                    : string.Compare(Id, other.Id, StringComparison.Ordinal);
            }
        }

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<AlertBox> { }
    }
}
