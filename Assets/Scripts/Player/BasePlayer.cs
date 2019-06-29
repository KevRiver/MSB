using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BasePlayer : MonoBehaviour
{
    public Transform tr;
    public Rigidbody2D rb;
    //Debug
    private Animator animator;

    
    //public GameObject basicAtkRange;
    //public GameObject skillAtkRange;

    public GameObject basicAtkRange;
    //private Sprite basicAtkRangeSprite;
    //private Vector2 basicAtkRangePos;

    public GameObject skillRange;
    //private Sprite skillRangeSprite;
    //private Vector2 skillRangePos;

    public GameObject aimAxis;
    public GameObject weaponAxis;
    public GameObject weapon;

    public bool isMovable;
    public float realSpeed;
    public bool isGrounded;

    //public int userIndex;
    //public string userID;

    public int hp;

    private void Awake()
    {
        aimAxis = gameObject.transform.Find("AimAxis").gameObject;
        weaponAxis = gameObject.transform.Find("WeaponAxis").gameObject;
        AttachWeapon(1);
    }

    void Start()
    {
        Debug.Log("BasePlayer Start called");
        tr = transform;
        rb = GetComponent<Rigidbody2D>();

        animator = gameObject.GetComponent<Animator>();
        /*weaponAxis = gameObject.transform.Find("WeaponAxis").gameObject;
        weapon = weaponAxis.transform.GetChild(0).gameObject;
        hp = 5;*/

        isMovable = true;
        
    }

    private void Update()
    {
        SetAnimatorParam(Mathf.Abs(realSpeed), isGrounded);
    }

    public void SetAnimatorParam(float _speed, bool _isGrounded)
    {
        this.animator.SetFloat("Speed", _speed);
        this.animator.SetBool("Grounded", _isGrounded);
    }

    public void AttachWeapon(int index)    //플레이어 오브젝트에 달아줄 무기의 인덱스
    {
        Debug.Log("Attach Weapon called");
        //float posX = 0;
        //float posY = 0;
        switch (index)
        {
            case 1:
                //WeaponAxis의 자식으로 소드 프리팹을 붙혀준다
                weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/Sword"), weaponAxis.transform) as GameObject;

                //소드 프리팹의 기본공격 범위와 스킬영역 범위
                basicAtkRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/BasicAtkRange"), aimAxis.transform) as GameObject;
                basicAtkRange.GetComponent<SpriteRenderer>().enabled = false;
                skillRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/SkillRange"), aimAxis.transform) as GameObject;
                skillRange.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 2:
                //활 프리팹을 붙혀준다
                weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/BowPrefab/Bow"), weaponAxis.transform) as GameObject;

                //활 프리팹의 기본공격 범위와 스킬영역 범위
                basicAtkRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/BowPrefab/BasicAtkRange"), aimAxis.transform) as GameObject;
                basicAtkRange.GetComponent<SpriteRenderer>().enabled = false;
                skillRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/BowPrefab/SkillRange"), aimAxis.transform) as GameObject;
                skillRange.GetComponent<SpriteRenderer>().enabled = false;
                break;
            default:
                break;
        }
    }

    public void showAtkAnim()
    {
        Debug.Log("showAtkAnim called");
        weapon.GetComponent<Weapon>().isAttacking = true;
    }

    public void showSkillAnim()
    {
        weapon.GetComponent<Weapon>().isUsingSkill = true;
    }

    public void Die()
    {
        // 리스폰 장소에서 다시 리스폰
    }

    public float GetDamage(float currentHp, float damage)
    {
        float updatedHp = currentHp - damage;
        return updatedHp;
    }

    private void OnTriggerEnter2D(Collider2D collision) //캐릭터가 땅에 닿아있으면 Grounded true;
    {
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    /*
    public void showAttackMotion()
    {
        //19.05.03 이제 PlayAttackMotion 이 불림
        //19.04.28 이제 히트박스 애니메이션이 실행됨
        //hitbox.SetActive(true);
        //gameObject.transform.GetChild(0).GetComponent<HitBox>().HitAnimStart();
        //StartCoroutine(WaitForIt());
    }*/

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
        //hitbox.SetActive(false);
    }

    public void showSkillMotion()
    {

    }
}
