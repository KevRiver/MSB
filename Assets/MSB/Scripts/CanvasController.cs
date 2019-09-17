using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject[] uiObjs;
    public GameObject[] initialUIObjs;    

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {     
        foreach (GameObject uiObj in uiObjs)
        {
            uiObj.transform.GetChild(0).gameObject.SetActive(false);
        }

        foreach (GameObject uiObj in initialUIObjs)
        {
            uiObj.transform.GetChild(0).gameObject.SetActive(true);
        }  
    }
}
