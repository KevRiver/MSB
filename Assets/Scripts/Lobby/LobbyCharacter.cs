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
    public Sprite[] character = new Sprite[3];

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void changeSprite(int id){
        GetComponent<SpriteRenderer>().sprite = character[id];
        switch (id)
        {
            case 0:
                animator.SetTrigger("CharacterID_01");
                break;
            case 1:
                animator.SetTrigger("CharacterID_02");
                break;
        }
    }

}
