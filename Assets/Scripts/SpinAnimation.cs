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

    private IEnumerator _spinAnim;
    private Vector3[] _vector = {Vector3.right, Vector3.up, Vector3.forward};

    private void OnEnable()
    {
        _spinAnim = Spin(RotateDirection, rotateSpeed);
        StartCoroutine(_spinAnim);
    }

    private void OnDisable()
    {
        StopCoroutine(_spinAnim);
    }

    private IEnumerator Spin(RotateDirections rotDir, float speed)
    {
        while (true)
        {
            transform.Rotate(_vector[(int) rotDir] * speed );
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
