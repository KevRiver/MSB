using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleImageLodaer : MonoBehaviour
{
	public GameObject logoObject;
	public GameObject backgroundObject;
	public GameObject titleBackground;
	public Sprite[] sprites = new Sprite[9];

	RectTransform rectTransform;
	GameObject[] backgrounds = new GameObject[9];

    // Start is called before the first frame update
    void Start()
    {
		//canvasObject = gameObject;
		rectTransform = backgroundObject.GetComponent<RectTransform>();
		logoObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
		loadBackgroundSprite();
	}

    // Update is called once per frame
    void Update()
    {
	}

	public void loadBackgroundSprite()
	{
		StartCoroutine(handleBackgroundSprite(0, 0.0f));
		StartCoroutine(handleBackgroundSprite(1, 0.05f));
		StartCoroutine(handleBackgroundSprite(2, 0.10f));
		StartCoroutine(handleBackgroundSprite(3, 0.15f));
		StartCoroutine(handleBackgroundSprite(4, 0.20f));
		StartCoroutine(handleBackgroundSprite(5, 0.25f));
		StartCoroutine(handleBackgroundSprite(6, 0.30f));
		StartCoroutine(handleBackgroundSprite(7, 0.35f));
		StartCoroutine(handleBackgroundSprite(8, 0.40f));
		StartCoroutine(showMainLogo(0.70f));
		StartCoroutine(showMainText(1.00f));
	}

	IEnumerator showMainLogo(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		logoObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		yield return scaleInSprite(logoObject, true, true, 0.40f);
	}

	IEnumerator showMainText(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

        //yield return scaleInSprite(logoObject, true, true, 2.00f);
    }

	IEnumerator handleBackgroundSprite(int i, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		//rectTransform.position = new Vector3(rectTransform.position.x, rectTransform.position.y, -1);
		backgrounds[i] = Instantiate(titleBackground, rectTransform);
		backgrounds[i].GetComponent<Image>().sprite = sprites[i];
		if (i != 0) yield return fadeInSprite(backgrounds[i].GetComponent<Image>(), 0.20f);
		if (i == 1) yield return scaleInSprite(backgrounds[i], true, true, 5.00f);
		if (i == 4) yield return scaleInSprite(backgrounds[i], true, true, 5.00f);
		if (i == 6) yield return scaleInSprite(backgrounds[i], true, true, 5.00f);
		if (i == 7) yield return scaleInSprite(backgrounds[i], true, true, 5.00f);
		if (i == 8) yield return scaleInSprite(backgrounds[i], true, true, 5.00f);
	}

	IEnumerator fadeInSprite(Image targetImage, float duration)
	{
		//Set Values depending on if fadeIn or fadeOut
		float fromAlpha = 0.0f, targetAlpha = 1.0f;

		//Color currentColor = Color.clear;
		float counter = 0f;

		while (counter < duration)
		{
			counter += Time.deltaTime;
			float alpha = Mathf.Lerp(fromAlpha, targetAlpha, counter / duration);

			//targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
			Color temp = targetImage.color;
			temp.a = alpha;
			targetImage.color = temp;
			yield return null;
		}
	}

	IEnumerator scaleInSprite(GameObject targetObject, bool scaleX, bool scaleY, float duration)
	{
		//Set Values depending on if fadeIn or fadeOut
		float fromScale = 1.0f, targetScale = 1.2f;

		//Color currentColor = Color.clear;
		float counter = 0f;

		while (counter < duration)
		{
			counter += Time.deltaTime;
			float scale = Mathf.Lerp(fromScale, targetScale, counter / duration);
			targetObject.transform.localScale = new Vector3(scaleX ? scale : 1, scaleY ? scale : 1 , 1);
			
			yield return null;
		}
	}

}
