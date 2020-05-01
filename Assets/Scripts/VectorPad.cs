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
    
    public float FeedBackPlayDelay = 0.5f;
    public float curTime = 0;
    private bool _IsFeedBackEnable = true;

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        _controller = collider.GetComponent<CorgiController>();
        if (_controller == null)
        {
            return;
        }		
        _controller.ContactWithVectorPad = true;
        _controller.ContactVectorPadForce = _vector[(int) direction] * Mathf.Sqrt(2f * JumpPlatformBoost * Mathf.Abs(_controller.Parameters.Gravity));
    }

    protected override void OnTriggerExit2D(Collider2D collider)
    {
        if (_controller != null)
        {
            if (collider.gameObject == _controller.gameObject)
            {
                _controller.ContactWithVectorPad = false;
                _controller.ContactVectorPadForce = Vector2.zero;
                _controller = null;
            }
        }
    }

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
