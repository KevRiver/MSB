//
//  ScrollViewContents
//  Created by 문주한 on 20/05/2019.
//
//  스크롤 뷰에서 캐릭터 초상화가 화면 중앙에 오면 크기를 키우며 강조하고, 초상화 패널의 아이디 값을 전송한다.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContents : MonoBehaviour
{
    public int panelID;

    GameObject contentView;
    GameObject canvas;

    // 사운드 관련 변수
    private bool soundCheck = false;
    public GameObject panelSound;

    float scaleNum = 1;

    private bool firstCheck = true;

    // 로비 플레이어
    GameObject l_Player;

    void Start()
    {
        contentView = GameObject.Find("Content");
        canvas = GameObject.Find("Canvas");

        panelSound = GameObject.Find("PanelSound");

        l_Player = GameObject.Find("LobbyCharacter");


    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // 초상화 확대시 사운드 재생
        if(transform.localScale.x > 1 && soundCheck == true)
        {
            panelSound.GetComponent<AudioSource>().Play();
            soundCheck = false;
            /*
            // 개발되지 않은 부분
            if (panelID != 0)
            {
                characterSelectButton.GetComponent<CharacterSelectButton>().activeRejectBackground();
            }
            else
            {
                characterSelectButton.GetComponent<CharacterSelectButton>().disableRejectBackground();
            }
            */
            // 초상화 확대시 패널 ID 전송해서 애니메이션 교체하는 함수 실행
            
                // 캐릭터 스킨 애니메이션 교체하는 기능
                l_Player.GetComponent<LobbyCharacter>().l_changeSkin(panelID);
            

        }
        else if(transform.localScale.x == 1)
        {
            soundCheck = true;
        }
    }

    private void LateUpdate()
    {
    }
    
    public void sendPanelID()
    {
        
            // 캐릭터 스킨 선택
            canvas.GetComponent<ManageLobbyObject>().getSkinID(panelID);
        

    }
    
}
