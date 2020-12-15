using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Collaborate.Utilities
{
    static class MenuUtilities
    {
        /// <summary>
        /// Corner of the anchor element the dialogue should anchor to.
        /// </summary>
        public enum AnchorPoint
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        /// <summary>
        /// Direction the dialogue should open from its anchor.
        /// </summary>
        public enum OpenDirection
        {
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        }

        /// <summary>
        /// Given an element and an anchor point, calculate the world coords to draw a menu at.
        /// </summary>
        /// <param name="e">Element to start at.</param>
        /// <param name="anchorPoint">Corner of the element to calculate.</param>
        /// <returns>World coordinates from the given values.</returns>
        public static (float X, float Y) GetMenuPosition(VisualElement e, AnchorPoint anchorPoint)
        {
            // Calculate position of the start corner.
            (float x, float y) anchorCoords;
            switch (anchorPoint)
            {
                case AnchorPoint.TopLeft:
                    anchorCoords = (e.worldBound.xMin, e.worldBound.yMin);
                    break;
                case AnchorPoint.TopRight:
                    anchorCoords = (e.worldBound.xMax, e.worldBound.yMin);
                    break;
                case AnchorPoint.BottomLeft:
                    anchorCoords = (e.worldBound.xMin, e.worldBound.yMax);
                    break;
                case AnchorPoint.BottomRight:
                    anchorCoords = (e.worldBound.xMax, e.worldBound.yMax);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchorPoint), anchorPoint, null);
            }

            return anchorCoords;
        }
    }
}
