using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using UnityEngine.Serialization;

public class VectorPad : MonoBehaviour
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
    public float horizontalSpeed;
    public float verticalSpeed;
    public float activateDelay;
    
    private CorgiController _controller;
    private MSB_Character _character;
    private Vector2 _speedMultiplier;

    private IEnumerator _activate;
    private bool _contacted;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogWarning("VectorPad Collided with " + other.name);
        _controller = other.GetComponent<CorgiController>();
        if (_controller == null)
            return;

        _character = other.GetComponent<MSB_Character>();
        if (_character == null)
            return;

        if (_character.IsRemote)
            return;

        _speedMultiplier.x = horizontalSpeed;
        _speedMultiplier.y = verticalSpeed;

        _activate = Activate(activateDelay);
        StartCoroutine(_activate);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        StopCoroutine(_activate);
    }

    private IEnumerator Activate(float delay)
    {
        yield return new WaitForSeconds(delay);
        _controller.SetHorizontalForce(0);
        _controller.SetVerticalForce(0);
        _controller.SetForce(_vector[(int) direction] * _speedMultiplier);
    }
}
