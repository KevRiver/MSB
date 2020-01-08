using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

public class VectorPad : Jumper
{
    public enum Directions
    {
        Right,
        Left,
        Up,
        Down,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }
    
    private readonly Vector2[] _vector =
    {
        Vector2.right,
        Vector2.left,
        Vector2.up,
        Vector2.down,
        Vector2.up + Vector2.right,
        Vector2.up + Vector2.left,
        Vector2.down + Vector2.right,
        Vector2.down + Vector2.left
    };

    public Directions direction;
    private MSB_Character _character;

    /*protected override void LateUpdate()
    {
        if (_controllers.Count != 0)
        {
            foreach (var controller in _controllers)
            {
                _character = controller.gameObject.GetComponent<MSB_Character>();
                _characterJump = controller.gameObject.MMGetComponentNoAlloc<CharacterJump>();
                if (_character != null && !_character.IsRemote)
                {
                    controller.SetForce(_vector[(int) direction] *
                                        Mathf.Sqrt(2f * JumpPlatformBoost * -_controller.Parameters.Gravity));
                    if (_characterJump != null)
                        _characterJump.CanJumpStop = false;
                }

                ActivationFeedback?.PlayFeedbacks();
            }
        }
    }*/
    
    protected override void LateUpdate()
    {
        if (_controller != null)
        {
            _character = _controller.gameObject.GetComponent<MSB_Character>();
            _characterJump = _controller.gameObject.MMGetComponentNoAlloc<CharacterJump>();
            if (_character != null && !_character.IsRemote)
            {
                _controller.SetForce(_vector[(int) direction] *
                                    Mathf.Sqrt(2f * JumpPlatformBoost * -_controller.Parameters.Gravity));
                if (_characterJump != null)
                    _characterJump.CanJumpStop = false;
            }
            ActivationFeedback?.PlayFeedbacks();
        }
    }
}
