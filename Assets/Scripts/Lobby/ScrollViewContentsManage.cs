using UnityEngine;
using System.Collections;

public class ScrollViewContentsManage : MonoBehaviour
{
    public GameObject centerObj;
    public GameObject[] skinArray = new GameObject [6];
    GameObject skinPanel;
    int centerNum;
    int instantiateNum;
    float pos_x;

    float centerPos_x;
    float limitLeftPos_x;
    float limitRightPos_x;

    // Use this for initialization
    void Start()
    {
        // 스킨 패널 생성 준비 
        pos_x = centerObj.transform.position.x;
        centerNum = skinArray.Length / 2;
        instantiateNum = centerNum;

        // 스킨 패널 생성
        skinInstantiat();

        // 양쪽 스킨 패널의 한계 포지션 계산
        limitLeftPos_x = 0;
        limitRightPos_x = -90 * ( skinArray.Length - 1 );

    }

    private void FixedUpdate()
    {
        // 컨텐츠 오브젝트의 포지션이 양쪽 마지막 스킨 패널의 포지션을 넘어가면 막음
        if (transform.localPosition.x > limitLeftPos_x)
        {
            transform.localPosition = new Vector2(limitLeftPos_x, transform.localPosition.y);
        }else if (transform.localPosition.x < limitRightPos_x)
        {
            transform.localPosition = new Vector2(limitRightPos_x, transform.localPosition.y);
        }

    }

    // 스킨 패널 생성 
    void skinInstantiat()
    {
        for (int x = 0; x < skinArray.Length; x++)
        {
            skinPanel = Instantiate(skinArray[x], new Vector3(pos_x, centerObj.transform.position.y, 0), Quaternion.identity);
            // 스킨 ID 부여
            skinPanel.GetComponent<CharacterSelectScrollView>().panelID = x;
            pos_x += 90f;
            skinPanel.transform.parent = transform;
            if(x == centerNum)
            {
                centerPos_x = skinPanel.transform.localPosition.x;
            }
        }
    }
}
