using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this ability to a Character and it'll be able to glide through the air, slowing its fall when pressing the Glide button (by default the same binding as the Jump button, but separated for convenience)
    /// 
	/// Animator parameters : Gliding (bool)
    /// </summary>
    public class CharacterGlide : CharacterAbility
    {
        public override string HelpBoxText() { return "This component allows a Character to glide through the air, slowing its fall when pressing the Glide button (by default the same binding as the Jump button, but separated for convenience). Here you can define the force to apply to slow down the fall, and whether or not the Glide should wait for the Character to have exhausted all its jumps (otherwise it'll take priority over any jump after the first)."; }

        /// the force to apply when gliding
        public float VerticalForce = 0.1f;
        /// whether or not the glide will wait for jumps to be exhausted
        public bool GlideOnlyIfNoJumpsLeft = true;

        protected bool _gliding;
        protected CharacterJump _characterJump;
        protected CharacterWalljump _characterWallJump;
        protected CharacterSwim _characterSwim;

        /// <summary>
        /// On Start we grab our components
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _characterJump = this.gameObject.GetComponentNoAlloc<CharacterJump>();
            _characterWallJump = this.gameObject.GetComponentNoAlloc<CharacterWalljump>();
            _characterSwim = this.gameObject.GetComponentNoAlloc<CharacterSwim>();
        }

        /// <summary>
        /// Looks for glide related inputs
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            if (_inputManager.GlideButton.State.CurrentState == MMInput.ButtonStates.ButtonDown )
            {
                GlideStart();
            }

            if (_inputManager.GlideButton.State.CurrentState == MMInput.ButtonStates.ButtonUp && _gliding)
            {
                GlideStop();
            }
        }

        /// <summary>
        /// When pressing the glide button we make sure we can glide, and initiate it
        /// </summary>
        protected virtual void GlideStart()
        {
            if ((!AbilityPermitted) // if the ability is not permitted
                || (_controller.State.IsGrounded) // or if we're on the ground
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing) // or if we're dashing
                || (_movement.CurrentState == CharacterStates.MovementStates.WallClinging) // or if we're wallclinging
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
            {
                return;
            }

            if (_characterSwim != null)
            {
                if (_characterSwim.InWater)
                {
                    return;
                }                
            }

            // if we're walljumping, we prevent the character from gliding
            if (_characterWallJump != null)
            {
                if ((_movement.CurrentState == CharacterStates.MovementStates.WallJumping) && _characterWallJump.WallJumpHappenedThisFrame)
                {
                    return;
                }
            }

            // if we want to wait for the character to not have any jumps left, and if conditions are met, we prevent it from gliding
            if (GlideOnlyIfNoJumpsLeft 
                && (_characterJump != null))
            { 
                if ((_characterJump.NumberOfJumpsLeft > 0) || (_characterJump.JumpHappenedThisFrame))
                {
                    return;
                }
            }

            // if this is the first time we're here, we trigger our sounds
            if (_movement.CurrentState != CharacterStates.MovementStates.Gliding)
            {
                // we play the gliding start sound 
                PlayAbilityStartSfx();
                PlayAbilityUsedSfx();
                _gliding = true;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Gliding);
        }

        /// <summary>
        /// Stops the character from gliding
        /// </summary>
        protected virtual void GlideStop()
        {           
            // we play our stop sound
            if (_movement.CurrentState == CharacterStates.MovementStates.Gliding)
            {
                StopAbilityUsedSfx();
                PlayAbilityStopSfx();
            }
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _gliding = false;
        }
    
        /// <summary>
        /// Stops the character from gliding if needed
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            // if we're not gliding anymore, we stop our walking sound
            if (_movement.CurrentState != CharacterStates.MovementStates.Gliding && _abilityInProgressSfx != null)
            {
                StopAbilityUsedSfx();
            }

            // if we're not in the gliding state anymore
            if (_movement.CurrentState != CharacterStates.MovementStates.Gliding && _gliding)
            {
                GlideStop();
            }

            // if we're touching the ground
            if (_controller.State.IsCollidingBelow && _gliding)
            {
                GlideStop();
            }

            // if we're colliding with something above (which shouldn't happen for regular glides but can happen when applying high forces)
            if (_controller.State.IsCollidingAbove && (_movement.CurrentState != CharacterStates.MovementStates.Gliding))
            {
                _controller.SetVerticalForce(0);
            }

            // if we're gliding, we apply our force
            if (_gliding)
            {
                _controller.SetVerticalForce(VerticalForce);
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter("Gliding", AnimatorControllerParameterType.Bool);
        }

        /// <summary>
        /// At the end of each cycle, we send our character's animator the current gliding status
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimator.UpdateAnimatorBool(_animator, "Gliding", (_movement.CurrentState == CharacterStates.MovementStates.Gliding), _character._animatorParameters);
        }
    }
}
