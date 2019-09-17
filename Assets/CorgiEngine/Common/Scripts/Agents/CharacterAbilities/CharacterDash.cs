using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a character and it'll be able to perform a horizontal dash
	/// Animator parameters : Dashing
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Dash")] 
	public class CharacterDash : CharacterAbility
	{		
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to dash. Here you can define the distance the dash should cover, how much force to apply, and the cooldown between the end of a dash and the start of the next one."; }

		[Header("Dash")]
		/// the duration of dash (in seconds)
		public float DashDistance = 3f;
		/// the force of the dash
		public float DashForce = 40f;	
		/// the duration of the cooldown between 2 dashes (in seconds)
		public float DashCooldown = 1f;	

		protected float _cooldownTimeStamp = 0;

		protected float _startTime ;
		protected Vector3 _initialPosition ;
		protected float _dashDirection;
		protected float _distanceTraveled = 0f;
		protected bool _shouldKeepDashing = true;
		protected float _computedDashForce;
		protected float _slopeAngleSave = 0f;
		protected bool _dashEndedNaturally = true;
        protected IEnumerator _dashCoroutine;
        
		/// <summary>
		/// At the start of each cycle, we check if we're pressing the dash button. If we
		/// </summary>
		protected override void HandleInput()
		{
			if (_inputManager.ActiveSkillButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				StartDash();
			}
		}

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			// If the character is dashing, we cancel the gravity
			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing) 
			{
				_controller.GravityActive(false);
				_controller.SetVerticalForce(0);
			}
			// we reset our slope tolerance if dash didn't end naturally
			if ((!_dashEndedNaturally) && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
			{
				_dashEndedNaturally = true;
				_controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
			}
		}

		/// <summary>
		/// Causes the character to dash or dive (depending on the vertical movement at the start of the dash)
		/// </summary>
		public virtual void StartDash()
		{
			// if the Dash action is enabled in the permissions, we continue, if not we do nothing
			if (!AbilityPermitted 
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.LedgeHanging)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
				return;		
			
			// If the user presses the dash button and is not aiming down
			if (_verticalInput > -_inputManager.Threshold.y) 
			{	
				// if the character is allowed to dash
				if (_cooldownTimeStamp <= Time.time)
				{
					InitiateDash ();
				}			
			}			
		}

		public virtual void InitiateDash()
		{
			// we set its dashing state to true
			_movement.ChangeState(CharacterStates.MovementStates.Dashing);

			// we start our sounds
			PlayAbilityStartSfx();
			PlayAbilityUsedSfx();

			_cooldownTimeStamp = Time.time + DashCooldown;
            // we launch the boost corountine with the right parameters
            _dashCoroutine = Dash();
            StartCoroutine(_dashCoroutine);
		}

		/// <summary>
		/// Coroutine used to move the player in a direction over time
		/// </summary>
		protected virtual IEnumerator Dash()
        {
            // if the character is not in a position where it can move freely, we do nothing.
            if ( !AbilityPermitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
			{
				yield break;
            }

            // we initialize our various counters and checks
            _startTime = Time.time;
			_dashEndedNaturally = false;
			_initialPosition = this.transform.position;
			_distanceTraveled = 0;
			_shouldKeepDashing = true;
			_dashDirection = _character.IsFacingRight ? 1f : -1f;
			_computedDashForce = DashForce * _dashDirection;

			// we prevent our character from going through slopes
			_slopeAngleSave = _controller.Parameters.MaximumSlopeAngle;
			_controller.Parameters.MaximumSlopeAngle = 0;

			// we keep dashing until we've reached our target distance or until we get interrupted
			while (_distanceTraveled < DashDistance && _shouldKeepDashing && _movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                _distanceTraveled = Vector3.Distance(_initialPosition,this.transform.position);

				// if we collide with something on our left or right (wall, slope), we stop dashing, otherwise we apply horizontal force
				if ((_controller.State.IsCollidingLeft || _controller.State.IsCollidingRight))
				{
					_shouldKeepDashing = false;
					_controller.SetForce (Vector2.zero);
				}
				else
				{
					_controller.GravityActive(true);
					_controller.SetVerticalForce(0);
                    _controller.SetHorizontalForce(_computedDashForce);
				}
				yield return null;
			}

            StopDash();				
		}

        public virtual void StopDash()
        {
            StopCoroutine(_dashCoroutine);

            // once our dash is complete, we reset our various states

            _controller.DefaultParameters.MaximumSlopeAngle = _slopeAngleSave;
            _controller.Parameters.MaximumSlopeAngle = _slopeAngleSave;
            _controller.GravityActive(true);
            _dashEndedNaturally = true;

            // we play our exit sound
            StopAbilityUsedSfx();
            PlayAbilityStopSfx();

            // once the boost is complete, if we were dashing, we make it stop and start the dash cooldown
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                _movement.RestorePreviousState();
            }
        }

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter("Dashing", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// At the end of the cycle, we update our animator's Dashing state 
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Dashing",(_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters);
		}

	}
}