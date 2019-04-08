using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMJ : MonoBehaviour {
    private Transform m_tr;
    private Rigidbody2D m_rb;

    public int m_userIndex;
    public string m_userID;

    public int m_hp;
    public float m_moveSpeed;
    public float m_jumpForce;
    private float m_maxSpeed;
    public bool isMovable; //Player의 이동가능여부 플래그
    public Vector3 m_velocity;

    private GameObject hitbox;
    private bool isAttacking = false;

    /*public int UserIndex
    {
        get
        {
            return userIndex;
        }

        set
        {
            userIndex = value;
        }
    }
    //public string UserID
    {
        get
        {
            return userID;
        }

        set
        {
            userID = value;
        }
    }*/

    //public Weapon weapon; 

    // Use this for initialization
    void Start () {
        Debug.Log("Hoo Ha");
        m_tr = GetComponent<Transform>();
        m_rb = GetComponent<Rigidbody2D>();
        m_hp = 5;
        m_moveSpeed = 15.0f;
        m_jumpForce = 700.0f; //git - pull test by Gon
        m_maxSpeed = 6.0f;
        isMovable = true;

        hitbox = transform.GetChild(0).gameObject;
        hitbox.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        move();
        m_velocity = m_rb.velocity;
        attack();
            
	}

    public void die() {
        // 리스폰 장소에서 다시 리스폰
    }

    public void move() {
        if (!isMovable)
        {
            Debug.Log("return");
            return;
        }
            

        if (Mathf.Abs(m_rb.velocity.x) > m_maxSpeed)
        {

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_rb.AddForce(Vector3.right * m_moveSpeed);    //AddForce는 Time.deltaTime을 곱해줄 필요가 없다
            m_tr.localScale = new Vector3(1.5f,1.5f,0f);   //localScale을 좌우로 바꾼다
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_rb.AddForce(Vector3.left * m_moveSpeed);
            m_tr.localScale = new Vector3(-1.5f,1.5f,0f);
        }
        else {
            // stop condition
        }
    }

    public void jump() {
        Debug.Log("Jump!");
        m_rb.AddForce(Vector3.up * m_jumpForce);
    }

    public float getDamage(float currentHp, float damage) {
        float updatedHp = currentHp - damage;
        return updatedHp;
    }

    public void attack() {
        // Hit Box 를 0.2 초간 enabled = true 후 다시 enable = false 시킨다
        // 추후 업그레이드
        // Animation을 실행시키고 Animation 프레임마다 BoxCollider로 HitBox 넣어줌 << 이게 제일 괜찮아 보임 판정상
        // +모든 공격은 조준을 해야되므로 플레이어가 조준한 곳으로 애니메이션, 히트박스가 생성되어야함
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            isAttacking = true;
            hitbox.SetActive(true);
            StartCoroutine(WaitForIt());
            StartCoroutine(CoolTime());
        }
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
        hitbox.SetActive(false);
        //isAttacking = false;
    }

    IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(0.5f);
        //hitbox.SetActive(false);
        isAttacking = false;
    }

    public void useSkill()
    {
         
    }
}
