using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this class to an empty component in your scene, and it'll allow you to swap characters in your scene when pressing the SwapButton (P, by default)
    /// Each character in your scene will need to have a CharacterSwap class on it, and the corresponding PlayerID.
    /// You can see an example of such a setup in the MinimalCharacterSwap demo scene
    /// </summary>
    public class CharacterSwapManager : MonoBehaviour
    {
        /// the name of the axis to use to catch input and trigger a swap on press
        public string SwapButtonName = "Player1_SwapCharacter";
        /// the PlayerID set on the Characters you want to swap between
        public string PlayerID = "Player1";
        
        protected CharacterSwap[] _characterSwapArray;
        protected List<CharacterSwap> _characterSwapList; 
        protected CorgiEngineEvent _swapEvent = new CorgiEngineEvent(CorgiEngineEventTypes.CharacterSwap);

        /// <summary>
        /// On Start we update our list of characters to swap between
        /// </summary>
        protected virtual void Start()
        {
            UpdateList();
        }

        /// <summary>
        /// Grabs all CharacterSwap equipped characters in the scene and stores them in a list, sorted by Order
        /// </summary>
        public virtual void UpdateList()
        {
            _characterSwapArray = FindObjectsOfType<CharacterSwap>();
            _characterSwapList = new List<CharacterSwap>();

            // stores the array into the list if the PlayerID matches
            for (int i = 0; i<_characterSwapArray.Length; i++)
            {
                if (_characterSwapArray[i].PlayerID == PlayerID)
                {
                    _characterSwapList.Add(_characterSwapArray[i]);
                }
            }

            if (_characterSwapList.Count == 0)
            {
                return;
            }

            // sorts the list by order
            _characterSwapList.Sort(SortSwapsByOrder);
        }

        /// <summary>
        /// Static method to compare two CharacterSwaps
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int SortSwapsByOrder(CharacterSwap a, CharacterSwap b)
        {
            return a.Order.CompareTo(b.Order);
        }

        /// <summary>
        /// On Update, we watch for input
        /// </summary>
        protected virtual void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// If the user presses the Swap button, we swap characters
        /// </summary>
        protected virtual void HandleInput()
        {
            if (Input.GetButtonDown(SwapButtonName))
            {
                SwapCharacter();
            }
        }

        /// <summary>
        /// Changes the current character to the next one in line
        /// </summary>
        public virtual void SwapCharacter()
        {
            if (_characterSwapList.Count == 0)
            {
                return;
            }

            int newIndex = -1;

            for (int i=0; i<_characterSwapList.Count; i++)
            {
                if (_characterSwapList[i].Current())
                {
                    newIndex = i + 1;
                }
                _characterSwapList[i].ResetCharacterSwap();
            }

            if (newIndex >= _characterSwapList.Count)
            {
                newIndex = 0;
            }
            _characterSwapList[newIndex].SwapToThisCharacter();

            LevelManager.Instance.Players[0] = _characterSwapList[newIndex].gameObject.MMGetComponentNoAlloc<Character>();
            MMEventManager.TriggerEvent(_swapEvent);
        }
    }
}
