using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Cloud.Collaborate.Assets;
using Unity.Cloud.Collaborate.UserInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Components
{
    [UsedImplicitly]
    internal class TabView : VisualElement
    {
        public const string UssClassName = "unity-tab-view";
        public const string ContentContainerUssClassName = UssClassName + "__content-container";
        public const string TabHeaderUssClassName = UssClassName + "__tab-header";
        public const string ToolbarUssClassName = UssClassName + "__toolbar";

        static readonly string k_StylePath = $"{CollaborateWindow.StylePath}/{nameof(TabView)}.uss";

        const int k_NoTabs = -1;
        int m_ActiveTabIndex = k_NoTabs;

        bool m_Active;

        readonly VisualElement m_Content;
        readonly VisualElement m_Toolbar;
        readonly List<(TextButton button, TabPageComponent tab)> m_TabList;

        public event Action<int> TabSwitched;

        public TabView()
        {
            AddToClassList(UssClassName);
            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_StylePath));

            m_Toolbar = new VisualElement { name = "unity-tab-view-toolbar" };
            m_Toolbar.AddToClassList(ToolbarUssClassName);
            m_Toolbar.AddToClassList(UiConstants.ussDefaultInset);
            hierarchy.Add(m_Toolbar);

            m_Content = new VisualElement { name = "unity-content-container" };
            m_Content.AddToClassList(ContentContainerUssClassName);
            hierarchy.Add(m_Content);

            m_TabList = new List<(TextButton button, TabPageComponent tab)>();
        }

        public void SetActive()
        {
            Assert.IsFalse(m_Active, "TabView is already active.");
            m_Active = true;
            if (m_ActiveTabIndex != k_NoTabs)
            {
                m_TabList[m_ActiveTabIndex].tab.SetActive(true);
            }
        }

        public void SetInactive()
        {
            Assert.IsTrue(m_Active, "TabView is already inactive.");
            m_Active = false;
            if (m_ActiveTabIndex != k_NoTabs)
            {
                m_TabList[m_ActiveTabIndex].tab.SetActive(false);
            }
        }

        /// <summary>
        /// Add a tab to the view.
        /// </summary>
        /// <param name="tabName">Title of the tab.</param>
        /// <param name="tab">Tab content to display.</param>
        public void AddTab(string tabName, TabPageComponent tab)
        {
            // Get the tab index
            var index = m_TabList.Count;

            tab.style.flexGrow = 1;
            tab.style.flexShrink = 1;
            tab.style.flexBasis = new StyleLength(StyleKeyword.Auto);

            // Copy value to avoid modification of the closure scope.
            var indexCopy = index;
            var btn = new TextButton(() => SwitchTabInternal(indexCopy)) { text = tabName };
            btn.AddToClassList(TabHeaderUssClassName);
            m_Toolbar.Add(btn);

            // Add tab to list
            m_TabList.Add((btn, tab));

            // If no currently active tab, switch to this newly added one.
            if (m_ActiveTabIndex == k_NoTabs)
            {
                SwitchToNextTab();
            }
        }

        /// <summary>
        /// Switch to the tab with the given index. Does nothing with an invalid index.
        /// </summary>
        /// <param name="index">Index of the tab to switch to.</param>
        public void SwitchTab(int index)
        {
            // Sanitise index to be passed into the internal switch method.
            if (index == k_NoTabs) return;
            if (index < m_TabList.Count)
            {
                SwitchTabInternal(index);
            }
        }

        /// <summary>
        /// Switch to tab with the given index. Does *NOT* check the validity of the index.
        /// </summary>
        /// <param name="index">Index of the tab to switch to.</param>
        void SwitchTabInternal(int index)
        {
            // Reset tab state of previously active content/button - if there was one.
            if (m_ActiveTabIndex != k_NoTabs)
            {
                m_TabList[m_ActiveTabIndex].tab.RemoveFromHierarchy();
                if (m_Active)
                {
                    m_TabList[m_ActiveTabIndex].tab.SetActive(false);
                }
                m_TabList[m_ActiveTabIndex].button.RemoveFromClassList(UiConstants.ussActive);
            }

            // Set new active tab.
            m_ActiveTabIndex = index;

            // Set tab state for newly active content/button.
            if (m_Active)
            {
                m_TabList[m_ActiveTabIndex].tab.SetActive(true);
            }
            m_TabList[m_ActiveTabIndex].button.AddToClassList(UiConstants.ussActive);
            m_Content.Add(m_TabList[m_ActiveTabIndex].tab);

            TabSwitched?.Invoke(index);
        }

        /// <summary>
        /// Switch to the next valid tab. Wraps if there's no direct successor.
        /// </summary>
        void SwitchToNextTab()
        {
            var index = (m_ActiveTabIndex + 1) % m_TabList.Count;
            SwitchTabInternal(index);
        }

        public override VisualElement contentContainer => m_Content;

        [UsedImplicitly]
        public new class UxmlFactory : UxmlFactory<TabView> { }
    }
}
