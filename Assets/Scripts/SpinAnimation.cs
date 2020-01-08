using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAnimation : MonoBehaviour
{
    public enum RotateDirections
    {
        X,Y,Z
    }

    public RotateDirections RotateDirection;
    public float rotateSpeed;

    //private IEnumerator _spinAnim;
    private static Vector3[] _vector = {Vector3.right, Vector3.up, Vector3.forward};
    private Vector3 _euler;
    
    private void Update()
    {
        _euler = _vector[(int) RotateDirection] * rotateSpeed * Time.deltaTime;
        transform.Rotate(_euler);
    }
}
