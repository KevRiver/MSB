using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This action performs the defined number of jumps. Look below for a breakdown of how this class works.
    /// </summary>
    public class AIActionJump : AIAction
    {
        /// the number of jumps to perform while in this state
        public int NumberOfJumps = 1;

        protected CharacterJump _characterJump;
        protected int _numberOfJumps = 0;

        /// <summary>
        /// On init we grab our CharacterJump component
        /// </summary>
        protected override void Initialization()
        {
            _characterJump = this.gameObject.GetComponent<CharacterJump>();
        }

        /// <summary>
        /// On PerformAction we jump
        /// </summary>
        public override void PerformAction()
        {
            Jump();
        }

        /// <summary>
        /// Calls CharacterJump's JumpStart method to initiate the jump
        /// </summary>
        protected virtual void Jump()
        {
            if (_numberOfJumps < NumberOfJumps)
            {
                _characterJump.JumpStart();
                _numberOfJumps++;
            }            
        }

        /// <summary>
        /// When entering this state we reset our jump counter
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            _numberOfJumps = 0;
        }
    }
}
