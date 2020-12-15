// This code snippet was provided originally by stanislav.osipov@unity3d.com from #ui-elements slack channel.
using System;
using System.Collections;
using Unity.Cloud.Collaborate.Assets;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Tests
{
    internal static class UiTestHelpers
    {
        #region Events​
        // In order for tests to run without an EditorWindow but still be able to send
        // events, we sometimes need to force the event type. IMGUI::GetEventType() (native) will
        // return the event type as Ignore if the proper views haven't yet been
        // initialized. This (falsely) breaks tests that rely on the event type. So for tests, we
        // just ensure the event type is what we originally set it to when we sent it.
        // This original type can be retrieved via Event.rawType.
        static Event CreateEvent(Event evt)
        {
            return evt; //UIElementsUtility.CreateEvent(evt, evt.rawType);
        }

        public static Event MakeEvent(EventType type)
        {
            var evt = new Event { type = type };
            return CreateEvent(evt);
        }

        public static Event MakeEvent(EventType type, Vector2 position)
        {
            var evt = new Event { type = type, mousePosition = position };
            return CreateEvent(evt);
        }

        public static Event MakeKeyEvent(KeyCode code, EventType type = EventType.KeyDown, EventModifiers modifiers = EventModifiers.None, char character = '\0')
        {
            var evt = new Event { type = type, keyCode = code, character = character, modifiers = modifiers };
            return CreateEvent(evt);
        }

        public static Event MakeMouseEvent(EventType type, Vector2 position, MouseButton button = MouseButton.LeftMouse, EventModifiers modifiers = EventModifiers.None, int clickCount = 1)
        {
            var evt = new Event { type = type, mousePosition = position, button = (int)button, modifiers = modifiers, clickCount = clickCount };
            return CreateEvent(evt);
        }

        public static Event MakeScrollWheelEvent(Vector2 delta, Vector2 position)
        {
            var evt = new Event
            {
                type = EventType.ScrollWheel,
                delta = delta,
                mousePosition = position
            };
            return CreateEvent(evt);
        }

        public static Event MakeCommandEvent(EventType type, string command)
        {
            var evt = new Event { type = type, commandName = command };
            return CreateEvent(evt);
        }

        #endregion

        #region EditorWindow API

        public static bool IsCompletelyVisible(EditorWindow window, VisualElement element)
        {
            if (element.ClassListContains(UiConstants.ussHidden))
            {
                return false;
            }
            var windowBounds = window.rootVisualElement.worldBound;
            var elementBounds = element.worldBound;
            return elementBounds.x >= windowBounds.x
                && elementBounds.y >= windowBounds.y
                && windowBounds.x + windowBounds.width >= elementBounds.x + elementBounds.width
                && windowBounds.y + windowBounds.height >= elementBounds.y + elementBounds.height;
        }

        public static bool IsPartiallyVisible(EditorWindow window, VisualElement element)
        {
            if (element.ClassListContains(UiConstants.ussHidden))
            {
                return false;
            }
            var windowBounds = window.rootVisualElement.worldBound;
            var elementBounds = element.worldBound;
            return !(elementBounds.x > windowBounds.x + windowBounds.width)
                && !(elementBounds.x + elementBounds.width < windowBounds.x)
                && !(elementBounds.y > windowBounds.y + windowBounds.height)
                && !(elementBounds.y + elementBounds.height < windowBounds.y);
        }

        public static void SendMouseDownEvent(EditorWindow window, VisualElement element)
        {
            var evt = MakeMouseEvent(EventType.MouseDown, element.worldBound.center);
            window.SendEvent(evt);
        }

        public static void SendClickEvent(EditorWindow window, VisualElement element)
        {
            var evt = MakeMouseEvent(EventType.MouseDown, element.worldBound.center);
            window.SendEvent(evt);
            evt = MakeMouseEvent(EventType.MouseUp, element.worldBound.center);
            window.SendEvent(evt);
        }

        public static void SimulateTyping(EditorWindow window, string text)
        {
            foreach (var character in text)
            {
                var evt = MakeKeyEvent(KeyCode.None, EventType.KeyDown, EventModifiers.None, character);
                window.SendEvent(evt);
            }
        }

        public static void SimulateTyping(EditorWindow window, char character, int repetitions)
        {
            for (var i = 0; i < repetitions; i++)
            {
                var evt = MakeKeyEvent(KeyCode.None, EventType.KeyDown, EventModifiers.None, character);
                window.SendEvent(evt);
            }
        }

        public static IEnumerator Pause(int frames)
        {
            for (var i = 0; i < frames; i++)
            {
                yield return null;
            }
        }
        #endregion
    }
}


