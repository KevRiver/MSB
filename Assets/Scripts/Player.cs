using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Transform m_tr;
    private Rigidbody2D m_rb;

    public float m_hp;
    public float m_moveSpeed;
    public float m_jumpForce;
    private float m_maxSpeed;
    public bool isMovable; //Player의 이동가능여부 플래그
    public Vector3 m_velocity;
    //public Weapon weapon; 

	// Use this for initialization
	void Start () {
        m_tr = GetComponent<Transform>();
        m_rb = GetComponent<Rigidbody2D>();
        m_hp = 3.0f;
        m_moveSpeed = 15.0f;
        m_jumpForce = 600.0f;
        m_maxSpeed = 6.0f;
        isMovable = true;
	}
	
	// Update is called once per frame
	void Update () {
        move();
        m_velocity = m_rb.velocity;
            
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
            Debug.Log("blank");
            /*if (m_rb.velocity.x > 0)
            {
                m_rb.velocity = new Vector2(5.9f, 0f);
            }
            else if (m_rb.velocity.x < 0) {
                m_rb.velocity = new Vector2(-5.9f, 0f);
            }*/
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("right");
            m_rb.AddForce(Vector3.right * m_moveSpeed);    //AddForce는 Time.deltaTime을 곱해줄 필요가 없다
            m_tr.localScale = new Vector3(1.5f,1.5f,0f);   //localScale을 좌우로 바꾼다
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Debug.Log("left");
            m_rb.AddForce(Vector3.left * m_moveSpeed);
            m_tr.localScale = new Vector3(-1.5f,1.5f,0f);
        }
        else {
            Debug.Log("else");
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

    }

    public void useSkill()
    {
         
    }
}
