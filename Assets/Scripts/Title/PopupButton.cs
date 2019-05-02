using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupButton : MonoBehaviour
{
    public GameObject popup;
    public GameObject policyUI;
    public GameObject nickNameUI;
    Vector2 centerPos;
    Vector2 outsidePos;
    // Start is called before the first frame update
    void Start()
    {
        outsidePos = popup.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void policyButtonClick()
    {
        centerPos = popup.transform.position;
        policyUI.transform.position = outsidePos;
        nickNameUI.transform.position = centerPos;
    }

    public void nicknameButtonClick()
    {
        popup.transform.position = outsidePos;
    }
}
