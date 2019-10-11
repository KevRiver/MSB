using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// As the name implies, an action that does nothing. Just waits there.
    /// </summary>
    public class AIActionDoNothing : AIAction
    {
        /// <summary>
        /// On PerformAction we do nothing
        /// </summary>
        public override void PerformAction()
        {

        }
    }
}
