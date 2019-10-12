using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to a character and it'll be able to switch its model
    /// when pressing the SwitchCharacter button
    /// Note that this will only change the model, not the prefab. Only the visual representation, not the abilities and settings.
    /// If instead you'd like to change the prefab entirely, look at the CharacterSwitchManager class.
    /// If you want to swap characters between a bunch of characters within a scene, look at the CharacterSwap ability and CharacterSwapManager
    /// </summary>
    [HiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Switch Model")] 
	public class CharacterSwitchModel : CharacterAbility
    {
        /// the possible orders the next character can be selected from
        public enum NextModelChoices { Sequential, Random }

        [Header("Models")]
        [Information("Add this component to a character and it'll be able to switch its model when pressing the SwitchCharacter button (P by default).", InformationAttribute.InformationType.Info, false)]
        
        /// the list of possible characters models to switch to
        public GameObject[] CharacterModels;
        /// the order in which to pick the next character
        public NextModelChoices NextCharacterChoice = NextModelChoices.Sequential;
        /// the initial (and at runtime, current) index of the character prefab
        public int CurrentIndex = 0;
        /// if you set this to true, when switching model, the Character's animator will also be bound. This requires your model's animator is at the top level of the model in the hierarchy.
        /// you can look at the MinimalModelSwitch scene for examples of that
        public bool AutoBindAnimator = true;

        protected string _bindAnimatorMessage = "BindAnimator";
        protected bool[] _characterModelsFlipped;

        /// <summary>
        /// On init we disable our models and activate the current one
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            if (CharacterModels.Length == 0)
            {
                return;
            }

            foreach (GameObject model in CharacterModels)
            {
                model.SetActive(false);
            }

            CharacterModels[CurrentIndex].SetActive(true);
            _characterModelsFlipped = new bool[CharacterModels.Length];
        }

        /// <summary>
        /// At the beginning of each cycle, we check if we've pressed or released the switch button
        /// </summary>
        protected override void HandleInput()
		{
			if (_inputManager.SwitchCharacterButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
                SwitchModel();
			}	
		}

        /// <summary>
        /// On flip we store our state for our current model
        /// </summary>
        public override void Flip()
        {
            if (_characterModelsFlipped == null)
            {
                _characterModelsFlipped = new bool[CharacterModels.Length];
            }
            if (_characterModelsFlipped.Length == 0)
            {
                _characterModelsFlipped = new bool[CharacterModels.Length];
            }
            if (_character == null)
            {
                _character = this.gameObject.GetComponent<Character>();
            }
            
            if (!_character.IsFacingRight)
            {
                _characterModelsFlipped[CurrentIndex] = true;
            }
            else
            {
                _characterModelsFlipped[CurrentIndex] = false;
            }
        }

        /// <summary>
        /// Switches to the next model in line
        /// </summary>
		protected virtual void SwitchModel()
        {
            if (CharacterModels.Length <= 1)
            {
                return;
            }
            
            CharacterModels[CurrentIndex].gameObject.SetActive(false);

            // we determine the next index
            if (NextCharacterChoice == NextModelChoices.Random)
            {
                CurrentIndex = Random.Range(0, CharacterModels.Length);
            }
            else
            {
                CurrentIndex = CurrentIndex + 1;
                if (CurrentIndex >= CharacterModels.Length)
                {
                    CurrentIndex = 0;
                }
            }

            // we activate the new current model
            CharacterModels[CurrentIndex].gameObject.SetActive(true);
            _character.CharacterModel = CharacterModels[CurrentIndex];

            // we bind our animator
            if (AutoBindAnimator)
            {
                _character.CharacterAnimator = CharacterModels[CurrentIndex].gameObject.MMGetComponentNoAlloc<Animator>();
                _character.AssignAnimator();
                SendMessage(_bindAnimatorMessage);                
            } 
            
            // we flip our character's model if needed 
            if (_character.IsFacingRight == _characterModelsFlipped[CurrentIndex])
            {
                _character.FlipModel();
                _characterModelsFlipped[CurrentIndex] = !_character.IsFacingRight;
            }

            // we play our feedback
            PlayAbilityStartFeedbacks();
        }
	}
}
