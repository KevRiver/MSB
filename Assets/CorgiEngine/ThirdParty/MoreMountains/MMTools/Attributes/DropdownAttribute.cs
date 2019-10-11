using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    public class DropdownAttribute : PropertyAttribute
    {
        public readonly object[] DropdownValues;

        public DropdownAttribute(params object[] dropdownValues)
        {
            DropdownValues = dropdownValues;
        }
    }
}
