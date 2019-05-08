using UnityEngine;
using System.Collections;

public class ContentMoving : MonoBehaviour
{
    public GameObject centerPos;
    public GameObject[] skin = new GameObject [6];
    GameObject t1;
    int centerNum;
    int instantiateNum;
    float pos_x;

    float limitLeftPos_x;
    float limitRightPos_x;


    // Use this for initialization
    void Start()
    {

        pos_x = centerPos.transform.position.x;
        centerNum = skin.Length / 2;
        instantiateNum = centerNum;

        skinInstantiat();

        limitLeftPos_x = 90 * centerNum;
        if(skin.Length % 2 == 1)
        {
            limitRightPos_x = -90 * centerNum;
        }
        else
        {
            limitRightPos_x = -90 * (centerNum - 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("lp = " + transform.localPosition.x);
    }

    private void FixedUpdate()
    {
        if(transform.localPosition.x > limitLeftPos_x)
        {
            transform.localPosition = new Vector2(limitLeftPos_x, transform.localPosition.y);
        }else if (transform.localPosition.x < limitRightPos_x)
        {
            transform.localPosition = new Vector2(limitRightPos_x, transform.localPosition.y);
        }
    }

    void skinInstantiat()
    {
        for (int x = 0; x < skin.Length; x++)
        {
            if (x <= centerNum)
            {
                t1 = Instantiate(skin[instantiateNum], new Vector3(pos_x, centerPos.transform.position.y, 0), Quaternion.identity);
                instantiateNum--;
                pos_x -= 90f;
                t1.transform.parent = transform;
                if (x == centerNum)
                {
                    pos_x = centerPos.transform.position.x + 90f;
                    instantiateNum = centerNum + 1;
                }
            }
            else
            {
                t1 = Instantiate(skin[instantiateNum], new Vector3(pos_x, centerPos.transform.position.y, 0), Quaternion.identity);
                instantiateNum++;
                pos_x += 90f;
                t1.transform.parent = transform;
            }
        }
    }
}
