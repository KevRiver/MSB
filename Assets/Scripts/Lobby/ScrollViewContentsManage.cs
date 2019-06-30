//
//  ScrollViewContentsManage
//  Created by 문주한 on 20/05/2019.
//
//  스크롤 뷰에 캐릭터 초상화 패널과 무기 초상화 패널을 생성
//  스크롤 뷰에서 컨텐츠가 화면 밖으로 넘어가지 않게 한계 설정
//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewContentsManage : MonoBehaviour
{
    public GameObject[] skinArray = new GameObject [6];
    public GameObject centerObj;
    GameObject skinPanel;
    float pos_x;

    float limitLeftPos_x;
    float limitRightPos_x;

    public float portraitPos_x;

    float panelDistance;
    float initPanelDistance = Screen.width / 6.8f;

    // 스크롤 뷰
    public GameObject scrollView;
    ScrollRect scrollRect;

    // Use this for initialization
    void Start()
    {
       // 스킨 패널 생성 준비 
        pos_x = centerObj.transform.position.x;

        // 스킨 패널 생성
        skinInstantiat();

        // 양쪽 스킨 패널의 한계 포지션 계산
        limitLeftPos_x = transform.position.x;
        limitRightPos_x = limitLeftPos_x - (panelDistance * (skinArray.Length - 1));


    }

    private void Update()
    {
        //Debug.Log(scrollRect.content.sizeDelta);
    }

    
    //  스크롤 뷰에서 컨텐츠가 화면 밖으로 넘어가지 않게 한계 설정
    private void FixedUpdate()
    {
        // 컨텐츠 오브젝트의 포지션이 양쪽 마지막 스킨 패널의 포지션을 넘어가면 막음
        if (transform.position.x > limitLeftPos_x)
        { 
            transform.position = new Vector2(limitLeftPos_x, transform.position.y);
        }else if (transform.position.x < limitRightPos_x)
        {
            transform.position = new Vector2(limitRightPos_x, transform.position.y);
        }

    }
    

    // 스킨 패널 생성 
    void skinInstantiat()
    {
        for (int x = 0; x < skinArray.Length; x++)
        {

            skinPanel = Instantiate(skinArray[x], new Vector3(pos_x, centerObj.transform.position.y, 0), Quaternion.identity);
            // 스킨 ID 부여
            skinPanel.GetComponent<ScrollViewContents>().panelID = x;

            // 0번 스킨 패널과 1번 패널 사이의 거리 계산
            if (x == 0)
            {
                panelDistance = - (skinPanel.transform.position.x);
            }else if(x == 1)
            {
                panelDistance += skinPanel.transform.position.x;
            }

            pos_x += initPanelDistance;
            skinPanel.transform.parent = transform;
        }
    }
}
