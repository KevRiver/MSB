using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MoreMountains.Tools
{
    public enum BackgroundAttributeColor
    {
        Red,
        Pink,
        Orange,
        Yellow,
        Green,
        Blue,
        Violet,
        White
    }

    public class BackgroundColorAttribute : PropertyAttribute
    {
        public BackgroundAttributeColor Color;

        public BackgroundColorAttribute(BackgroundAttributeColor color = BackgroundAttributeColor.Yellow)
        {
            this.Color = color;
        }
    }
}
