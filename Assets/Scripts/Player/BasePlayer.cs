using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BasePlayer : MonoBehaviour
{
    public Transform tr;
    public Rigidbody2D rb;

    public int userIndex;
    public string userID;

    public int hp;
    
    void Start()
    {
        tr = transform;
        rb = GetComponent<Rigidbody2D>();
        hp = 5;
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
        //hitbox.SetActive(false);
    }

    public void showSkillMotion()
    {

    }
}
