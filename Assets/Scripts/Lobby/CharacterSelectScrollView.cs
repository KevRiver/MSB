using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectScrollView : MonoBehaviour
{
    public int panelID;

    Transform panelTrans;
    GameObject centerPanel;
    GameObject contentView;
    GameObject selectButton;

    float scaleNum;
    // Start is called before the first frame update    
    float contentViewPos_x;
    void Start()
    {
        panelTrans = transform;
        centerPanel = GameObject.Find("CenterPos");
        contentView = GameObject.Find("Content");
        selectButton = GameObject.Find("TransparentButton");

        // 처음 컨텐츠 오브젝트의 x 위치 
        contentViewPos_x = contentView.transform.position.x;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float distance_center = centerPanel.transform.localPosition.x + contentViewPos_x - panelTrans.position.x;

        if(distance_center != 0)
        {
            scaleNum = (44f - Mathf.Abs(distance_center)) / 220f;
            scaleNum += 1;
        }

        if (44 > distance_center && distance_center > 0)
        {
            sendPanelID();
            panelTrans.localScale = new Vector3(scaleNum, scaleNum, 1.2f);

        }
        else if(-44 < distance_center && distance_center <= 0)
        {
            sendPanelID();
            panelTrans.localScale = new Vector3(scaleNum, scaleNum, 1.2f);
        }
        else
        {
            panelTrans.localScale = new Vector3(1, 1, 1);
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
