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
        protected SerializedProperty _abilityStartFeedbacks;
        protected SerializedProperty _abilityStopFeedbacks;

        protected List<String> _propertiesToHide;
        protected bool _hasHiddenProperties = false;

        private void OnEnable()
        {
            _propertiesToHide = new List<string>();

            _abilityStartFeedbacks = this.serializedObject.FindProperty("AbilityStartFeedbacks");
            _abilityStopFeedbacks = this.serializedObject.FindProperty("AbilityStopFeedbacks");

            HiddenPropertiesAttribute[] attributes = (HiddenPropertiesAttribute[])target.GetType().GetCustomAttributes(typeof(HiddenPropertiesAttribute), false);
            if (attributes != null)
            {
                if (attributes.Length != 0)
                {
                    if (attributes[0].PropertiesNames != null)
                    {
                        _propertiesToHide = new List<String>(attributes[0].PropertiesNames);                        
                        _hasHiddenProperties = true;
                    }
                }                
            }
        }
        
		/// <summary>
		/// When inspecting a Character, adds to the regular inspector some labels, useful for debugging
		/// </summary>
		public override void OnInspectorGUI()
		{
			CharacterAbility t = (target as CharacterAbility);

			serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            if (t.HelpBoxText() != "")
			{
				EditorGUILayout.HelpBox(t.HelpBoxText(),MessageType.Info);
			}

			Editor.DrawPropertiesExcluding(serializedObject, new string[] { "AbilityStartFeedbacks", "AbilityStopFeedbacks" });

			EditorGUILayout.Space();
                        
            if (_propertiesToHide.Count > 0)
            {
                if (_propertiesToHide.Count < 2)
                {
                    EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);
                }                
                if (!_propertiesToHide.Contains("AbilityStartFeedbacks"))
                {
                    EditorGUILayout.PropertyField(_abilityStartFeedbacks);
                }
                if (!_propertiesToHide.Contains("AbilityStopFeedbacks"))
                {
                    EditorGUILayout.PropertyField(_abilityStopFeedbacks);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_abilityStartFeedbacks);
                EditorGUILayout.PropertyField(_abilityStopFeedbacks);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }                
        }	
	}
}