using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BasePlayer : MonoBehaviour
{
    private Transform m_tr;
    private Rigidbody2D m_rb;

    public int m_userIndex;
    public string m_userID;

    public int m_hp;

    private GameObject hitbox;

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
    
    void Awake()
    {
        Tr = GetComponent<Transform>();
        Rb = GetComponent<Rigidbody2D>();
        m_hp = 5;

        //hitbox = transform.GetChild(0).gameObject;
        //hitbox.SetActive(false);
    }

    public void die()
    {
        // 리스폰 장소에서 다시 리스폰
    }

    public float getDamage(float currentHp, float damage)
    {
        float updatedHp = currentHp - damage;
        return updatedHp;
    }

    public void showAttackMotion()
    {
        //19.05.03 이제 PlayAttackMotion 이 불림
        //19.04.28 이제 히트박스 애니메이션이 실행됨
        //hitbox.SetActive(true);
        //gameObject.transform.GetChild(0).GetComponent<HitBox>().HitAnimStart();
        //StartCoroutine(WaitForIt());
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(0.1f);
        hitbox.SetActive(false);
    }

    public void showSkillMotion()
    {

    }
}
