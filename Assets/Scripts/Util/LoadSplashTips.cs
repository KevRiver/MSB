using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSplashTips : MonoBehaviour
{
	public static int TIP_SIZE = 3;
	public Sprite[] TIP_SPRITE = new Sprite[TIP_SIZE];
	public string[] TIP_TEXT = new string[TIP_SIZE];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public Sprite getTipSprite(int index)
	{
		if (index < 0 || index >= TIP_SIZE)
		{
			Debug.Log("ERROR :: LOAD TIP SIZE IS INVALID");
			return TIP_SPRITE[0];
		}
		return TIP_SPRITE[index];
	}

	public string getTipText(int index)
	{
		if (index < 0 || index >= TIP_SIZE)
		{
			Debug.Log("ERROR :: LOAD TIP SIZE IS INVALID");
			return TIP_TEXT[0];
		}
		return TIP_TEXT[index];
	}
}
