using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Transform m_tr;
    private Rigidbody2D m_rb;

    public float m_hp;
    public float m_moveSpeed;
    private float m_maxSpeed;
    public bool isMovable;
    public Vector3 m_velocity;
    //public Weapon weapon;
    
	// Use this for initialization
	void Start () {
        m_tr = GetComponent<Transform>();
        m_rb = GetComponent<Rigidbody2D>();
        m_hp = 3.0f;
        m_moveSpeed = 15.0f;
        m_maxSpeed = 6.0f;
        isMovable = true;
	}
	
	// Update is called once per frame
	void Update () {
        move();
        m_velocity = m_rb.velocity;
            
	}

    public void die() {

    }

    public void move() {
        if (!isMovable)
            return;

        if (Mathf.Abs(m_rb.velocity.x) > m_maxSpeed)
        {

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_rb.AddForce(Vector3.right * m_moveSpeed);
            m_tr.localScale = new Vector3(1.5f,1.5f, 0f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_rb.AddForce(Vector3.left * m_moveSpeed);
            m_tr.localScale = new Vector3(-1.5f, 1.5f, 0f);
        }
        else {}
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
