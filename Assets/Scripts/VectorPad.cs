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

    private CorgiController _controller;
    private MSB_Character _character;
    private Vector2 _speedMultiplier;
    private void OnTriggerEnter2D(Collider2D other)
    {
        _controller = other.GetComponent<CorgiController>();
        if (_controller == null)
            return;

        _character = other.GetComponent<MSB_Character>();
        if (_character == null)
            return;

        if (_character.IsRemote)
            return;
        _speedMultiplier = new Vector2(horizontalSpeed, verticalSpeed);
        _controller.SetForce(Vector2.zero);
        _controller.SetForce(_vector[(int) direction] * _speedMultiplier);
    }
}
