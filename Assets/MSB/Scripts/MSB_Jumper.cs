using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

public class MSB_Jumper : Jumper
{
    public enum JumpDirection { Up, UpRight, UpLeft}
    public JumpDirection jumpDirection;
    public MSB_Character player;
   
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        _controller = null;

        player = collider.GetComponent<MSB_Character>();
        if (player == null)
        {
            return;
        }

        if (player.isLocalUser && (player.MovementState.CurrentState != CharacterStates.MovementStates.Dashing))
        {           
            _controller = player.GetComponent<CorgiController>();
        }
    }

    protected override void OnTriggerExit2D(Collider2D collider)
    {
        if (_controller != null)
        {
            if (collider.gameObject == _controller.gameObject)
            {
                _controller = null;
            }
        }
    }

    protected override void LateUpdate()
    {
        if (_controller != null)
        {
            player.MovementState.ChangeState(CharacterStates.MovementStates.Jumping);
            switch (jumpDirection)
            {
                case JumpDirection.Up:
                    _controller.SetForce(Vector2.up * Mathf.Sqrt(2f * JumpPlatformBoost * -_controller.Parameters.Gravity));
                    break;
                case JumpDirection.UpRight:
                    _controller.SetForce(new Vector2(1, 1) * Mathf.Sqrt(JumpPlatformBoost * -_controller.Parameters.Gravity));
                    break;
                case JumpDirection.UpLeft:
                    _controller.SetForce(new Vector2(-1, 1) * Mathf.Sqrt(JumpPlatformBoost * -_controller.Parameters.Gravity));
                    break;
            }

            _characterJump = _controller.gameObject.GetComponentNoAlloc<CharacterJump>();
            if (_characterJump != null)
            {
                _characterJump.CanJumpStop = false;
            }
        }
    }
}
