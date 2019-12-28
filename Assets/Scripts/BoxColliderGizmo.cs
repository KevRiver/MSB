using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderGizmo : MonoBehaviour
{
    private BoxCollider2D _collider;
    private Vector3 _center;
    private void OnDrawGizmos()
    {
        _collider = GetComponent<BoxCollider2D>();
        if (!_collider)
            return;
        _center.x = transform.position.x + _collider.offset.x;
        _center.y = transform.position.y + _collider.offset.y;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_center, transform.localScale * _collider.size);
    }
}
