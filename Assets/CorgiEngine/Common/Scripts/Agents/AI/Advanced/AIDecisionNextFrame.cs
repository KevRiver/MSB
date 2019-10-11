using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This decision will return true when entering the state this Decision is on.
    /// </summary>
    public class AIDecisionNextFrame : AIDecision
    {
        /// <summary>
        /// We return true on Decide
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return true;
        }
    }
}
