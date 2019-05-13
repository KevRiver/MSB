using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Animator animator;
    public GameObject weaponAxis;

    public float attackDmg;
    public float skillDmg;
    //public float attackCoolTime;
    public float skillCoolTime;

    //Animation Parameters
    public bool isInAction;
    public bool isDoingBasicAtk;    //true가 되면 공격 애니메이션을 실행한다
    public bool isDoingSkill;       //true가 되면 스킬 애니메이션을 실행한다

    protected virtual void ShowAttackAnim(Vector2 dir) { }
    protected virtual void ShowSkillAnim(Vector2 dir) { }

    private void Start()
    {
        weaponAxis = gameObject.transform.parent.gameObject;
        animator = gameObject.GetComponent<Animator>();

        isInAction = false;
        isDoingBasicAtk = false;
        isDoingSkill = false;
    }

    private void Update()
    {
        animator.SetBool("isDoingBasicAtk", isDoingBasicAtk);
        animator.SetBool("isDoingSkill", isDoingSkill);
   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

        }
    }

    public void SetAngleZero()
    {
           //gameObject.transform.parent.gameObject.transform.localRotation = Quaternion.identity;
    }
    /*public void SetAnimatorParam(bool _isDoingBasicAtk, bool _isDoingSkill)
    {
        
    }*/
}
