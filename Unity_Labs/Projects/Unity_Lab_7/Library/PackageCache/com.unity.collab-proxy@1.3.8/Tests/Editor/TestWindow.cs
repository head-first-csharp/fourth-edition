using System;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Tests
{
    internal class TestWindow : EditorWindow
    {
        void OnEnable()
        {
            var root = rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(CollaborateWindow.MainStylePath));

            root.AddToClassList(EditorGUIUtility.isProSkin
                ? UiConstants.ussDark
                : UiConstants.ussLight);
        }
    }
}
