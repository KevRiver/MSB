using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

	[CustomEditor (typeof(CharacterAbility),true)]
	[CanEditMultipleObjects]

	/// <summary>
	/// Adds custom labels to the Character inspector
	/// </summary>

	public class CharacterAbilityInspector : Editor 
	{		
		SerializedProperty AbilityStartSfx, AbilityInProgressSfx, AbilityStopSfx;

		protected bool _foldout;

		protected virtual void OnEnable()
		{
			AbilityStartSfx = this.serializedObject.FindProperty("AbilityStartSfx");
			AbilityInProgressSfx = this.serializedObject.FindProperty("AbilityInProgressSfx");
			AbilityStopSfx = this.serializedObject.FindProperty("AbilityStopSfx");
		}

		/// <summary>
		/// When inspecting a Character, adds to the regular inspector some labels, useful for debugging
		/// </summary>
		public override void OnInspectorGUI()
		{
			CharacterAbility t = (target as CharacterAbility);

			serializedObject.Update();

			if (t.HelpBoxText() != "")
			{
				EditorGUILayout.HelpBox(t.HelpBoxText(),MessageType.Info);
			}

			Editor.DrawPropertiesExcluding(serializedObject, new string[] { "AbilityStartSfx","AbilityInProgressSfx","AbilityStopSfx" });

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck ();
	        EditorGUILayout.GetControlRect (true, 16f, EditorStyles.foldout);
	        Rect foldRect = GUILayoutUtility.GetLastRect ();
	        if (Event.current.type == EventType.MouseUp && foldRect.Contains (Event.current.mousePosition)) 
	        {
	            _foldout = !_foldout;
	            GUI.changed = true;
	            Event.current.Use ();
	        }
	        _foldout = EditorGUI.Foldout (foldRect, _foldout, "Ability Sounds");	      

	        if (_foldout) 
	        {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(AbilityStartSfx);
				EditorGUILayout.PropertyField(AbilityInProgressSfx);
				EditorGUILayout.PropertyField(AbilityStopSfx);
		        EditorGUI.indentLevel--;
	         }

			serializedObject.ApplyModifiedProperties();
		}	
	}
}