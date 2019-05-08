using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    string gName;
    int panelID;
    float vel = 50;
    Transform panelTrans;
    GameObject centerPanel;
    GameObject contentView;

    GameObject textobject;
    Text t1;
    string t2;

    float scaleNum;
    // Start is called before the first frame update    
    float contentViewPos_x;
    void Start()
    {


        panelTrans = transform;
        gName = gameObject.name;
        centerPanel = GameObject.Find("CenterPos");
        contentView = GameObject.Find("Content");

        textobject = GameObject.Find("CText");
        t1 = textobject.GetComponent<Text>();
       

        // 처음 컨텐츠 오브젝트의 x 위치 
        contentViewPos_x = contentView.transform.position.x;

        // 위치에 따라 패널 ID 부여
        if (panelTrans.localPosition.x % 90 != 0) { 
            panelID = 0; 
        }
        else
        {
            panelID = (int)panelTrans.localPosition.x / 90;
        }

        t2 = "Select ID : " + panelID;
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

            t1.text = t2;
            //contentView.GetComponent<Rigidbody2D>().AddForce(Vector2.right * vel);
            panelTrans.localScale = new Vector3(scaleNum, scaleNum, 1.2f);

        }
        else if(-44 < distance_center && distance_center <= 0)
        {
            t1.text = t2;
            //contentView.GetComponent<Rigidbody2D>().AddForce(Vector2.left * vel);
            panelTrans.localScale = new Vector3(scaleNum, scaleNum, 1.2f);
        }
        else
        {
            panelTrans.localScale = new Vector3(1, 1, 1);
        }
    }
}
