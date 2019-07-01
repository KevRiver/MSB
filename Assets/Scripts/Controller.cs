using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject targetObj;
    private GameObject aimAxis;
    private GameObject weaponAxis;

    public MoveCtrlJoystick moveCtrlJoystick;
    public AtkCtrlJoystick atkCtrlJoystick;
    public SkillCtrlJoystick skillCtrlJoystick;

    private Vector3 moveVector;
    private Vector3 atkVector;
    private Vector3 skillVector;

    private float angle;

    private void Start()
    {
        aimAxis = targetObj.transform.Find("AimAxis").gameObject;
        weaponAxis = targetObj.transform.Find("WeaponAxis").gameObject;

        moveVector = Vector3.zero;
        atkVector = Vector3.zero;
        skillVector = Vector3.zero;
    }

    public void SetTargetObj(GameObject _targetObj)
    {
        targetObj = _targetObj;
    }

    private void Update()
    {
        if (targetObj == null)
            return;

        HandleInput();

        if (atkCtrlJoystick.isUsing)
            targetObj.GetComponent<PlayerAction>().Aim(atkVector, "BasicAtkRange(Clone)");
        else if (skillCtrlJoystick.isUsing)
            targetObj.GetComponent<PlayerAction>().Aim(skillVector, "SkillRange(Clone)");
        else
        {
            aimAxis.transform.Find("BasicAtkRange(Clone)").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            aimAxis.transform.Find("SkillRange(Clone)").gameObject.GetComponent<SpriteRenderer>().enabled = false;
            aimAxis.transform.localRotation = Quaternion.identity;
        }
    }

    private void FixedUpdate()
    {
        targetObj.GetComponent<PlayerMovement>().Move(moveVector);
        targetObj.GetComponent<PlayerMovement>().Jump();
    }

    public void HandleInput()
    {
        moveVector = MoveCtrlInput();
        atkVector = AtkCtrlInput();
        skillVector = SkillCtrlInput();
    }

    //Move Controll
    public Vector3 MoveCtrlInput()
    {
        float h = moveCtrlJoystick.GetHorizontalValue();
        float v = moveCtrlJoystick.GetVerticalValue();
        Vector3 moveVector = new Vector3(h, v).normalized;

        return moveVector;
    }
    //Attak Controll
    public Vector3 AtkCtrlInput()
    {
        float h = atkCtrlJoystick.GetHorizontalValue();
        float v = atkCtrlJoystick.GetVerticalValue();
        Vector3 atkVector = new Vector3(h, v).normalized;

        return atkVector;
    }
    //Skill Controll
    public Vector3 SkillCtrlInput()
    {
        float h = skillCtrlJoystick.GetHorizontalValue();
        float v = skillCtrlJoystick.GetVerticalValue();
        Vector3 skillVector = new Vector3(h, v).normalized;

        return skillVector;
    }
}
