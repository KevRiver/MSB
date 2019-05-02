using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private bool arrowMarkerOn;

    public GameObject arrowMarker;

    private int x = 0;
    // Start is called before the first frame update
    void Start()
    {
        arrowMarker = GameObject.Find("FlagArrow");
    }

    // Update is called once per frame
    void Update()
    {
        if (arrowMarkerOn)
        {
            // 화샇표 마커의 회전
            //arrowMarker.transform.rotation = Quaternion.Euler(0, 0, x);
            x++;
        }
    }
    void OnBecameVisible() {
        arrowMarkerOn = false;
    }
    void OnBecameInvisible(){
        arrowMarkerOn = true;
    }
}
