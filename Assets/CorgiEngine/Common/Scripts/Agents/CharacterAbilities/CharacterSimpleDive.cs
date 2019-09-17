using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// Add this class to a Character and it'll be able to dive by pressing down, pounding the ground in the process.
    /// This class is derived from CharacterDive and shows how you can very simply extend an ability to change how it detects input
    /// Animator parameters : Diving (bool)
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Simple Dive")]
    public class CharacterSimpleDive : CharacterDive
    {
        /// <summary>
		/// We override input detection to have it simply look at the down direction
		/// </summary>
		protected override void HandleInput()
        {
            if (_verticalInput < -_inputManager.Threshold.y)
            {
                InitiateDive();
            }
        }
    }
}