using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MoreMountains.Tools
{
    public class ColorAttribute : PropertyAttribute
    {
        public Color color;

        public ColorAttribute(float red = 1, float green = 0, float blue = 0)
        {
            this.color = new Color(red, green, blue, 1);
        }
    }
}
