using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //private Animator animator;
    private GameObject weaponAxis;
    private Animator weaponAnimator;
    public Sprite weaponSprite;
    public Sprite weaponBasicAtkRange;
    public Sprite weaponSkillRange;

    public BoxCollider2D basicAtkHitBox;
    public BoxCollider2D skillHitBox;

    public float attackDmg;
    public float skillDmg;
    //public float attackCoolTime;
    public float skillCoolTime;

    //Animation Parameters
    public bool isInAction;
    public bool isAttacking;    //true가 되면 공격 애니메이션을 실행한다
    public bool isUsingSkill;       //true가 되면 스킬 애니메이션을 실행한다

    //protected virtual void ShowAttackAnim(Vector2 dir) { }
    //protected virtual void ShowSkillAnim(Vector2 dir) { }

    private void Awake()
    {
        weaponAnimator = gameObject.GetComponent<Animator>();
        //weaponBasicAtkRange = Resources.Load<Sprite>("Sprites/AttackRange/Sword/BasicAtkRange");
        //weaponSkillRange = Resources.Load<Sprite>("Sprites/AttackRange/Sword/SkillRange");
    }

    private void Start()
    {
        weaponAxis = gameObject.transform.parent.gameObject;
        isInAction = false;
        isAttacking = false;
        isUsingSkill = false;   
    }

    private void Update()
    {
        weaponAnimator.SetBool("isAttacking", isAttacking);
        weaponAnimator.SetBool("isUsingSkill", isUsingSkill);
        //weaponAnimator.SetBool("isUsingSkill", isUsingSkill);
   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 force;
            Debug.Log("Player Hit");
            Debug.Log(weaponAxis.transform.localScale.x);
            Debug.Log(weaponAxis.transform.localRotation.z);
            float angle = weaponAxis.transform.localRotation.z;
            if (gameObject.transform.parent.gameObject.transform.parent.transform.localScale.x < 0)
            {
                Debug.Log(Mathf.Cos(angle));
                force = new Vector3(Mathf.Cos(angle) * -1f, Mathf.Sin(angle)).normalized;
            }
            else
            {
                force = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
            }
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(force * 500);
        }

        if (collision.gameObject.GetComponent<DestroyBlock>() != null)
        {
            collision.gameObject.GetComponent<DestroyBlock>().destroyBlock();
        }
    }
}
