using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class JoystickBackgroundAlpha : MonoBehaviour
{
    Image background;
    Color backgroundColor;

    [Range(0.0f,1.0f)]
    public float alpha = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        backgroundColor = background.color;

        //apply alpha;
        background.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, alpha);
    }

    
}
