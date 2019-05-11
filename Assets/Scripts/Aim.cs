using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    //public GameObject target;
    public AtkCtrlJoystick atkController;
    private Vector3 attackVector;

    void Start()
    {
        //gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        attackVector = Vector3.zero;
        //atkController = gameObject.GetComponent<AtkCtrlJoystick>();
    }

    void Update()
    {
        HandleInput();
        Aiming();
    }

    void Aiming()
    {
        if (attackVector == Vector3.zero)
        {
            Debug.Log("No Attack Input!");
            transform.Find("LongAttackRange").GetComponent<SpriteRenderer>().enabled = false;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.Find("LongAttackRange").GetComponent<SpriteRenderer>().enabled = true;
            float angle;
            if (transform.parent.localScale.x < 0)
                angle = Mathf.Rad2Deg * Mathf.Atan2(attackVector.x, attackVector.y) + 90;
            else
                angle = -1 * Mathf.Rad2Deg * Mathf.Atan2(attackVector.x, attackVector.y) + 90;

            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void HandleInput()
    {
        attackVector = AtkCtrlInput();
        Debug.Log("Attack Vector : " + attackVector);
    }

    public Vector3 AtkCtrlInput()
    {
        float h = atkController.GetHorizontalValue();
        float v = atkController.GetVerticalValue();
        Vector3 attackVector = new Vector3(h, v).normalized;

        return attackVector;
    }
}
