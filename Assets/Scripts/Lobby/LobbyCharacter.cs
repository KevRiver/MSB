//
//  LobbyCharacter
//  Created by 문주한 on 20/05/2019.
//
//  로비에서 뒷 배경의 플레이어가 움직이는 기능
//  플레이어 애니메이션 관리
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacter : MonoBehaviour
{
    Animator animator;

    public bool start;

    bool nextOn = false;

    Vector3 nextPos;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        nextPos = new Vector3(4, transform.position.y, -1);
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            l_Move();
        }
    }

    // 캐릭터 움직임
    private void FixedUpdate()
    {
        if (nextOn)
        {
            transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime * 2f);
            if (transform.position.x > 3.8f)
            {
                nextOn = false;
                l_Stop();
            }
        }
    }


    // 스킨 선택에 따른 에니메이터 컨트롤러 변경 
    public void l_changeSkin(int id)
    {
        animator.SetInteger("SkinID", id);
        
    }

    // 캐릭터 움직일 시 
    void l_Move()
    {
        animator.SetBool("isMoving", true);
        transform.localScale = new Vector3(-(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        nextOn = true;
    }

    // 캐릭터 멈췄을 시 
    void l_Stop()
    {
        animator.SetBool("isMoving", false);
        transform.localScale = new Vector3(-(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

}
