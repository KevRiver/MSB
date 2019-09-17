using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(Collider2D))]
	/// <summary>
	/// Extend this class to activate something when a button is pressed in a certain zone
	/// </summary>
	public class ButtonActivated : MonoBehaviour 
	{
        [Header("Activation Conditions")]
        [Information("Here you can specific how that zone is interacted with. Does it require the ButtonActivation character ability? Can it only be interacted with by the Player? Does it require a button press? Can it only be activated while standing on the ground?", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// if this is true, this zone can only be activated if the character has the required ability
        public bool RequiresButtonActivationAbility = true;
        /// if this is true, this can only be activated by player Characters
        public bool RequiresPlayerCharacter = true;
        /// if true, the zone will activate whether the button is pressed or not
        public bool AutoActivation = false;
        /// Set this to true if you want the CharacterBehaviorState to be notified of the player's entry into the zone.
        public bool ShouldUpdateState = true;
        /// if this is set to false, the zone won't be activable while not grounded
        public bool CanOnlyActivateIfGrounded = false;

        [Header("Number of Activations")]
        [Information("You can decide to have that zone be interactable forever, or just a limited number of times, and can specify a delay between uses (in seconds).", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// if this is set to false, your number of activations will be MaxNumberOfActivations
        public bool UnimitedActivations = true;
        /// the number of times the zone can be interacted with
        public int MaxNumberOfActivations = 0;
        public float DelayBetweenUses = 0f;

        [Header("Visual Prompt")]
        [Information("You can have this zone show a visual prompt to indicate to the player that it's interactable.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// the gameobject to instantiate to present the prompt
        public ButtonPrompt ButtonPromptPrefab;
        /// the text to display in the button prompt
        public string ButtonPromptText = "A";
        /// the text to display in the button prompt
        public Color ButtonPromptColor = MoreMountains.Tools.Colors.LawnGreen;
        /// the color for the prompt's text
        public Color ButtonPromptTextColor = MoreMountains.Tools.Colors.White;
        /// If true, the "buttonA" prompt will always be shown, whether the player is in the zone or not.
        public bool AlwaysShowPrompt = true;
        /// If true, the "buttonA" prompt will be shown when a player is colliding with the zone
        public bool ShowPromptWhenColliding = true;
        /// If true, the prompt will hide after use
        public bool HidePromptAfterUse = false;
        /// the position of the actual buttonA prompt relative to the object's center
        public Vector3 PromptRelativePosition = Vector3.zero;

        protected Animator _buttonPromptAnimator;
        protected ButtonPrompt _buttonPrompt;
        protected Collider2D _collider;
        protected bool _promptHiddenForever = false;
        protected CharacterButtonActivation _characterButtonActivation;
        protected int _numberOfActivationsLeft;
        protected float _lastActivationTimestamp;

        protected Character _currentCharacter;

        /// <summary>
        /// On Enable, we initialize our ButtonActivated zone
        /// </summary>
        protected virtual void OnEnable()
        {
            Initialization();
        }

        /// <summary>
        /// Grabs components and shows prompt if needed
        /// </summary>
		public virtual void Initialization()
        {
            _collider = this.gameObject.GetComponent<Collider2D>();
            _numberOfActivationsLeft = MaxNumberOfActivations;

            if (AlwaysShowPrompt)
            {
                ShowPrompt();
            }
        }

        /// <summary>
        /// When the input button is pressed, we check whether or not the zone can be activated, and if yes, trigger ZoneActivated
        /// </summary>
        public virtual void TriggerButtonAction()
        {
            if (!CheckNumberOfUses())
            {
                PromptError();
                return;
            }


            // if we can only activate this zone when grounded, we check if we have a controller and if it's not grounded,
            // we do nothing and exit
            if (CanOnlyActivateIfGrounded)
            {
                if (_currentCharacter != null)
                {
                    CorgiController controller = _currentCharacter.gameObject.GetComponentNoAlloc<CorgiController>();
                    if (controller != null)
                    {
                        if (!controller.State.IsGrounded)
                        {
                            return;
                        }
                    }
                }
            }

            ActivateZone();
        }

        /// <summary>
        /// Activates the zone
        /// </summary>
		protected virtual void ActivateZone()
        {
            _lastActivationTimestamp = Time.time;
            if (HidePromptAfterUse)
            {
                _promptHiddenForever = true;
                HidePrompt();
            }
            _numberOfActivationsLeft--;
        }

        /// <summary>
        /// Triggers an error 
        /// </summary>
        public virtual void PromptError()
        {
            if (_buttonPromptAnimator != null)
            {
                _buttonPromptAnimator.SetTrigger("Error");
            }
        }

        /// <summary>
        /// Shows the button A prompt.
        /// </summary>
        public virtual void ShowPrompt()
        {
            if ((_promptHiddenForever) || (ButtonPromptPrefab == null))
            {
                return;
            }

            // we add a blinking A prompt to the top of the zone
            if (_buttonPrompt == null)
            {
                _buttonPrompt = (ButtonPrompt)Instantiate(ButtonPromptPrefab);
                _buttonPromptAnimator = _buttonPrompt.gameObject.GetComponentNoAlloc<Animator>();
            }

            if (_collider != null)
            {
                _buttonPrompt.transform.position = _collider.bounds.center + PromptRelativePosition;
            }
            _buttonPrompt.gameObject.SetActive(true);
            _buttonPrompt.transform.parent = transform;
            _buttonPrompt.SetText(ButtonPromptText);
            _buttonPrompt.SetBackgroundColor(ButtonPromptColor);
            _buttonPrompt.SetTextColor(ButtonPromptTextColor);
            _buttonPrompt.Show();
        }

        /// <summary>
        /// Hides the button A prompt.
        /// </summary>
        public virtual void HidePrompt()
        {
            if (_buttonPrompt != null)
            {
                _buttonPrompt.gameObject.SetActive(true);
                _buttonPrompt.Hide();
            }            
        }

        /// <summary>
        /// Handles enter collision with 2D triggers
        /// </summary>
        /// <param name="collidingObject">Colliding object.</param>
        protected virtual void OnTriggerEnter2D(Collider2D collidingObject)
        {
            TriggerEnter(collidingObject.gameObject);
        }
        /// <summary>
        /// Handles enter collision with 2D triggers
        /// </summary>
        /// <param name="collidingObject">Colliding object.</param>
        protected virtual void OnTriggerExit2D(Collider2D collidingObject)
        {
            TriggerExit(collidingObject.gameObject);
        }

        /// <summary>
        /// Triggered when something collides with the button activated zone
        /// </summary>
        /// <param name="collider">Something colliding with the water.</param>
        protected virtual void TriggerEnter(GameObject collider)
        {
            _currentCharacter = collider.gameObject.GetComponentNoAlloc<Character>();
            CharacterButtonActivation characterButtonActivation = collider.gameObject.GetComponentNoAlloc<CharacterButtonActivation>();

            if (!CheckConditions(_currentCharacter, characterButtonActivation))
            {
                return;
            }


            if (ShouldUpdateState)
            {
                _characterButtonActivation = characterButtonActivation;
                if (characterButtonActivation != null)
                {
                    characterButtonActivation.InButtonActivatedZone = true;
                    characterButtonActivation.ButtonActivatedZone = this;
                    characterButtonActivation.InButtonAutoActivatedZone = AutoActivation;
                }
            }

            if (AutoActivation)
            {
                TriggerButtonAction();
            }

            // if we're not already showing the prompt and if the zone can be activated, we show it
            if (ShowPromptWhenColliding)
            {
                ShowPrompt();
            }
        }

        /// <summary>
        /// Triggered when something exits the water
        /// </summary>
        /// <param name="collider">Something colliding with the dialogue zone.</param>
        protected virtual void TriggerExit(GameObject collider)
        {
            // we check that the object colliding with the water is actually a TopDown controller and a character
            Character character = collider.gameObject.GetComponentNoAlloc<Character>();
            CharacterButtonActivation characterButtonActivation = collider.gameObject.GetComponentNoAlloc<CharacterButtonActivation>();

            if (!CheckConditions(character, characterButtonActivation))
            {
                return;
            }

            if (ShouldUpdateState)
            {
                _characterButtonActivation = characterButtonActivation;
                if (characterButtonActivation != null)
                {
                    characterButtonActivation.InButtonActivatedZone = false;
                    characterButtonActivation.ButtonActivatedZone = null;
                }
            }

            if ((_buttonPrompt != null) && !AlwaysShowPrompt)
            {
                HidePrompt();
            }

            _currentCharacter = null;
        }

        /// <summary>
        /// Checks the remaining number of uses and eventual delay between uses and returns true if the zone can be activated.
        /// </summary>
        /// <returns><c>true</c>, if number of uses was checked, <c>false</c> otherwise.</returns>
        public virtual bool CheckNumberOfUses()
        {
            if (Time.time - _lastActivationTimestamp < DelayBetweenUses)
            {
                return false;
            }

            if (UnimitedActivations)
            {
                return true;
            }

            if (_numberOfActivationsLeft == 0)
            {
                return false;
            }

            if (_numberOfActivationsLeft > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether or not this zone should be activated
        /// </summary>
        /// <returns><c>true</c>, if conditions was checked, <c>false</c> otherwise.</returns>
        /// <param name="character">Character.</param>
        /// <param name="characterButtonActivation">Character button activation.</param>
        protected virtual bool CheckConditions(Character character, CharacterButtonActivation characterButtonActivation)
        {
            if (character == null)
            {
                return false;
            }

            if (RequiresPlayerCharacter)
            {
                if (character.CharacterType != Character.CharacterTypes.Player)
                {
                    return false;
                }
            }

            if (RequiresButtonActivationAbility)
            {
                // we check that the object colliding with the water is actually a TopDown controller and a character
                if (characterButtonActivation == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
