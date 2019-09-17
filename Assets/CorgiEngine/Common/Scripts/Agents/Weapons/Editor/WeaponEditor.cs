using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

    [CustomEditor(typeof(Weapon), true)]
    [CanEditMultipleObjects]

    /// <summary>
    /// Adds custom labels to the Character inspector
    /// </summary>

    public class WeaponEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Weapon weapon = (Weapon)target;

            // adds movement and condition states
            if (weapon.WeaponState != null)
            {
                EditorGUILayout.LabelField("Weapon State", weapon.WeaponState.CurrentState.ToString());
            }
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

    }
}
