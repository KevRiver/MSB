using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(Collider2D))]
	/// <summary>
	/// Extend this class to activate something when a button is pressed in a certain zone
	/// </summary>
	public class ButtonActivated : MonoBehaviour 
	{
        public enum ButtonActivatedRequirements { Character, ButtonActivator, Either, None }

        [Header("Requirements")]
        /// the requirement(s) for this zone
        public ButtonActivatedRequirements ButtonActivatedRequirement = ButtonActivatedRequirements.Either;
        /// if this is true, this can only be activated by player Characters
        public bool RequiresPlayerType = true;
        /// if this is true, this zone can only be activated if the character has the required ability
        public bool RequiresButtonActivationAbility = true;


        [Header("Activation Conditions")]
        [Information("Here you can specific how that zone is interacted with. Does it require the ButtonActivation character ability? Can it only be interacted with by the Player? Does it require a button press? Can it only be activated while standing on the ground?", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// if this is false, the zone won't be activable 
        public bool Activable = true;
        /// if true, the zone will activate whether the button is pressed or not
        public bool AutoActivation = false;
        /// if this is set to false, the zone won't be activable while not grounded
        public bool CanOnlyActivateIfGrounded = false;
        /// Set this to true if you want the CharacterBehaviorState to be notified of the player's entry into the zone.
        public bool ShouldUpdateState = true;
        /// if this is true, enter won't be retriggered if another object enters, and exit will only be triggered when the last object exits
        public bool OnlyOneActivationAtOnce = true;

        [Header("Number of Activations")]
        [Information("You can decide to have that zone be interactable forever, or just a limited number of times, and can specify a delay between uses (in seconds).", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// if this is set to false, your number of activations will be MaxNumberOfActivations
        public bool UnlimitedActivations = true;
        /// the number of times the zone can be interacted with
        public int MaxNumberOfActivations = 0;
        /// the delay (in seconds) after an activation during which the zone can't be activated
        public float DelayBetweenUses = 0f;
        /// if this is true, the zone will disable itself (forever or until you manually reactivate it) after its last use
        public bool DisableAfterUse = false;

        [Header("Visual Prompt")]
        [Information("You can have this zone show a visual prompt to indicate to the player that it's interactable.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// if this is true, a prompt will be shown if setup properly
        public bool UseVisualPrompt = true;
        /// the gameobject to instantiate to present the prompt
        [ConditionAttribute("UseVisualPrompt", true)]
        public ButtonPrompt ButtonPromptPrefab;
        /// the text to display in the button prompt
        [ConditionAttribute("UseVisualPrompt", true)]
        public string ButtonPromptText = "A";
        /// the text to display in the button prompt
        [ConditionAttribute("UseVisualPrompt", true)]
        public Color ButtonPromptColor = MMColors.LawnGreen;
        /// the color for the prompt's text
        [ConditionAttribute("UseVisualPrompt", true)]
        public Color ButtonPromptTextColor = MMColors.White;
        /// If true, the "buttonA" prompt will always be shown, whether the player is in the zone or not.
        [ConditionAttribute("UseVisualPrompt", true)]
        public bool AlwaysShowPrompt = true;
        /// If true, the "buttonA" prompt will be shown when a player is colliding with the zone
        [ConditionAttribute("UseVisualPrompt", true)]
        public bool ShowPromptWhenColliding = true;
        /// If true, the prompt will hide after use
        [ConditionAttribute("UseVisualPrompt", true)]
        public bool HidePromptAfterUse = false;
        /// the position of the actual buttonA prompt relative to the object's center
        [ConditionAttribute("UseVisualPrompt", true)]
        public Vector3 PromptRelativePosition = Vector3.zero;
        
        [Header("Feedbacks")]
        /// a feedback to play when the zone gets activated
        public MMFeedbacks ActivationFeedback;
        /// a feedback to play when the zone tries to get activated but can't
        public MMFeedbacks DeniedFeedback;

        [Header("Actions")]
        public UnityEvent OnActivation;
        public UnityEvent OnExit;
        public UnityEvent OnStay;

        protected List<GameObject> _collidingObjects;
        protected Animator _buttonPromptAnimator;
        protected ButtonPrompt _buttonPrompt;
        protected Collider2D _collider;
        protected bool _promptHiddenForever = false;
        protected CharacterButtonActivation _characterButtonActivation;
        protected int _numberOfActivationsLeft;
        protected float _lastActivationTimestamp;
        protected bool _staying = false;

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
            _collidingObjects = new List<GameObject>();
            _collider = this.gameObject.GetComponent<Collider2D>();
            _numberOfActivationsLeft = MaxNumberOfActivations;

            if (AlwaysShowPrompt)
            {
                ShowPrompt();
            }
            ActivationFeedback?.Initialization(this.gameObject);
            DeniedFeedback?.Initialization(this.gameObject);
        }

        /// <summary>
        /// Makes the zone activable
        /// </summary>
        public virtual void MakeActivable()
        {
            Activable = true;
        }

        /// <summary>
        /// Makes the zone unactivable
        /// </summary>
        public virtual void MakeUnactivable()
        {
            Activable = false;
        }

        /// <summary>
        /// Makes the zone activable if it wasn't, unactivable if it was activable.
        /// </summary>
        public virtual void ToggleActivable()
        {
            Activable = !Activable;
        }

        protected virtual void Update()
        {
            if (_staying && (OnStay != null))
            {
                OnStay.Invoke();
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

            _staying = true;
            ActivateZone();
        }

        public virtual void TriggerExitAction(GameObject collider)
        {
            _staying = false;
            if (OnExit != null)
            {
                OnExit.Invoke();
            }
        }

        /// <summary>
        /// Activates the zone
        /// </summary>
		protected virtual void ActivateZone()
        {
            if (OnActivation != null)
            {
                OnActivation.Invoke();
            }
            _lastActivationTimestamp = Time.time;
            if (HidePromptAfterUse)
            {
                _promptHiddenForever = true;
                HidePrompt();
            }

            ActivationFeedback?.PlayFeedbacks(this.transform.position);
            _numberOfActivationsLeft--;

            if (DisableAfterUse && (_numberOfActivationsLeft <= 0))
            {
                DisableZone();
            }
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
            DeniedFeedback?.PlayFeedbacks(this.transform.position);
        }

        /// <summary>
        /// Shows the button A prompt.
        /// </summary>
        public virtual void ShowPrompt()
        {            
            if (!UseVisualPrompt || _promptHiddenForever || (ButtonPromptPrefab == null))
            {
                return;
            }

            // we add a blinking A prompt to the top of the zone
            if (_buttonPrompt == null)
            {
                _buttonPrompt = (ButtonPrompt)Instantiate(ButtonPromptPrefab);
                _buttonPrompt.Initialization();
                _buttonPromptAnimator = _buttonPrompt.gameObject.MMGetComponentNoAlloc<Animator>();
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
        /// Enables the button activated zone
        /// </summary>
        public virtual void DisableZone()
        {
            Activable = false;
            _collider.enabled = false;
        }

        /// <summary>
        /// Disables the button activated zone
        /// </summary>
        public virtual void EnableZone()
        {
            Activable = true;
            _collider.enabled = true;
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
            if (!CheckConditions(collider))
            {
                return;
            }

            // if we can only activate this zone when grounded, we check if we have a controller and if it's not grounded,
            // we do nothing and exit
            if (CanOnlyActivateIfGrounded)
            {
                CorgiController controller = collider.gameObject.MMGetComponentNoAlloc<CorgiController>();
                if (controller != null)
                {
                    if (!controller.State.IsGrounded)
                    {
                        return;
                    }
                }                
            }

            // at this point the object is colliding and authorized, we add it to our list
            _collidingObjects.Add(collider.gameObject);
            if (!TestForLastObject(collider))
            {
                return;
            }

            if (ShouldUpdateState)
            {
                _characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<CharacterButtonActivation>();
                if (_characterButtonActivation != null)
                {
                    _characterButtonActivation.InButtonActivatedZone = true;
                    _characterButtonActivation.ButtonActivatedZone = this;
                    _characterButtonActivation.InButtonAutoActivatedZone = AutoActivation;
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
            
            if (!CheckConditions(collider))
            {
                return;
            }

            _collidingObjects.Remove(collider.gameObject);
            if (!TestForLastObject(collider))
            {
                return;
            }

            if (ShouldUpdateState)
            {
                _characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<CharacterButtonActivation>();
                if (_characterButtonActivation != null)
                {
                    _characterButtonActivation.InButtonActivatedZone = false;
                    _characterButtonActivation.ButtonActivatedZone = null;
                }
            }

            if ((_buttonPrompt != null) && !AlwaysShowPrompt)
            {
                HidePrompt();
            }

            TriggerExitAction(collider);
        }

        /// <summary>
        /// Tests if the object exiting our zone is the last remaining one
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        protected virtual bool TestForLastObject(GameObject collider)
        {
            if (OnlyOneActivationAtOnce)
            {
                if (_collidingObjects.Count > 0)
                {
                    bool lastObject = true;
                    foreach (GameObject obj in _collidingObjects)
                    {
                        if ((obj != null) && (obj != collider))
                        {
                            lastObject = false;
                        }
                    }
                    return lastObject;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks the remaining number of uses and eventual delay between uses and returns true if the zone can be activated.
        /// </summary>
        /// <returns><c>true</c>, if number of uses was checked, <c>false</c> otherwise.</returns>
        public virtual bool CheckNumberOfUses()
        {
            if (!Activable)
            {
                return false;
            }

            if (Time.time - _lastActivationTimestamp < DelayBetweenUses)
            {
                return false;
            }

            if (UnlimitedActivations)
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
		protected virtual bool CheckConditions(GameObject collider)
        {
            Character character = collider.gameObject.MMGetComponentNoAlloc<Character>();

            switch (ButtonActivatedRequirement)
            {
                case ButtonActivatedRequirements.Character:
                    if (character == null)
                    {
                        return false;
                    }
                    break;

                case ButtonActivatedRequirements.ButtonActivator:
                    if (collider.gameObject.MMGetComponentNoAlloc<ButtonActivator>() == null)
                    {
                        return false;
                    }
                    break;

                case ButtonActivatedRequirements.Either:
                    if ((character == null) && (collider.gameObject.MMGetComponentNoAlloc<ButtonActivator>() == null))
                    {
                        return false;
                    }
                    break;


            }

            if (RequiresPlayerType)
            {
                if (character == null)
                {
                    return false;
                }
                if (character.CharacterType != Character.CharacterTypes.Player)
                {
                    return false;
                }
            }

            if (RequiresButtonActivationAbility)
            {
                CharacterButtonActivation characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<CharacterButtonActivation>();
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

