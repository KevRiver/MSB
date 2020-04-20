using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KillMessageHandler : MonoBehaviour
{
    public GameObject messageBackground;
    public GameObject messageUserA;
    public GameObject messageUserAImage;
    public GameObject messageUserB;
    public GameObject messageUserBImage;
    public GameObject messageText;

    public Sprite purpHead;
    public Sprite jamonHead;
    public Sprite titanyanHead;

    private class OnGameStatus : NetworkModule.OnGameStatusListener
    {
        private KillMessageHandler _killMessageHandler;

        public OnGameStatus(KillMessageHandler killMessageHandler)
        {
            this._killMessageHandler = killMessageHandler;
        }
        public void OnGameEventCount(int count)
        {
            
        }

        public void OnGameEventTime(int time)
        {
            
        }

        public void OnGameEventReady(string readyData)
        {
            
        }

        public void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint)
        {
            
        }

        public void OnGameEventMessage(int type, string message)
        {
            if (type == 1) _killMessageHandler.DisplayKillMessage(type, message);
        }
    }

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
        messageUserAImage.GetComponent<Image>().preserveAspect = true;
        messageUserB.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageUserBImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageUserBImage.GetComponent<Image>().preserveAspect = true;
        messageText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        
        NetworkModule.GetInstance().AddOnEventGameStatus(new OnGameStatus(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayKillMessage(int type, string message)
    {
        JObject killObject = JObject.Parse(message);
        int killMakerIndex = killObject.GetValue("killMaker").Value<Int32>();
        int killTargetIndex = killObject.GetValue("killTarget").Value<int>();
        int killCount = killObject.GetValue("killCount").Value<int>();
        int deathCount = killObject.GetValue("deathCount").Value<int>();
        MSB_Character killUser = null;
        MSB_Character deadUser = null;
        if (!MSB_LevelManager.Instance._allPlayersCharacter.TryGetValue(killMakerIndex, out killUser))
        {
            Debug.LogWarning("***NO USER FOR KILLER INDEX " + killMakerIndex + "***");
            return;
        }
        if (!MSB_LevelManager.Instance._allPlayersCharacter.TryGetValue(killTargetIndex, out deadUser))
        {
            Debug.LogWarning("***NO USER FOR TARGET INDEX " + killTargetIndex + "***");
            return;
        }
        string displayMessage = killUser.cUserData.userNick + ", " + deadUser.cUserData.userNick + " 처치!";
        if (killUser.cUserData.userNumber == deadUser.cUserData.userNumber)
        {
            displayMessage = deadUser.cUserData.userNick + " 낙사!";
        }
        messageBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        messageUserA.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.75f);
        messageUserB.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.75f);
        messageText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
        if (killUser.cUserData.userWeapon == 0) // PURP
        {
            messageUserAImage.GetComponent<Image>().sprite = purpHead;
        } else if (killUser.cUserData.userWeapon == 1) // TITANYAN
        {
            messageUserAImage.GetComponent<Image>().sprite = titanyanHead;
        }
        else // JAMON
        {
            messageUserAImage.GetComponent<Image>().sprite = jamonHead;
        }
        messageUserAImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        if (deadUser.cUserData.userWeapon == 0)
        {
            messageUserBImage.GetComponent<Image>().sprite = purpHead;
        } else if (deadUser.cUserData.userWeapon == 1)
        {
            messageUserBImage.GetComponent<Image>().sprite = titanyanHead;
        }
        else
        {
            messageUserBImage.GetComponent<Image>().sprite = jamonHead;
        }
        messageUserBImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        messageText.GetComponent<Text>().text = displayMessage;

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
