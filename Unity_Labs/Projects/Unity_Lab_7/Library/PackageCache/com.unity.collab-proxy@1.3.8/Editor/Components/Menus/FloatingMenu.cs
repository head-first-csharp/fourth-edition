using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.UserInterface;
using Unity.Cloud.Collaborate.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.Menus
{
    [UsedImplicitly]
    internal class FloatingMenu
    {
        public const string ussClassName = "unity-floating-menu";

        // Fields used to display the option list.
        const float k_ItemHeight = 28f;
        readonly List<(string Text, Action Action, bool Enabled)> m_Items;

        /// <summary>
        /// Location the uss file for this element is stored.
        /// </summary>
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(FloatingMenu)}.uss";

        /// <summary>
        /// Container for the menu items.
        /// </summary>
        readonly VisualElement m_Content;

        /// <summary>
        /// Direction to open the menu.
        /// </summary>
        MenuUtilities.OpenDirection m_OpenDirection = MenuUtilities.OpenDirection.DownLeft;

        /// <summary>
        /// Create a new floating menu. Follows the builder pattern.
        /// </summary>
        public FloatingMenu()
        {
            m_Items = new List<(string Text, Action Action, bool Enabled)>();
            m_Content = new VisualElement();
            m_Content.AddToClassList(ussClassName);
            m_Content.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
        }

        /// <summary>
        /// Add a single option to the menu.
        /// </summary>
        /// <param name="text">Text in the option.</param>
        /// <param name="action">Action to invoke on click.</param>
        /// <param name="enabled">State of the entry.</param>
        /// <returns>This.</returns>
        public FloatingMenu AddEntry(string text, Action action, bool enabled)
        {
            m_Items.Add((text, action, enabled));
            return this;
        }

        /// <summary>
        /// Add a list of entries.
        /// </summary>
        /// <param name="items">Entries to add.</param>
        /// <returns>This.</returns>
        public FloatingMenu AddEntries(IEnumerable<(string Text, Action Action, bool Enabled)> items)
        {
            m_Items.AddRange(items);
            return this;
        }

        /// <summary>
        /// Sets the open direction of the menu.
        /// </summary>
        /// <param name="openDirection">Direction the menu opens towards.</param>
        /// <returns>This.</returns>
        public FloatingMenu SetOpenDirection(MenuUtilities.OpenDirection openDirection)
        {
            m_OpenDirection = openDirection;
            return this;
        }

        /// <summary>
        /// Opens the constructed menu.
        /// </summary>
        /// <param name="x">World x coordinate.</param>
        /// <param name="y">World y coordinate.</param>
        public void Open(float x, float y)
        {
            FloatingDialogue.Instance.Open(x, y, GenerateContent(), m_OpenDirection);
        }

        /// <summary>
        /// Generate the visual element that displays the content of this menu.
        /// </summary>
        /// <returns>The constructed visual element.</returns>
        VisualElement GenerateContent()
        {
            m_Content.Clear();
            foreach (var item in m_Items)
            {
                // Ensure the dialogue closes once the option is selected
                void Action()
                {
                    FloatingDialogue.Instance.Close();
                    item.Action();
                }

                var elem = new FloatingMenuItem(item.Text, Action, item.Enabled, k_ItemHeight);
                m_Content.Add(elem);
            }

            return m_Content;
        }
    }
}
