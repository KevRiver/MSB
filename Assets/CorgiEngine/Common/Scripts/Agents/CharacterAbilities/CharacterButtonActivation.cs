using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a character and it'll be able to crouch and crawl
	/// Animator parameters : Activating (bool)
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Button Activation")] 
	public class CharacterButtonActivation : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to interact with button powered objects (dialogue zones, switches...). "; }
		/// true if the character is in a dialogue zone
		public bool InButtonActivatedZone {get;set;}
        /// true if the zone is automated
        public bool InButtonAutoActivatedZone { get; set; }
        /// the current button activated zone
        public ButtonActivated ButtonActivatedZone {get;set;}

		public bool PreventJumpWhenInZone = true;

		protected bool _activating = false;

		/// <summary>
		/// Gets and stores components for further use
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			InButtonActivatedZone=false;
			ButtonActivatedZone=null;
		}

		/// <summary>
		/// Every frame, we check the input to see if we need to pause/unpause the game
		/// </summary>
		protected override void HandleInput()
		{
			if (_inputManager.InteractButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)				
			{
				ButtonActivation();
			}
		}

		/// <summary>
		/// Every frame, we check if we're crouched and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			_activating = false;
		}

		/// <summary>
		/// Tries to activate the button activated zone
		/// </summary>
		protected virtual void ButtonActivation()
		{
			// if the player is in a button activated zone, we handle it
			if ((InButtonActivatedZone)
			    && (ButtonActivatedZone!=null)
				&& (_condition.CurrentState == CharacterStates.CharacterConditions.Normal || _condition.CurrentState == CharacterStates.CharacterConditions.Frozen)
			    && (_movement.CurrentState != CharacterStates.MovementStates.Dashing))
			{
				// if the button can only be activated while grounded and if we're not grounded, we do nothing and exit
				if (ButtonActivatedZone.CanOnlyActivateIfGrounded && !_controller.State.IsGrounded)
				{
					return;
				}
                // if it's an auto activated zone, we do nothing
                if (ButtonActivatedZone.AutoActivation)
                {
                    return;
                }
				// we trigger a character event
				MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.ButtonActivation);

				ButtonActivatedZone.TriggerButtonAction();
				_activating = true;
			}
		}

        /// <summary>
        /// On Death we lose any connection we may have had to a button activated zone
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            InButtonActivatedZone = false;
            ButtonActivatedZone = null;
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter ("Activating", AnimatorControllerParameterType.Bool);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimator.UpdateAnimatorBool(_animator,"Activating", _activating, _character._animatorParameters);	
		}
	}
}
