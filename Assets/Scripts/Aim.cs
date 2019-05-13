using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    private float angle;

    void Start()
    {
        angle = 0;
    }

    public void Aiming(Vector3 vector)
    {
        if (vector == Vector3.zero)
        {
            Debug.Log("No Attack Input!");
            transform.Find("LongAttackRange").GetComponent<SpriteRenderer>().enabled = false;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.Find("LongAttackRange").GetComponent<SpriteRenderer>().enabled = true;
            if (transform.parent.localScale.x < 0)
                angle = Mathf.Rad2Deg * Mathf.Atan2(vector.x, vector.y) + 90;
            else
                angle = -1 * Mathf.Rad2Deg * Mathf.Atan2(vector.x, vector.y) + 90;

            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public float GetAngle()
    {
        return angle;
    }
}
