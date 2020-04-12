using System;
using System.Collections;
using System.Collections.Generic;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MedalMessageHandler : MonoBehaviour
{
    public GameObject messageImage;
    public GameObject messageTitle;
    public GameObject messageText;

    public Sprite medal1;
    public Sprite medal2;
    public Sprite medal3;
    public Sprite medal4;
    public Sprite medal5;
    public Sprite medal6;
    public Sprite medalAtt1;
    public Sprite medalAtt2;
    public Sprite medalAtt3;
    public Sprite medalDef1;
    public Sprite medalDef2;
    public Sprite medalDef3;

    private class OnGameStatus : NetworkModule.OnGameStatusListener
    {
        private MedalMessageHandler _medalMessageHandler;

        public OnGameStatus(MedalMessageHandler medalMessageHandler)
        {
            this._medalMessageHandler = medalMessageHandler;
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
            if (type == 2) _medalMessageHandler.DisplayKillMessage(type, message);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        messageImage = GameObject.Find("MessageImage");
        messageTitle = GameObject.Find("MessageTitle");
        messageText = GameObject.Find("MessageText");

        messageImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        messageTitle.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        messageText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0f);
        
        NetworkModule.GetInstance().AddOnEventGameStatus(new OnGameStatus(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayKillMessage(int type, string message)
    {
        int medalIndex = Convert.ToInt32(message);
        string medalTitle = string.Empty, medalText = String.Empty;
        switch (medalIndex)
        {
            case 1:
                messageImage.GetComponent<Image>().sprite = medal1;
                medalTitle = "선취점";
                //medalText = "더할 나위 없는 스타트!";
                break;
            case 2:
                messageImage.GetComponent<Image>().sprite = medal2;
                medalTitle = "연파";
                //medalText = "5초면 충분합니다";
                break;
            case 3:
                messageImage.GetComponent<Image>().sprite = medal3;
                medalTitle = "눈에는 눈, 이에는 이";
                //medalText = "복수";
                break;
            case 4:
                messageImage.GetComponent<Image>().sprite = medal4;
                medalTitle = "금강불괴";
                //medalText = "내가 이렇게까지 게임합니다";
                break;
            case 5:
                messageImage.GetComponent<Image>().sprite = medal5;
                medalTitle = "나는 미치지 않았어";
                //medalText = "이건 좋은 훈장일까요?";
                break;
            case 6:
                messageImage.GetComponent<Image>().sprite = medal6;
                medalTitle = "어시스트";
                //medalText = "솔직히 0.9킬";
                break;
            case 7:
                messageImage.GetComponent<Image>().sprite = medalAtt1;
                medalTitle = "화력의 증표 : 동";
                //medalText = "완전히 날아다녔습니다";
                break;
            case 8:
                messageImage.GetComponent<Image>().sprite = medalAtt2;
                medalTitle = "화력의 증표 : 은";
                //medalText = "게임을 완전히 지배했습니다";
                break;
            case 9:
                messageImage.GetComponent<Image>().sprite = medalAtt3;
                medalTitle = "화력의 증표 : 금";
                //medalText = "이 게임은 당신을 위해 존재합니다";
                break;
            case 10:
                messageImage.GetComponent<Image>().sprite = medalDef1;
                medalTitle = "수호의 증표 : 동";
                //medalText = "이 판 내가 혼자 다 했습니다";
                break;
            case 11:
                messageImage.GetComponent<Image>().sprite = medalDef2;
                medalTitle = "수호의 증표 : 은";
                //medalText = "팀원들덕에 내가 이렇게까지 게임합니다";
                break;
            case 12:
                messageImage.GetComponent<Image>().sprite = medalDef3;
                medalTitle = "수호의 증표 : 금";
                //medalText = "솔직히, 이정도면 팀원들에게 공을 돌립니다";
                break;
        }

        messageImage.GetComponent<Image>().preserveAspect = true;
        messageImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        messageTitle.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
        messageText.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
        messageTitle.GetComponent<Text>().text = medalTitle;
        messageText.GetComponent<Text>().text = medalText;
        
        StartCoroutine(fadeObject(false, messageImage.GetComponent<Image>(), 1f, 1f));
        StartCoroutine(fadeObject(false, messageTitle.GetComponent<Text>(), 1f, 1f));
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
