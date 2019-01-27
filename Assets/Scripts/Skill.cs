using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    //스킬은 무기에 포함되어있는 능력이다
    //스킬은 쿨타입이 있다
    //스킬은 사용하는 방향이 있다
    //스킬은 스킬이 적용되는 범위가있다
    //스킬은 플레이어가 살아있을때 쓸 수 있다
    //스킬에는 사용효과가 있다 
    //스킬에는 스킬모션이 있다
    //스킬에는 사운드이펙트가 있다
    
    public void useSkill(Rigidbody2D rb, float dir)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            dash(rb, dir);
        }
    }

    public void useSkill(Rigidbody2D rb, Vector3 dir)
    {
        // 캐릭터를 원하는 방향으로 이동시킬때
    }

    public void useSkill(GameObject instance, Vector3 dir)
    {
        // 투사체를 원하는 방향으로 날릴때
    }

    //dash에 필요한 arguments를 생각해보자 addforce를 사용하기때문에 스크립트가 적용되는 오브젝트의 rigidbody가 필요하다
    //각 dash 스킬마다 addforce 값이 다르기 때문에 float 값을 받아야한다
    //dash 스킬이 진행되는 방향이 필요하기 때문에 vector2 가 필요하다 aim에서 유저가 조작한 방향을 가져와서 cos, sin 을 활용해 vector2를 만들어야함
    public void dash(Rigidbody2D rb, float dir)
    {
        rb.AddForce(new Vector2(dir * 500, 0));
    }

    
    // 투사체의 prefab instance, 투사체 속도, 투사체 방향,
    public void fire()
    {
        //투사체 발사!
    }
}

