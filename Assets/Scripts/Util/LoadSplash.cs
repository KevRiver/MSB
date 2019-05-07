using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면에 로딩 화면을 덮어주는 모듈
/// 1. LoadSplash Component 를 원하는 Canvas Object 에 달아준다.
/// 2. 달아준 LoadSplash Component 의 SPLASH_PREFAB 항목에 Prefabs/LoadSplash 를 연결해준다.
/// 3. Canvas Object -> LoadSplash Component -> showLoadSplash 호출 순으로 사용한다.
/// </summary>
public class LoadSplash : MonoBehaviour
{
	public GameObject SPLASH_PREFAB;

	public enum SPLASH_TYPE
	{
		TYPE_SHORT, TYPE_TIP
	}

	bool IS_ACTIVE = false;

	GameObject loaderObject;
	GameObject loaderText;
	GameObject shortImageA;
	GameObject shortImageB;
	GameObject tipImage;
	GameObject tipText;
	LoadSplashTips tipPool;

    // Start is called before the first frame update
    void Start()
    {
		if (gameObject.GetComponent<Canvas>() == null)
		{
			Debug.Log("ERROR :: SPLASH IS NOT IN CANVAS OBJECT");
			return;
		}
		if (SPLASH_PREFAB == null)
		{
			Debug.Log("ERROR :: SPLASH PREFAB IS NULL");
			return;
		}
		loaderObject = Instantiate(SPLASH_PREFAB, gameObject.GetComponent<RectTransform>());
		loaderText = GameObject.Find("loaderText");
		shortImageA = GameObject.Find("shortImageA");
		shortImageB = GameObject.Find("shortImageB");
		tipImage = GameObject.Find("tipImage");
		tipText = GameObject.Find("tipText");
		tipPool = loaderObject.GetComponent<LoadSplashTips>();
		closeLoadSplash();
    }

    // Update is called once per frame
    void Update()
    {
		if (!IS_ACTIVE) return;
        shortImageA.transform.Rotate(0, 0, 80 * Time.deltaTime);
	}

	public void showLoadSplash(int ms, string message, SPLASH_TYPE type)
	{
		IS_ACTIVE = true;
		loaderObject.transform.localScale = new Vector3(1, 1, 1);
		shortImageA.transform.localScale = new Vector3(0, 0, 0);
		shortImageB.transform.localScale = new Vector3(0, 0, 0);
		tipImage.transform.localScale = new Vector3(0, 0, 0);
		tipText.transform.localScale = new Vector3(0, 0, 0);
		loaderText.GetComponent<Text>().text = "";
		if (message != null) loaderText.GetComponent<Text>().text = message; 
		Invoke("closeLoadSplash", (float)ms/1000);
		switch(type)
		{
			case SPLASH_TYPE.TYPE_SHORT:
				shortImageA.transform.localScale = new Vector3(1, 1, 1);
				shortImageB.transform.localScale = new Vector3(1, 1, 1);
				break;
			case SPLASH_TYPE.TYPE_TIP:
				int tipIndex = Random.Range(0, LoadSplashTips.TIP_SIZE);
				tipImage.GetComponent<Image>().sprite = tipPool.getTipSprite(tipIndex);
				tipText.GetComponent<Text>().text = tipPool.getTipText(tipIndex);
				tipImage.transform.localScale = new Vector3(1, 1, 1);
				tipText.transform.localScale = new Vector3(1, 1, 1);
				break;
		}
	}

	public void closeLoadSplash()
	{
		IS_ACTIVE = false;
		loaderObject.transform.localScale = new Vector3(0, 0, 0);
	}
}
