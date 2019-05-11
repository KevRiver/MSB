using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Player : MonoBehaviour {
    private GameManager gameManager;

    private Transform tr;
    private Rigidbody2D rb;
    private Animator animator;

    public int m_userIndex;
    public string m_userID;
    
    public int hp;
    private float moveSpeed;
    private float jumpForce;
    private float maxSpeed;
    private Vector3 moveVector;
    //private Vector3 attackVector;
    //private Vector3 skillVector;
    
    public MoveCtrlJoystick moveController;
    //public AtkCtrlJoystick atkController;
    //public SkillCtrlJoystick skillController;

    public bool isMovable = true;
    private bool isAttacking = false;

    public enum ACTION_TYPE
    {
        TYPE_ATTACK, TYPE_SKILL, TYPE_HIT
    }
    
    void Start () {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        tr = transform;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();

        hp = 5;
        moveSpeed = 50.0f;
        jumpForce = 800.0f;
        maxSpeed = 10.0f;
        moveVector = Vector3.zero;
        //attackVector = Vector3.zero;
        //skillVector = Vector3.zero;
        isMovable = true;

        StartCoroutine("syncUserMove");
    }
	
	// Update is called once per frame
	void Update () {
        HandleInput();
        Attack();
	}

    private void FixedUpdate()
    {
        Move();
    }

    public void HandleInput()
    {
        moveVector = MoveCtrlInput();
        //attackVector = AtkCtrlInput();
        //Debug.Log("Attack Vector : " + attackVector);
        //skillVector = SkillCtrlInput();
        //Debug.Log("Skill Vector : " + skillVector);
    }

    public Vector3 MoveCtrlInput()
    {
        float h = moveController.GetHorizontalValue();
        float v = moveController.GetVerticalValue();
        Vector3 moveVector = new Vector3(h, v).normalized;

        return moveVector;
    }

    /*public Vector3 AtkCtrlInput()
    {
        float h = atkController.GetHorizontalValue();
        float v = atkController.GetVerticalValue();
        Vector3 attackVector = new Vector3(h, v).normalized;

        return attackVector;
    }

    public Vector3 SkillCtrlInput()
    {
        float h = skillController.GetHorizontalValue();
        float v = skillController.GetVerticalValue();
        Vector3 skillVector = new Vector3(h, v).normalized;

        return skillVector;
    }*/

    public void Die() {
        // 리스폰 장소에서 다시 리스폰
    }

    public void Move() {
        if (!isMovable)
        {
            Debug.Log("return");
            return;
        }

        if (moveVector.x > 0f) 
        {
            Debug.Log("Joystick input right");
            if (rb.velocity.x > maxSpeed)
                return;

            rb.AddForce(Vector3.right * moveSpeed);    //AddForce는 Time.deltaTime을 곱해줄 필요가 없다         
            tr.localScale = new Vector3(0.5f,0.5f,0f);   //localScale을 좌우로 바꾼다
        }
        else if (moveVector.x < 0f)
        {
            Debug.Log("Joystick input left");
            if (rb.velocity.x < -maxSpeed)
                return;

            rb.AddForce(Vector3.left * moveSpeed);
            tr.localScale = new Vector3(-0.5f,0.5f,0f);
        }
        else {
            Debug.Log("Joystick input zero");
            // 아무것도 하지 않음
        }
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
       
    }

    public void Jump()
    {
        //Debug.Log("Jump!");
        animator.SetBool("Grounded", false);
        rb.AddForce(Vector3.up * jumpForce);
    }

    public void Aim()
    {

    }

    public void Attack()
    {
        // Hit Box 를 0.2 초간 enabled = true 후 다시 enable = false 시킨다
        // 추후 업그레이드
        // Animation을 실행시키고 Animation 프레임마다 BoxCollider로 HitBox 넣어줌 << 이게 제일 괜찮아 보임 판정상
        // +모든 공격은 조준을 해야되므로 플레이어가 조준한 곳으로 애니메이션, 히트박스가 생성되어야함
        if (!isAttacking)
        {
            isAttacking = true;
            //gameObject.GetComponent<BasePlayer>().showAttackMotion();
            gameObject.transform.GetChild(0).GetComponent<Sword>().PlayAttackAnim();
            StartCoroutine(WaitForIt());
            StartCoroutine(CoolTime());
            sendUserAttack();
        }
    }

    public void useSkill()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision) //캐릭터가 땅에 닿아있으면 Grounded true;
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("Grounded", true);
        }
    }

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

    public float getDamage(float currentHp, float damage) {
        float updatedHp = currentHp - damage;
        return updatedHp;
    } 

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator CoolTime()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }
}
