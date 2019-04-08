using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class Player : MonoBehaviour {
    private Transform m_tr;
    private Rigidbody2D m_rb;

    public int m_userIndex;
    public string m_userID;
    //public bool isLocalPlayer = false;

    public int m_hp;
    public float m_moveSpeed;
    public float m_jumpForce;
    private float m_maxSpeed;
    public bool isMovable; //Player의 이동가능여부 플래그
    public Vector3 m_velocity;

    private GameObject hitbox;
    private bool isAttacking = false;
    //bool isPlayerUser = true;

    public enum ACTION_TYPE
    {
        TYPE_ATTACK, TYPE_SKILL, TYPE_HIT
    }
    
    private int gameRoomIndex;
    private int gamePlayerIndex;
    private SocketIOComponent socketIO;

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
        Tr = GetComponent<Transform>();
        Rb = GetComponent<Rigidbody2D>();
        m_hp = 5;
        m_moveSpeed = 15.0f;
        m_jumpForce = 700.0f; //git - pull test by Gon
        m_maxSpeed = 6.0f;
        isMovable = true;

        hitbox = transform.GetChild(0).gameObject;
        hitbox.SetActive(false);

        GameObject networkmodule = GameObject.Find("NetworkModule");
        UserData userData = GameObject.Find("UserData").GetComponent<UserData>();
        gameRoomIndex = userData.getRoomIndex();
        gamePlayerIndex = userData.getPlayerIndex();

        socketIO = networkmodule.GetComponent<NetworkModule>().get_socket();

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
            

        if (Mathf.Abs(Rb.velocity.x) > m_maxSpeed)
        {

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Rb.AddForce(Vector3.right * m_moveSpeed);    //AddForce는 Time.deltaTime을 곱해줄 필요가 없다
            Tr.localScale = new Vector3(1.5f,1.5f,0f);   //localScale을 좌우로 바꾼다
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Rb.AddForce(Vector3.left * m_moveSpeed);
            Tr.localScale = new Vector3(-1.5f,1.5f,0f);
        }
        else {
            // stop condition
        }
    }

    IEnumerator syncUserMove()
    {
        while (true)
        {
            sendUserMove();
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void sendUserMove()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("gameRoomIndex", gameRoomIndex);
        jsonData.AddField("positionX", this.gameObject.transform.position.x);
        jsonData.AddField("positionY", this.gameObject.transform.position.y);
        jsonData.AddField("positionZ", this.gameObject.transform.position.z);
        jsonData.AddField("toward", this.gameObject.transform.localScale.x);
        jsonData.AddField("forceX", this.gameObject.GetComponent<Rigidbody2D>().velocity.x);
        jsonData.AddField("forceY", this.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        socketIO.Emit("userGameMove", jsonData);
        Debug.Log("userGameMove SENT");
    }

    public void sendUserAttack()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("gameRoomIndex", gameRoomIndex);
        jsonData.AddField("action", "Attack");
        //jsonData.AddField("animation",) 애니메이션 만들 떄 뭐 필요함?
        //jsonData.AddField("direction",) 애니메이션이 실행되는 방향
        socketIO.Emit("userGameAction", jsonData);
        Debug.Log("userGameAction SENT");
    }

    public void sendUserSkill()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("gameRoomIndex", gameRoomIndex);
        jsonData.AddField("action", "Skill");
        //jsonData.AddField("mel") 스킬 애니메이션에 필요한 것들을 가져와야댐
        socketIO.Emit("userGameAction", jsonData);
        Debug.Log("userGameAction SENT");
    }

    public void sendUserHit()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("gameRoomIndex", gameRoomIndex);
        jsonData.AddField("action", "Hit");
        //jsonData.AddField("mel") 어떻게 누구를 때렸는지, CC기가 적용되는지 안되는지
        // 상대방이 어떻게 제어되는지까지 각 무기마다 다 다르기 때문에 Weapon을 가져오고
        socketIO.Emit("userGameAction", jsonData);
        Debug.Log("userGameAction SENT");
    }

    public void jump() {
        Debug.Log("Jump!");
        Rb.AddForce(Vector3.up * m_jumpForce);
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
            sendUserAttack();
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
