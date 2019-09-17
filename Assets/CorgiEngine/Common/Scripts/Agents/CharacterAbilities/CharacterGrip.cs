using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to grip level elements that have the Grip component
	/// Animator parameters : Gripping (bool)
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Grip")] 
	public class CharacterGrip : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "Add this component to a character and it'll be able to grip level elements that have the Grip component."; }
		/// The duration (in seconds) during which a character can't grip again after exiting a grip
		public float BufferDurationAfterGrip = 0.3f;
		/// Returns true if the character can grip right now, false otherwise
		public bool CanGrip { get { return (Time.time - _lastGripTimestamp > BufferDurationAfterGrip); }}

		protected CharacterJump _characterJump;
		protected float _lastGripTimestamp = 0f;
		protected Grip _gripTarget; 

		/// <summary>
		/// On Start() we grab our character jump component
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_characterJump = GetComponent<CharacterJump>();
		}

		/// <summary>
		/// Every frame we check to see if we should be gripping
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			Grip();
			Detach ();
		}

		/// <summary>
		/// A public method to have the character grip the specified target
		/// </summary>
		/// <param name="gripTarget">Grip target.</param>
		public virtual void StartGripping(Grip gripTarget)
		{
			if (!CanGrip) { return;	}

			_gripTarget = gripTarget;
			_movement.ChangeState (CharacterStates.MovementStates.Gripping);
		}

		/// <summary>
		/// Called at update, handles gripping to Grip components (ropes, etc)
		/// </summary>
		protected virtual void Grip()
		{
			// if we're gripping to something, we disable the gravity
			if (_movement.CurrentState == CharacterStates.MovementStates.Gripping)
			{	
				_controller.SetForce(Vector2.zero);
				_controller.GravityActive(false);		
				if (_characterJump != null)
				{
					_characterJump.ResetNumberOfJumps();
				}
				_controller.transform.position = _gripTarget.transform.position + _gripTarget.GripOffset;
			}
		}	

		/// <summary>
		/// Checks whether we should stop gripping or not
		/// </summary>
		protected virtual void Detach()
		{
			if ((_movement.CurrentState != CharacterStates.MovementStates.Gripping) && (_movement.PreviousState == CharacterStates.MovementStates.Gripping))
			{
				_lastGripTimestamp = Time.time;
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Gripping", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// At the end of each cycle, we send our character's animator the current gripping status
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Gripping",(_movement.CurrentState == CharacterStates.MovementStates.Gripping),_character._animatorParameters);
		}	
	}
}