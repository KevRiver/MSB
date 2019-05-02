using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Player : MonoBehaviour {
    private Transform m_tr;
    private Rigidbody2D m_rb;
    private Animator animator;

    public int m_userIndex;
    public string m_userID;
    public Animator m_animator;

    public int m_hp;
    public float m_moveSpeed;
    public float m_jumpForce;
    private float m_maxSpeed;
    public bool isMovable;
    public Vector3 m_velocity;
    
    private bool isAttacking = false;

    public enum ACTION_TYPE
    {
        TYPE_ATTACK, TYPE_SKILL, TYPE_HIT
    }

    private GameManager gameManager;

    public Transform Tr
    {
        get
        {
            return m_tr;
        }

        set
        {
            m_tr = value;
        }
    }

    public Rigidbody2D Rb
    {
        get
        {
            return m_rb;
        }

        set
        {
            m_rb = value;
        }
    }
    
    void Start () {
        //Debug.Log("Hoo Ha");
        Tr = GetComponent<Transform>();
        Rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        m_hp = 5;
        m_moveSpeed = 100.0f;
        m_jumpForce = 1050.0f; //git - pull test by Gon
        m_maxSpeed = 10.0f;
        isMovable = true;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        StartCoroutine("syncUserMove");
    }
	
	// Update is called once per frame
	void Update () {
        move();
        m_velocity = Rb.velocity;
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

        //m_animator.SetFloat("Speed", Mathf.Abs(Rb.velocity.x));
        animator.SetFloat("Speed", Mathf.Abs(Rb.velocity.x));

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //Debug.Log("Right Arrow input");
            if (Rb.velocity.x > m_maxSpeed)
                return;
            Rb.AddForce(Vector3.right * m_moveSpeed);    //AddForce는 Time.deltaTime을 곱해줄 필요가 없다
            Tr.localScale = new Vector3(1f,1f,0f);   //localScale을 좌우로 바꾼다
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Rb.velocity.x < -m_maxSpeed)
                return;
            Rb.AddForce(Vector3.left * m_moveSpeed);
            Tr.localScale = new Vector3(-1f,1f,0f);
        }
        else {
            // stop condition
        }
    }
/*
    IEnumerator syncUserMove()
    {
        while (true)
        {
            sendUserMove();
            yield return new WaitForSeconds(0.016f);
        }
    }

    public void sendUserMove()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("positionX", this.gameObject.transform.position.x);
        jsonData.AddField("positionY", this.gameObject.transform.position.y);
        jsonData.AddField("positionZ", this.gameObject.transform.position.z);
        jsonData.AddField("toward", this.gameObject.transform.localScale.x);
        jsonData.AddField("forceX", this.gameObject.GetComponent<Rigidbody2D>().velocity.x);
        jsonData.AddField("forceY", this.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        gameManager.sendUserMove(jsonData);
       // Debug.Log("userGameMove SENT");
    }

    public void sendUserAttack()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("action", ACTION_TYPE.TYPE_ATTACK.ToString());
        //jsonData.AddField("animation",) 애니메이션 만들 떄 뭐 필요함?
        //jsonData.AddField("direction",) 애니메이션이 실행되는 방향
        gameManager.sendUserAction(jsonData);
        Debug.Log("userGameAction SENT");
    }

    public void sendUserSkill()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("action", ACTION_TYPE.TYPE_SKILL.ToString());
        //jsonData.AddField("mel") 스킬 애니메이션에 필요한 것들을 가져와야댐
        gameManager.sendUserAction(jsonData);
        Debug.Log("userGameAction SENT");
    }

    public void sendUserHit(int targetUserIndex, Vector2 hitDirection, ACTION_TYPE actionType)
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("target", targetUserIndex);
        jsonData.AddField("type", actionType.ToString());
        jsonData.AddField("hitDirectionX", hitDirection.x);
        jsonData.AddField("hitDirectionY", hitDirection.y);
        //jsonData.AddField("mel") 어떻게 누구를 때렸는지, CC기가 적용되는지 안되는지
        // 상대방이 어떻게 제어되는지까지 각 무기마다 다 다르기 때문에 Weapon을 가져오고
        gameManager.sendUserHit(jsonData);
        Debug.Log("userGameHit SENT");
    }
    */
    public void jump() {
        Debug.Log("Jump!");
        Rb.AddForce(Vector3.up * m_jumpForce);
    }

    public float getDamage(float currentHp, float damage) {
        float updatedHp = currentHp - damage;
        return updatedHp;
    }

    public void attack()
    {
        // Hit Box 를 0.2 초간 enabled = true 후 다시 enable = false 시킨다
        // 추후 업그레이드
        // Animation을 실행시키고 Animation 프레임마다 BoxCollider로 HitBox 넣어줌 << 이게 제일 괜찮아 보임 판정상
        // +모든 공격은 조준을 해야되므로 플레이어가 조준한 곳으로 애니메이션, 히트박스가 생성되어야함
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            isAttacking = true;
            gameObject.GetComponent<BasePlayer>().showAttackMotion();
            StartCoroutine(WaitForIt());
            StartCoroutine(CoolTime());
            //sendUserAttack();
        }
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
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
