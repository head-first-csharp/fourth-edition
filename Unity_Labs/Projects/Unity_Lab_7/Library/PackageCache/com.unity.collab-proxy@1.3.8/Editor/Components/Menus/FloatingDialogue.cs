using System;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using Unity.Cloud.Collaborate.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components.Menus
{
    /// <summary>
    /// Global element that is used to display dialogues throughout the window.
    /// </summary>
    internal class FloatingDialogue : VisualElement
    {
        /// <summary>
        /// USS class name of elements of this type.
        /// </summary>
        public const string UssClassName = "unity-floating-dialogue";

        /// <summary>
        /// Location the uss file for this element is stored.
        /// </summary>
        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(FloatingDialogue)}.uss";

        /// <summary>
        /// Backing field for the singleton.
        /// </summary>
        static FloatingDialogue s_Instance;

        /// <summary>
        /// Singleton for accessing the global FloatingDialogue
        /// </summary>
        public static FloatingDialogue Instance => s_Instance ?? (s_Instance = new FloatingDialogue());

        /// <summary>
        /// Constructor used by the singleton.
        /// </summary>
        FloatingDialogue()
        {
            AddToClassList(UssClassName);
            AddToClassList(UiConstants.ussHidden);

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));
        }

        /// <summary>
        /// Allows focus to be set when the window opens. Allows for the options to be next up for tab.
        /// </summary>
        public override bool canGrabFocus => true;

        /// <summary>
        /// Set the position of the dialogue.
        /// </summary>
        /// <param name="x">World x coordinate.</param>
        /// <param name="y">World y coordinate.</param>
        /// <param name="content">Content of the window.</param>
        /// <param name="openDirection">Direction to open the dialogue towards.</param>
        void SetPosition(float x, float y, VisualElement content, MenuUtilities.OpenDirection openDirection)
        {
            // Correct for the top left corner of the element based on the direction it opens.
            switch (openDirection)
            {
                case MenuUtilities.OpenDirection.UpLeft:
                    x -= content.worldBound.width;
                    y -= content.worldBound.height;
                    break;
                case MenuUtilities.OpenDirection.UpRight:
                    y -= content.worldBound.height;
                    break;
                case MenuUtilities.OpenDirection.DownLeft:
                    x -= content.worldBound.width;
                    break;
                case MenuUtilities.OpenDirection.DownRight:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(openDirection), openDirection, null);
            }

            // Set the position.
            style.top = y - parent.worldBound.yMin;
            style.left = x - parent.worldBound.xMin;
            style.right = new StyleLength(StyleKeyword.Auto);
            style.bottom = new StyleLength(StyleKeyword.Auto);
        }

        /// <summary>
        /// Open the dialogue.
        /// </summary>
        /// <param name="x">World x coordinate.</param>
        /// <param name="y">World y coordinate.</param>
        /// <param name="content">Content to display.</param>
        /// <param name="openDirection">Direction to open the dialogue towards.</param>
        internal void Open(float x, float y, VisualElement content, MenuUtilities.OpenDirection openDirection = MenuUtilities.OpenDirection.DownLeft)
        {
            // Set new content
            Clear();
            Add(content);

            // Set visible and give focus
            RemoveFromClassList(UiConstants.ussHidden);
            Focus();
            BringToFront();

            // Catch scrolling to avoid menu becoming detached.
            parent.RegisterCallback<WheelEvent>(OnScroll, TrickleDown.TrickleDown);
            // Catch clicks outside of the dialogue and close it.
            parent.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
            // Catch window size changes so that the menu position can be fixed.
            parent.RegisterCallback<GeometryChangedEvent>(OnGeometryChange, TrickleDown.TrickleDown);

            content.RegisterCallback<GeometryChangedEvent>(SizeKnownCallback);
            void SizeKnownCallback(GeometryChangedEvent _)
            {
                // Unregister now that we know it has a size.
                content.UnregisterCallback<GeometryChangedEvent>(SizeKnownCallback);

                // Set the position in the window
                SetPosition(x, y, content, openDirection);
            }
        }

        /// <summary>
        /// Close the dialogue.
        /// </summary>
        internal void Close()
        {
            AddToClassList(UiConstants.ussHidden);
            Clear();

            // Avoid wasting extra cycles when closed.
            parent.UnregisterCallback<WheelEvent>(OnScroll, TrickleDown.TrickleDown);
            parent.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        }

        /// <summary>
        /// On scroll event.
        /// </summary>
        /// <param name="evt">Event data.</param>
        void OnScroll(WheelEvent evt)
        {
            // Close the window if the user scrolls outside the dialogue.
            if (!worldBound.Contains(evt.mousePosition))
            {
                Close();
            }
        }

        /// <summary>
        /// Mouse down event.
        /// </summary>
        /// <param name="evt">Event data.</param>
        void OnMouseDown(MouseDownEvent evt)
        {
            // Close the window if the user clicks outside the dialogue.
            if (!worldBound.Contains(evt.mousePosition))
            {
                Close();
            }
        }

        /// <summary>
        /// Geometry changed event.
        /// </summary>
        /// <param name="evt">Event data.</param>
        void OnGeometryChange(GeometryChangedEvent evt)
        {
            // Close to avoid the dialogue getting detached.
            Close();
        }
    }
}
