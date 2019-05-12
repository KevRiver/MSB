using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Animator animator1;

    public bool isInAction;
    public float attackDmg;
    public float skillDmg;
    public float attackCoolTime;
    public float skillCoolTime;

    //Animation Parameters
    public bool isDoingBasicAtk;    //true가 되면 공격 애니메이션을 실행한다
    public bool isDoingSkill;       //true가 되면 스킬 애니메이션을 실행한다

    protected virtual void ShowAttackAnim(Vector2 dir) { }
    protected virtual void ShowSkillAnim(Vector2 dir) { }

    private void Start()
    {
        animator1 = gameObject.GetComponent<Animator>();
        isInAction = false;
        isDoingBasicAtk = false;
        isDoingSkill = false;
    }

    private void Update()
    {
        animator1.SetBool("isDoingBasicAtk", isDoingBasicAtk);
        animator1.SetBool("isDoingSkill", isDoingSkill);
   
    }

    /*public void SetAngleZero(bool _isDoingBasicAtk)
    {
        if(!_isDoingBasicAtk)
            gameObject.transform.parent.gameObject.transform.localRotation = Quaternion.identity;
    }*/
    /*public void SetAnimatorParam(bool _isDoingBasicAtk, bool _isDoingSkill)
    {
        
    }*/
}
