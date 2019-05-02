using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupButton : MonoBehaviour
{
    public GameObject popup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonClick()
    {
        popup.transform.position = new Vector2(1000, 173);
    }
}
