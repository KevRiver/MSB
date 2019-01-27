using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //Player 게임오브젝트의 Transform, Rigidbody2D, Capsulecollider2D 를 드래그 앤 드롭
    public Transform tr;
    public Rigidbody2D rb;
    public BoxCollider2D bc;

    public int hp;
    public float dir;
    public float speed;

    void TakeDamage()
    {
        //공격 받았을 때 hp 깎임
        //피격 이펙트 : 캐릭터 색깔이 0.1초간 붉은색으로 바뀜
    }

    void Die()
    {
        //죽으면 죽는 이펙트 나오고, 게임오브젝트 삭제
    }
	
    // Use this for initialization
	void Start () {
        hp = 10;
        dir = 0;
        speed = 10f;
	}
	
	// Update is called once per frame
	void Update () {
        if (hp <= 0) //업데이트 마다 죽었는지 안죽었는지 체크
        {
            Die(); //죽었으면 Die 호출
        }

        GetComponent<Move>().move(rb,tr); // 화살표키로 이동
        GetComponent<Skill>().useSkill(rb, GetComponent<Move>().move(rb, tr)); //a키로 스킬사용
	}
}
