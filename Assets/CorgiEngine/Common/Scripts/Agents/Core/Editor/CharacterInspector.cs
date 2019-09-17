using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

	[CustomEditor (typeof(Character))]
	[CanEditMultipleObjects]

	/// <summary>
	/// Adds custom labels to the Character inspector
	/// </summary>

	public class CharacterInspector : Editor 
	{		
		void onEnable()
		{
			// nothing
		}
		
		/// <summary>
		/// When inspecting a Character, adds to the regular inspector some labels, useful for debugging
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Character character = (Character)target;


			// adds movement and condition states
			if (character.CharacterState!=null)
			{
				EditorGUILayout.LabelField("Movement State",character.MovementState.CurrentState.ToString());
				EditorGUILayout.LabelField("Condition State",character.ConditionState.CurrentState.ToString());
			}

			// auto completes the animator
			if (character.CharacterAnimator == null)
			{
				if (character.GetComponent<Animator>() != null)
				{
					character.CharacterAnimator = character.GetComponent<Animator>();
				}
			}

			// draws the default inspector if in Player mode
			if (character.CharacterType == Character.CharacterTypes.Player)
			{
				DrawDefaultInspector();
			}

			// in AI mode draws everything but the PlayerID field
			if (character.CharacterType == Character.CharacterTypes.AI)
			{
				character.PlayerID = "";
				Editor.DrawPropertiesExcluding(serializedObject, new string[] { "PlayerID" });
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Autobuild", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("The Character Autobuild button will automatically add all the components needed for a functioning Character, and set their settings, layer, tags. Be careful, if you've already customized your character, this will reset its settings!", MessageType.Warning, true);
			if(GUILayout.Button("AutoBuild Player Character"))
	        {
				GenerateCharacter(Character.CharacterTypes.Player);
	        }
			if(GUILayout.Button("AutoBuild AI Character"))
	        {
				GenerateCharacter(Character.CharacterTypes.AI);
	        }
				
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Adds all the possible components to a character
		/// </summary>
		protected virtual void GenerateCharacter(Character.CharacterTypes type)
		{
			Character character = (Character)target;

			Debug.LogFormat(character.name + " : Character Autobuild Start");

			if (type == Character.CharacterTypes.Player)
			{
				character.CharacterType = Character.CharacterTypes.Player;
				// sets the layer
				character.gameObject.layer = LayerMask.NameToLayer("Player");
				// sets the tag
				character.gameObject.tag = "Player";
				// sets the player ID
				character.PlayerID = "Player1";
			}

			if (type == Character.CharacterTypes.AI)
			{
				character.CharacterType = Character.CharacterTypes.AI;
				// sets the layer
				character.gameObject.layer = LayerMask.NameToLayer("Enemies");
			}

			// Adds the rigidbody2D
			Rigidbody2D rigidbody2D = (character.GetComponent<Rigidbody2D>() == null) ? character.gameObject.AddComponent<Rigidbody2D>() : character.GetComponent<Rigidbody2D>();
			rigidbody2D.useAutoMass = false;
			rigidbody2D.mass = 1;
			rigidbody2D.drag = 0;
			rigidbody2D.angularDrag = 0.05f;
			rigidbody2D.gravityScale = 1;
			rigidbody2D.interpolation = RigidbodyInterpolation2D.None;
			rigidbody2D.sleepMode = RigidbodySleepMode2D.StartAwake;
			rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
			rigidbody2D.isKinematic = true;

			// Adds the boxcollider 2D
			BoxCollider2D boxcollider2D = (character.GetComponent<BoxCollider2D>() == null) ? character.gameObject.AddComponent<BoxCollider2D>() : character.GetComponent<BoxCollider2D>();
			boxcollider2D.isTrigger = true;

			// Adds the Corgi Controller
			CorgiController corgiController = (character.GetComponent<CorgiController>() == null) ? character.gameObject.AddComponent<CorgiController>() : character.GetComponent<CorgiController>();
			corgiController.PlatformMask = LayerMask.GetMask("Platforms");
			corgiController.MovingPlatformMask = LayerMask.GetMask("MovingPlatforms");
			corgiController.OneWayPlatformMask = LayerMask.GetMask("OneWayPlatforms");
            corgiController.MovingOneWayPlatformMask = LayerMask.GetMask("MovingOneWayPlatforms");
            corgiController.MidHeightOneWayPlatformMask = LayerMask.GetMask("MidHeightOneWayPlatforms");

            if (character.GetComponent<Health>() == null) { character.gameObject.AddComponent<Health>(); } 

			if (character.GetComponent<CharacterLevelBounds>() == null) { character.gameObject.AddComponent<CharacterLevelBounds>(); } 
			if (character.GetComponent<CharacterHorizontalMovement>() == null) { character.gameObject.AddComponent<CharacterHorizontalMovement>(); } 
			if (character.GetComponent<CharacterCrouch>() == null) { character.gameObject.AddComponent<CharacterCrouch>(); } 
			if (character.GetComponent<CharacterDash>() == null) { character.gameObject.AddComponent<CharacterDash>(); } 
			if (character.GetComponent<CharacterDive>() == null) { character.gameObject.AddComponent<CharacterDive>(); } 
			if (character.GetComponent<CharacterDangling>() == null) { character.gameObject.AddComponent<CharacterDangling>(); } 
			if (character.GetComponent<CharacterJump>() == null) { character.gameObject.AddComponent<CharacterJump>(); } 
			if (character.GetComponent<CharacterRun>() == null) { character.gameObject.AddComponent<CharacterRun>(); } 
			if (character.GetComponent<CharacterJetpack>() == null) { character.gameObject.AddComponent<CharacterJetpack>(); } 
			if (character.GetComponent<CharacterLookUp>() == null) { character.gameObject.AddComponent<CharacterLookUp>(); } 
			if (character.GetComponent<CharacterGrip>() == null) { character.gameObject.AddComponent<CharacterGrip>(); } 
			if (character.GetComponent<CharacterWallClinging>() == null) { character.gameObject.AddComponent<CharacterWallClinging>(); } 
			if (character.GetComponent<CharacterWalljump>() == null) { character.gameObject.AddComponent<CharacterWalljump>(); } 
			if (character.GetComponent<CharacterLadder>() == null) { character.gameObject.AddComponent<CharacterLadder>(); } 
			if (character.GetComponent<CharacterPause>() == null) { character.gameObject.AddComponent<CharacterPause>(); } 
			if (character.GetComponent<CharacterButtonActivation>() == null) { character.gameObject.AddComponent<CharacterButtonActivation>(); } 
			if (character.GetComponent<CharacterHandleWeapon>() == null) { character.gameObject.AddComponent<CharacterHandleWeapon>(); } 

			if (type == Character.CharacterTypes.AI)
			{
				// Adds Damage on touch
				DamageOnTouch damageOnTouch = (character.GetComponent<DamageOnTouch>() == null) ? character.gameObject.AddComponent<DamageOnTouch>() : character.GetComponent<DamageOnTouch>();
				damageOnTouch.TargetLayerMask = LayerMask.GetMask("Player");

				if (character.GetComponent<AIWalk>() == null) { character.gameObject.AddComponent<AIWalk>(); } 
			}

			Debug.LogFormat(character.name + " : Character Autobuild Complete");
		}
	}
}