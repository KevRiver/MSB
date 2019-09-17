using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to jetpack
	/// Animator parameters : Jetpacking (bool)
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Jetpack")] 
	public class CharacterJetpack : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "Add this component to a character and it'll be able to activate a jetpack and fly through the level. Here you can define the force to apply when jetpacking, the particle system to use, various fuel info, and optionnally what sound to play when the jetpack gets fully refueled"; }

		[Header("Jetpack")]
		/// the duration (in seconds) during which we'll disable the collider when taking off jetpacking from a moving platform
		public float MovingPlatformsJumpCollisionOffDuration=0.05f;
		/// the jetpack associated to the character
		public ParticleSystem ParticleEmitter;		
		/// the force applied by the jetpack
		public float JetpackForce = 2.5f;	

		[Header("Fuel")]
		/// true if the character has unlimited fuel for its jetpack
		public bool JetpackUnlimited = false;
		/// the maximum duration (in seconds) of the jetpack
		public float JetpackFuelDuration = 5f;
		/// the jetpack refuel cooldown
		public float JetpackRefuelCooldown=1f;
		/// the remaining jetpack fuel duration (in seconds)
		public float JetpackFuelDurationLeft{get; protected set;}
		/// the minimum amount of fuel required in the tank to be able to jetpack again
		public float MinimumFuelRequirement = 0.2f;

		[Header("Jetpack Sounds")]
		/// The sound to play when the jetpack is refueled again
		public AudioClip JetpackRefueledSfx; 

		protected bool _stillFuelLeft = true;
		protected bool _refueling = false;
		protected bool _jetpacking = true;
		protected Vector3 _initialPosition;
		protected AudioSource _jetpackUsedSound;
		protected WaitForSeconds _jetpackRefuelCooldownWFS;

		/// <summary>
		/// On Start(), we grab our particle emitter if there's one, and setup our fuel reserves
		/// </summary>
		protected override void Initialization () 
		{
			base.Initialization();
					
			if (ParticleEmitter!=null)
			{
				_initialPosition = ParticleEmitter.transform.localPosition;
				ParticleSystem.EmissionModule emissionModule = ParticleEmitter.emission;
				emissionModule.enabled=false;
			}
			JetpackFuelDurationLeft = JetpackFuelDuration;
			_jetpackRefuelCooldownWFS = new WaitForSeconds (JetpackRefuelCooldown);

            if (GUIManager.Instance!= null && _character.CharacterType == Character.CharacterTypes.Player)
            { 
				GUIManager.Instance.SetJetpackBar(!JetpackUnlimited, _character.PlayerID);
				UpdateJetpackBar();
            }
		}

		/// <summary>
		/// Every frame, we check input to see if we're pressing or releasing the jetpack button
		/// </summary>
		protected override void HandleInput()
		{
			if (_inputManager.JetpackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.JetpackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				JetpackStart();
			}			
			
			if (_inputManager.JetpackButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{
				JetpackStop();
			}
		}
		
		/// <summary>
		/// Causes the character to start its jetpack.
		/// </summary>
		public virtual void JetpackStart()
		{

			if ((!AbilityPermitted) // if the ability is not permitted
				|| (!_stillFuelLeft) // or if there's no fuel left
				|| (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
                || (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
                || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
				|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
			{
				return;
			}				

			// if the jetpack is not unlimited and if we don't have fuel left
			if ((!JetpackUnlimited) && (JetpackFuelDurationLeft <= 0f)) 
			{
				// we stop the jetpack and exit
				JetpackStop();
				_stillFuelLeft = false;
				return;
			}

			// we set the vertical force
			if ((!_controller.State.IsGrounded) || (JetpackForce + _controller.ForcesApplied.y >= 0))
			{
				// if the character is standing on a moving platform and not pressing the down button,
				if ((_controller.State.IsGrounded) && (_controller.State.OnAMovingPlatform))
				{					
					// we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid air
					StartCoroutine (_controller.DisableCollisionsWithMovingPlatforms (MovingPlatformsJumpCollisionOffDuration));
					_controller.DetachFromMovingPlatform ();
				}
				_controller.SetVerticalForce (JetpackForce);
			} 

			// if this is the first time we're here, we trigger our sounds
			if (_movement.CurrentState != CharacterStates.MovementStates.Jetpacking)
			{
				// we play the jetpack start sound 
				PlayAbilityStartSfx();
				PlayAbilityUsedSfx();
				_jetpacking = true;
			}

			// we set the various states
			_movement.ChangeState(CharacterStates.MovementStates.Jetpacking);

			if (ParticleEmitter!=null)
			{
				ParticleSystem.EmissionModule emissionModule = ParticleEmitter.emission;
				emissionModule.enabled=true;
			}

			// if the jetpack is not unlimited, we start burning fuel
			if (!JetpackUnlimited) 
			{
				StartCoroutine (JetpackFuelBurn ());
				
			}
		}
		
		/// <summary>
		/// Causes the character to stop its jetpack.
		/// </summary>
		public virtual void JetpackStop()
		{
			if ((!AbilityPermitted) // if the ability is not permitted
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
                || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging) // or if we're in the ledge hanging state
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
				return;

			TurnJetpackElementsOff ();

			// we set our current state to the previous recorded one
			_movement.RestorePreviousState();
		}

		protected virtual void TurnJetpackElementsOff()
		{
			// we play our stop sound
			if (_movement.CurrentState == CharacterStates.MovementStates.Jetpacking)
			{
				StopAbilityUsedSfx();
				PlayAbilityStopSfx();
			}

			// if we have a jetpack particle emitter, we turn it off
			if (ParticleEmitter!=null)
			{
				ParticleSystem.EmissionModule emissionModule = ParticleEmitter.emission;
				emissionModule.enabled=false;
			}

			// if the jetpack is not unlimited, we start refueling
			if (!JetpackUnlimited)
			{
				StartCoroutine (JetpackRefuel());
			}
			_jetpacking = false;
		}


	    /// <summary>
	    /// Burns the jetpack fuel
	    /// </summary>
	    /// <returns>The fuel burn.</returns>
	    protected virtual IEnumerator JetpackFuelBurn()
		{
			// while the character is jetpacking and while we have fuel left, we decrease the remaining fuel
			float timer=JetpackFuelDurationLeft;
			while ((timer > 0) && (_movement.CurrentState == CharacterStates.MovementStates.Jetpacking) && (JetpackFuelDurationLeft <= timer ))
			{
				timer -= Time.deltaTime;
				JetpackFuelDurationLeft=timer;
				UpdateJetpackBar();
				yield return null;
			}
		}

	    /// <summary>
	    /// Refills the jetpack fuel
	    /// </summary>
	    /// <returns>The fuel refill.</returns>
	    protected virtual IEnumerator JetpackRefuel()
		{
			// we wait for a while before starting to refill
			yield return _jetpackRefuelCooldownWFS;
			_refueling = true;
			// then we progressively refill the jetpack fuel
			float refuelDuration = JetpackFuelDurationLeft;
			while ((refuelDuration < JetpackFuelDuration) && (_movement.CurrentState != CharacterStates.MovementStates.Jetpacking))
			{
				refuelDuration += Time.deltaTime/2;
				JetpackFuelDurationLeft = refuelDuration;
				UpdateJetpackBar();
				// we prevent the character to jetpack again while at low fuel and refueling
				if ((!_stillFuelLeft) && (refuelDuration > MinimumFuelRequirement))
				{
					_stillFuelLeft = true;
				}
				yield return null;
			}
			_refueling = false;
			// if we're full, we play our refueled sound 
			if (System.Math.Abs (JetpackFuelDurationLeft - JetpackFuelDuration) < JetpackFuelDuration/100)
			{
				PlayJetpackRefueledSfx ();
			}
		}	

		/// <summary>
		/// Every frame, we check if our character is colliding with the ceiling. If that's the case we cap its vertical force
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			// if we're not walking anymore, we stop our walking sound
			if (_movement.CurrentState != CharacterStates.MovementStates.Jetpacking && _abilityInProgressSfx != null)
			{
				StopAbilityUsedSfx();
			}

			if (_movement.CurrentState != CharacterStates.MovementStates.Jetpacking && _jetpacking )
			{
				TurnJetpackElementsOff ();
			}

			if (_controller.State.IsCollidingAbove && (_movement.CurrentState != CharacterStates.MovementStates.Jetpacking))
			{
				_controller.SetVerticalForce(0);
			}
		}

		/// <summary>
		/// Updates the GUI jetpack bar.
		/// </summary>
		protected virtual void UpdateJetpackBar()
		{
			if ( (GUIManager.Instance != null) && (_character.CharacterType == Character.CharacterTypes.Player) )
			{
				GUIManager.Instance.UpdateJetpackBar(JetpackFuelDurationLeft,0f,JetpackFuelDuration, _character.PlayerID);
			}
		}

		/// <summary>
		/// Flips the jetpack's emitter horizontally
		/// </summary>
		public override void Flip()
		{
			if (_character == null)
			{
				Initialization ();
			}

			if (ParticleEmitter != null) 
			{
				// we invert the rotation of the particle emitter
				ParticleEmitter.transform.eulerAngles = new Vector3 (ParticleEmitter.transform.eulerAngles.x, ParticleEmitter.transform.eulerAngles.y + 180, ParticleEmitter.transform.eulerAngles.z);	

				// we mirror its position around the transform's center
				if (ParticleEmitter.transform.localPosition == _initialPosition)
				{
					ParticleEmitter.transform.localPosition = Vector3.Scale (_initialPosition, _character.ModelFlipValue);	
				} 
				else 
				{
					ParticleEmitter.transform.localPosition = _initialPosition;	
				}
			}
		}

		/// <summary>
		/// Plays a sound when the jetpack is fully refueled
		/// </summary>
		protected virtual void PlayJetpackRefueledSfx()
		{
			if (JetpackRefueledSfx!=null) {	SoundManager.Instance.PlaySound(JetpackRefueledSfx,transform.position); }
		}	

		/// <summary>
		/// When the character dies we stop its jetpack
		/// </summary>
		public override void Reset()
		{
			// if we have a jetpack particle emitter, we turn it off
			if (ParticleEmitter!=null)
			{
				ParticleSystem.EmissionModule emissionModule = ParticleEmitter.emission;
				emissionModule.enabled=false;
			}
			StopAbilityUsedSfx();
			JetpackFuelDurationLeft = JetpackFuelDuration;
			UpdateJetpackBar();
			_movement.ChangeState (CharacterStates.MovementStates.Idle);
			_stillFuelLeft = true;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Jetpacking", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// At the end of each cycle, we send our character's animator the current jetpacking status
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Jetpacking",(_movement.CurrentState == CharacterStates.MovementStates.Jetpacking),_character._animatorParameters);
		}
	}
}