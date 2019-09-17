using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Swim")]

    /// <summary>
    /// Add this ability to a Character to allow it to swim in Water by pressing the Swim button (by default the same binding as the Jump button, but separated for convenience)
    /// 
    /// Animator parameters : Swimming (bool), SwimmingIdle (bool)
    /// </summary>
    public class CharacterSwim : CharacterAbility
    {
        public override string HelpBoxText() { return "This component allows a Character to swim in Water by pressing the Swim button (by default the same binding as the Jump button, but separated for convenience). Here you can define the swim force to apply, the duration of the associated animation, as well as VFX to instantiate when entering/exiting the water, and the force to apply when exiting."; }
        [ReadOnly]
        /// whether or not the character is in water
        public bool InWater = false;

        [Header("Swim")]
        /// defines how high the character can jump
		public float SwimHeight = 3.025f;
        /// the duration (in seconds) of the swim animation before it reverts back to swim idle
        public float SwimAnimationDuration = 0.8f;

        [Header("Splash Effects")]
        /// the effect that will be instantiated everytime the character enters the water
        public GameObject WaterEntryEffect;
        /// the effect that will be instantiated everytime the character exits the water
        public GameObject WaterExitEffect;
        /// the force to apply to the character when exiting water
        public Vector2 WaterExitForce = new Vector2(0f, 12f);

        protected float _swimDurationLeft = 0f;

        /// <summary>
        /// On Update we decrease our counter
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            _swimDurationLeft -= Time.deltaTime;
        }

        /// <summary>
		/// At the beginning of each cycle we check if we've just pressed or released the swim button
		/// </summary>
		protected override void HandleInput()
        {
            if (!InWater)
            {
                return;
            }

            if (_inputManager.SwimButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                Swim();
            }
        }

        /// <summary>
        /// When swimming we apply our swim force
        /// </summary>
        protected virtual void Swim()
        {
            _movement.ChangeState(CharacterStates.MovementStates.SwimmingIdle);
            _controller.SetVerticalForce(Mathf.Sqrt(2f * SwimHeight * Mathf.Abs(_controller.Parameters.Gravity)));
            _swimDurationLeft = SwimAnimationDuration;
        }

        /// <summary>
        /// When entering the water we instantiate a splash if needed and change our state
        /// </summary>
        public virtual void EnterWater()
        {
            InWater = true;
            _movement.ChangeState(CharacterStates.MovementStates.SwimmingIdle);
            if (WaterEntryEffect != null)
            {
                Instantiate(WaterEntryEffect, this.transform.position, Quaternion.identity);
            }            
        }

        /// <summary>
        /// When exiting the water we instantiate a splash if needed and change our state
        /// </summary>
        public virtual void ExitWater()
        {
            InWater = false;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _controller.SetForce(WaterExitForce);

            if (WaterExitEffect != null)
            {
                Instantiate(WaterExitEffect, this.transform.position, Quaternion.identity);
            }            
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter("Swimming", AnimatorControllerParameterType.Bool);
            RegisterAnimatorParameter("SwimmingIdle", AnimatorControllerParameterType.Bool);
        }

        /// <summary>
        /// At the end of each cycle, we send our Running status to the character's animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimator.UpdateAnimatorBool(_animator, "Swimming", (_swimDurationLeft > 0f), _character._animatorParameters);
            MMAnimator.UpdateAnimatorBool(_animator, "SwimmingIdle", (_movement.CurrentState == CharacterStates.MovementStates.SwimmingIdle), _character._animatorParameters);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            InWater = false;
        }
    }
}
