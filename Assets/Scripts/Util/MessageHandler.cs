using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour
{
    public GameObject messageBackground;
    public GameObject messageUserA;
    public GameObject messageUserAImage;
    public GameObject messageUserB;
    public GameObject messageUserBImage;
    public GameObject messageText;

    public Sprite swordHead;
    public Sprite shurikenHead;

    // Start is called before the first frame update
    void Start()
    {
        messageBackground = GameObject.Find("MessageBackground");
        messageUserA = GameObject.Find("MessageUserA");
        messageUserB = GameObject.Find("MessageUserB");
        messageUserAImage = GameObject.Find("MessageUserAImage");
        messageUserBImage = GameObject.Find("MessageUserBImage");
        messageText = GameObject.Find("MessageText");

        messageBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageUserA.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageUserAImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageUserB.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageUserBImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayKillLog(MSB_Character killUser, MSB_Character deadUser)
    {
        string message = killUser.c_userData.userNick + ", " + deadUser.c_userData.userNick + " 처치!";
        if (killUser.c_userData.userNumber == deadUser.c_userData.userNumber)
        {
            message = deadUser.c_userData.userNick + " 낙사!";
        }
        Debug.LogWarning("***DISPLAY KILL LOG***");
        messageBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        messageUserA.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.75f);
        messageUserB.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.75f);
        messageText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
        if (killUser.c_userData.userWeapon == 0)
        {
            messageUserAImage.GetComponent<Image>().sprite = swordHead;
        } else
        {
            messageUserAImage.GetComponent<Image>().sprite = shurikenHead;
        }
        messageUserAImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        if (deadUser.c_userData.userWeapon == 0)
        {
            messageUserBImage.GetComponent<Image>().sprite = swordHead;
        } else
        {
            messageUserBImage.GetComponent<Image>().sprite = shurikenHead;
        }
        messageUserBImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        messageText.GetComponent<Text>().text = message;

        StartCoroutine(fadeObject(false, messageBackground.GetComponent<Image>(), 1f, 1f));
        StartCoroutine(fadeObject(false, messageUserA.GetComponent<Image>(), 1f, 1f));
        StartCoroutine(fadeObject(false, messageUserAImage.GetComponent<Image>(), 1f, 1f));
        StartCoroutine(fadeObject(false, messageUserB.GetComponent<Image>(), 1f, 1f));
        StartCoroutine(fadeObject(false, messageUserBImage.GetComponent<Image>(), 1f, 1f));
        StartCoroutine(fadeObject(false, messageText.GetComponent<Text>(), 1f, 1f));
    }

    IEnumerator fadeObject(bool isFadeIn, Image targetImage, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        //Set Values depending on if fadeIn or fadeOut
        float fromAlpha = 1.0f, targetAlpha = 0.0f;
        if (isFadeIn)
        {
            fromAlpha = 0.0f;
            targetAlpha = 1.0f;
        }

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

    IEnumerator fadeObject(bool isFadeIn, Text targetText, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        //Set Values depending on if fadeIn or fadeOut
        float fromAlpha = 1.0f, targetAlpha = 0.0f;
        if (isFadeIn)
        {
            fromAlpha = 0.0f;
            targetAlpha = 1.0f;
        }

        //Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, targetAlpha, counter / duration);

            //targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            Color temp = targetText.color;
            temp.a = alpha;
            targetText.color = temp;
            yield return null;
        }
    }
}
