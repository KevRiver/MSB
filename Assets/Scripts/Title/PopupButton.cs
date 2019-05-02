using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupButton : MonoBehaviour
{
    public GameObject popup;
    public GameObject policyUI;
    public GameObject nickNameUI;
    Vector2 centerPos;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void policyButtonClick()
    {
        centerPos = popup.transform.position;
        policyUI.transform.position = new Vector2(1000, 0);
        nickNameUI.transform.position = centerPos;
    }

    public void nicknameButtonClick()
    {
        popup.transform.position = new Vector2(1000, 0);
    }
}
