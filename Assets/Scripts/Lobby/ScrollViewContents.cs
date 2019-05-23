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
    GameObject selectButton;

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
        selectButton = GameObject.Find("TransparentButton");

        panelSound = GameObject.Find("PanelSound");

        l_Player = GameObject.Find("LobbyPlayer");


    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float distance_center = contentView.GetComponent<ScrollViewContentsManage>().portraitPos_x - transform.position.x;

        if (distance_center != 0)
        {
            scaleNum = (44f - Mathf.Abs(distance_center)) / 220f;
            scaleNum += 1;
        }

        if (44 > distance_center && distance_center > 0)
        {
            sendPanelID();
            transform.localScale = new Vector3(scaleNum, scaleNum, 1.2f);

        }
        else if(-44 < distance_center && distance_center <= 0)
        {
            sendPanelID();
            transform.localScale = new Vector3(scaleNum, scaleNum, 1.2f);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // 초상화 확대시 사운드 재생
        if(transform.localScale.x > 1 && soundCheck == true)
        {
            panelSound.GetComponent<AudioSource>().Play();
            soundCheck = false;

            // 초상화 확대시 패널 ID 전송해서 애니메이션 교체하는 함수 실행
            if (selectButton.GetComponent<SelectButton>().characterChoice == false)
            {
                // 캐릭터 스킨 애니메이션 교체하는 기능
                l_Player.GetComponent<LobbyPlayer>().l_changeSkin(panelID);
            }
            else
            {
                // 무기 애니메이션 교체하는 기능
            }
        }
        else if(transform.localScale.x == 1)
        {
            soundCheck = true;
        }
    }

    private void LateUpdate()
    {
        // 중앙 초상화 x 위치 컨텐츠 메니저로 전송
        if (panelID == 0 && firstCheck)
        {
            scaleNum = 1.2f;
            contentView.GetComponent<ScrollViewContentsManage>().portraitPos_x = transform.position.x;
            firstCheck = false;
        }
    }

    public void sendPanelID()
    {
        if (selectButton.GetComponent<SelectButton>().characterChoice == false)
        {
            // 캐릭터 스킨 선택
            selectButton.GetComponent<SelectButton>().getSkinID(panelID);
        }
        else
        {
            // 무기 선택
            selectButton.GetComponent<SelectButton>().getWeaponID(panelID);
        }
    }
}
