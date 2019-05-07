using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 상단에서 내려오는 알림바를 만들어주는 모듈
/// 1. ToastAlerter Component 를 원하는 Canvas Object 에 달아준다.
/// 2. 달아준 ToastAlerter Component 의 TOAST_PREFAB 항목에 Prefabs/ToastAlerter 를 연결해준다.
/// 3. Canvas Object -> ToastAlerter Component -> showToast 호출 순으로 사용한다.
/// </summary>
public class ToastAlerter : MonoBehaviour
{
	public GameObject TOAST_PREFAB;

	GameObject canvasObject;
	GameObject toastObject;
	GameObject textObject;
	Text textClass;
	Image imageClass;

	float canvasX;
	float canvasY;
	float canvasWidth;
	float canvasHeight;

	public enum MESSAGE_TYPE {
		TYPE_GREEN, TYPE_BLUE, TYPE_RED, TYPE_ORANGE, TYPE_BLACK
	}

    // Start is called before the first frame update
    void Start()
    {
		canvasObject = gameObject;
		if (gameObject.GetComponent<Canvas>() == null)
		{
			Debug.Log("ERROR :: TOAST IS NOT IN CANVAS OBJECT");
			return;
		}
		if (TOAST_PREFAB == null)
		{
			Debug.Log("ERROR :: TOAST PREFAB IS NULL");
			return;
		}
		canvasX = canvasObject.GetComponent<RectTransform>().rect.x;
		canvasY = canvasObject.GetComponent<RectTransform>().rect.y;
		canvasWidth =canvasObject.GetComponent<RectTransform>().rect.width;
		canvasHeight = canvasObject.GetComponent<RectTransform>().rect.height;
		toastObject = Instantiate(TOAST_PREFAB, canvasObject.GetComponent<RectTransform>());
		imageClass = toastObject.GetComponent<Image>();
		textClass = GameObject.Find("ToastText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	public void showToast(string text, MESSAGE_TYPE type, int duration)
	{
		StartCoroutine(showToastCOR(text, type, duration));
	}

	private IEnumerator showToastCOR(string text, MESSAGE_TYPE type, int duration)
	{
		switch(type)
		{
			case MESSAGE_TYPE.TYPE_GREEN:
				imageClass.color = new Color(92f / 255f, 194f / 255f, 92f / 255f);
				break;
			case MESSAGE_TYPE.TYPE_BLUE:
				imageClass.color = new Color(2f / 255f, 117f / 255f, 216f / 255f);
				break;
			case MESSAGE_TYPE.TYPE_RED:
				imageClass.color = new Color(217f / 255f, 83f / 255f, 79f / 255f);
				break;
			case MESSAGE_TYPE.TYPE_ORANGE:
				imageClass.color = new Color(240f / 255f, 173f / 255f, 78f / 255f);
				break;
			case MESSAGE_TYPE.TYPE_BLACK:
				imageClass.color = new Color(41f / 255f, 43f / 255f, 44f / 255f);
				break;
		}

		textClass.text = text;
		textClass.enabled = true;

		//Fade in
		yield return fadeInAndOut(textClass, true, 0.4f);

		//Wait for the duration
		float counter = 0;
		while (counter < duration)
		{
			counter += Time.deltaTime;
			yield return null;
		}

		//Fade out
		yield return fadeInAndOut(textClass, false, 0.4f);

		textClass.enabled = false;
	}

	IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
	{
		//Set Values depending on if fadeIn or fadeOut
		float a, b;
		if (fadeIn)
		{
			a = 40;	
			b = 0;
		}
		else
		{
			a = 0;
			b = 40;
		}

		//Color currentColor = Color.clear;
		float counter = 0f;

		while (counter < duration)
		{
			counter += Time.deltaTime;
			float scale = Mathf.Lerp(a, b, counter / duration);

			//targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
			toastObject.transform.localPosition = new Vector3(0, scale + canvasHeight/2, 0);
			yield return null;
		}
	}
}
