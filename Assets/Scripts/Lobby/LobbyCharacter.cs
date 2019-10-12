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


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }


    // 스킨 선택에 따른 에니메이터 컨트롤러 변경 
    public void l_changeSkin(int id)
    {
        animator.SetInteger("SkinID", id);
        
    }

}
