using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this component to an empty object in your scene, and when you'll press the SwitchCharacter button (P by default, change that in Unity's InputManager settings), 
    /// your main character will be replaced by one of the prefabs in the list set on this component. You can decide the order (sequential or random), and have as many as you want.
    /// Note that this will change the whole prefab, not just the visuals. 
    /// If you're just after a visual change, look at the CharacterSwitchModel ability.
    /// If you want to swap characters between a bunch of characters within a scene, look at the CharacterSwap ability and CharacterSwapManager
    /// </summary>
    public class CharacterSwitchManager : MonoBehaviour
    {
        /// the possible orders the next character can be selected from
        public enum NextCharacterChoices { Sequential, Random }

        [Header("Character Switch")]
        [Information("Add this component to an empty object in your scene, and when you'll press the SwitchCharacter button (P by default, change that in Unity's InputManager settings), your main character will be replaced by one of the prefabs in the list set on this component. You can decide the order (sequential or random), and have as many as you want.", InformationAttribute.InformationType.Info, false)]
        
        /// the list of possible characters prefabs to switch to
        public Character[] CharacterPrefabs;
        /// the order in which to pick the next character
        public NextCharacterChoices NextCharacterChoice = NextCharacterChoices.Sequential;
        /// the initial (and at runtime, current) index of the character prefab
        public int CurrentIndex = 0;
        /// if this is true, current health value will be passed from character to character
        public bool CommonHealth;

        [Header("Visual Effects")]
        /// a particle system to play when a character gets changed
        public ParticleSystem CharacterSwitchVFX;

        protected Character[] _instantiatedCharacters;
        protected ParticleSystem _instantiatedVFX;
        protected InputManager _inputManager;
        protected CorgiEngineEvent _switchEvent = new CorgiEngineEvent(CorgiEngineEventTypes.CharacterSwitch);

        /// <summary>
        /// On Awake we grab our input manager and instantiate our characters and VFX
        /// </summary>
        protected virtual void Awake()
        {
            _inputManager = FindObjectOfType(typeof(InputManager)) as InputManager;
            InstantiateCharacters();
            InstantiateVFX();
        }

        /// <summary>
        /// Instantiates and disables all characters in our list
        /// </summary>
        protected virtual void InstantiateCharacters()
        {
            _instantiatedCharacters = new Character[CharacterPrefabs.Length];

            for (int i = 0; i < CharacterPrefabs.Length; i++)
            {
                Character newCharacter = Instantiate(CharacterPrefabs[i]);
                newCharacter.name = "CharacterSwitch_" + i;
                newCharacter.gameObject.SetActive(false);
                _instantiatedCharacters[i] = newCharacter;
            }            
        }

        /// <summary>
        /// Instantiates and disables the particle system if needed
        /// </summary>
        protected virtual void InstantiateVFX()
        {
            if (CharacterSwitchVFX != null)
            {
                _instantiatedVFX = Instantiate(CharacterSwitchVFX);
                _instantiatedVFX.Stop();
                _instantiatedVFX.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// On Update we watch for our input
        /// </summary>
        protected virtual void Update()
        {
            if (_inputManager == null)
            {
                return;
            }

            if (_inputManager.SwitchCharacterButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                SwitchCharacter();
            }
        }

        /// <summary>
        /// Switches to the next character in the list
        /// </summary>
        protected virtual void SwitchCharacter()
        {
            if (_instantiatedCharacters.Length <= 1)
            {
                return;
            }

            // we determine the next index
            if (NextCharacterChoice == NextCharacterChoices.Random)
            {
                CurrentIndex = Random.Range(0, _instantiatedCharacters.Length);
            }
            else
            {
                CurrentIndex = CurrentIndex + 1;
                if (CurrentIndex >= _instantiatedCharacters.Length)
                {
                    CurrentIndex = 0;
                }
            }

            // we disable the old main character, and enable the new one
            LevelManager.Instance.Players[0].gameObject.SetActive(false);
            _instantiatedCharacters[CurrentIndex].gameObject.SetActive(true);

            // we move the new one at the old one's position
            _instantiatedCharacters[CurrentIndex].transform.position = LevelManager.Instance.Players[0].transform.position;
            _instantiatedCharacters[CurrentIndex].transform.rotation = LevelManager.Instance.Players[0].transform.rotation;

            // we keep the health if needed
            if (CommonHealth)
            {
                _instantiatedCharacters[CurrentIndex].gameObject.MMGetComponentNoAlloc<Health>().CurrentHealth = LevelManager.Instance.Players[0].gameObject.MMGetComponentNoAlloc<Health>().CurrentHealth;
            }

            // we put it in the same state the old one was in
            _instantiatedCharacters[CurrentIndex].MovementState.ChangeState(LevelManager.Instance.Players[0].MovementState.CurrentState);
            _instantiatedCharacters[CurrentIndex].ConditionState.ChangeState(LevelManager.Instance.Players[0].ConditionState.CurrentState);

            // we make it the current character
            LevelManager.Instance.Players[0] = _instantiatedCharacters[CurrentIndex];

            // we play our vfx
            if (_instantiatedVFX != null)
            {
                _instantiatedVFX.gameObject.SetActive(true);
                _instantiatedVFX.transform.position = _instantiatedCharacters[CurrentIndex].transform.position;
                _instantiatedVFX.Play();
            }

            // we trigger a switch event (for the camera to know, mostly)
            MMEventManager.TriggerEvent(_switchEvent);
        }
    }
}
