using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class CharacterBounce : CharacterAbility
{
    public float BounceForce;
    public override void ProcessAbility()
    {
        if (!AbilityPermitted)
            return;
        
        if (_controller.State.IsCollidingBelow)
        {
            _controller.SetVerticalForce(Mathf.Sqrt( 2f * BounceForce * Mathf.Abs(_controller.Parameters.Gravity) ));
        }
        
        if (_controller.State.IsCollidingAbove)
        {
            _controller.SetVerticalForce(-Mathf.Sqrt( 2f * BounceForce * Mathf.Abs(_controller.Parameters.Gravity) ));
        }

        if (_controller.State.IsCollidingLeft)
        {
            _controller.SetHorizontalForce(Mathf.Sqrt( 2f * BounceForce * Mathf.Abs(_controller.Parameters.Gravity) ));
        }

        if (_controller.State.IsCollidingRight)
        {
            _controller.SetHorizontalForce(-Mathf.Sqrt( 2f * BounceForce * Mathf.Abs(_controller.Parameters.Gravity) ));
        }
    }
}
