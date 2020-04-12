using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MedalManager : MonoBehaviour
{
    public GameObject medalImage1;
    public GameObject medalImage2;
    public GameObject medalImage3;
    public GameObject medalImage4;
    public GameObject medalImage5;
    public GameObject medalImage6;
    public GameObject medalImage7;
    public GameObject medalImage8;
    public GameObject medalImage9;
    public GameObject medalImage10;
    public GameObject medalImage11;
    public GameObject medalImage12;
    public GameObject medalText1;
    public GameObject medalText2;
    public GameObject medalText3;
    public GameObject medalText4;
    public GameObject medalText5;
    public GameObject medalText6;
    public GameObject medalText7;
    public GameObject medalText8;
    public GameObject medalText9;
    public GameObject medalText10;
    public GameObject medalText11;
    public GameObject medalText12;

    public GameObject medalDetailPanel;
    public GameObject medalDetailImage;
    public GameObject medalDetailText;
    public GameObject medalDetailMessage;

    public Sprite MEDAL_1_ON;
    public Sprite MEDAL_1_OFF;
    public Sprite MEDAL_2_ON;
    public Sprite MEDAL_2_OFF;
    public Sprite MEDAL_3_ON;
    public Sprite MEDAL_3_OFF;
    public Sprite MEDAL_4_ON;
    public Sprite MEDAL_4_OFF;
    public Sprite MEDAL_5_ON;
    public Sprite MEDAL_5_OFF;
    public Sprite MEDAL_6_ON;
    public Sprite MEDAL_6_OFF;
    public Sprite MEDAL_7_ON;
    public Sprite MEDAL_7_OFF;
    public Sprite MEDAL_8_ON;
    public Sprite MEDAL_8_OFF;
    public Sprite MEDAL_9_ON;
    public Sprite MEDAL_9_OFF;
    public Sprite MEDAL_10_ON;
    public Sprite MEDAL_10_OFF;
    public Sprite MEDAL_11_ON;
    public Sprite MEDAL_11_OFF;
    public Sprite MEDAL_12_ON;
    public Sprite MEDAL_12_OFF;

    public GameObject achievementProgressBackground;
    public GameObject achievementProgressForeground;
    public GameObject achievementProgressText;
    
    public GameObject achievementSelectedImage;
    public GameObject achievementSelectedTitle;
    public GameObject achievementSelectedContent;
    public GameObject achievementSelectedPercentage;
    
    public GameObject achievementHolder1;
    public GameObject achievementHolder2;
    public GameObject achievementHolder3;
    public GameObject achievementHolder4;
    public GameObject achievementHolder5;
    public GameObject achievementHolder6;
    public GameObject achievementHolder7;
    public GameObject achievementHolder8;
    public GameObject achievementHolder9;
    public GameObject achievementContent1;
    public GameObject achievementContent2;
    public GameObject achievementContent3;
    public GameObject achievementContent4;
    public GameObject achievementContent5;
    public GameObject achievementContent6;
    public GameObject achievementContent7;
    public GameObject achievementContent8;
    public GameObject achievementContent9;
    public GameObject achievementPercentage1;
    public GameObject achievementPercentage2;
    public GameObject achievementPercentage3;
    public GameObject achievementPercentage4;
    public GameObject achievementPercentage5;
    public GameObject achievementPercentage6;
    public GameObject achievementPercentage7;
    public GameObject achievementPercentage8;
    public GameObject achievementPercentage9;

    public Sprite ACHIEVEMENT_CLEAR;
    public Sprite ACHIEVEMENT_UNCLEAR;
    public Sprite ACHIEVEMENT_BRONZE;
    public Sprite ACHIEVEMENT_SILVER;
    public Sprite ACHIEVEMENT_GOLD;

    public GameObject blinkTextMedal;
    public GameObject blinkTextAchievement;

    private JObject medalJSON;

    public class MedalListener : NetworkModule.OnSystemResultListener
    {
        private MedalManager instance;
        
        public MedalListener(MedalManager _instance)
        {
            instance = _instance;
        }
        
        public void OnSystemNickResult(bool _result, string _data)
        {
            
        }

        public void OnSystemRankResult(bool _result, string _data)
        {
            
        }

        public void OnSystemMedalResult(bool _result, string _data)
        {
            if (_result)
            {
                instance.OnMedalResult(_data);
            }
        }
    }
    
    void Start()
    {
        medalDetailPanel.SetActive(false);
    }
    
    void Update()
    {
        
    }

    public void RequestMedal()
    {
        NetworkModule.GetInstance().AddOnEventSystem(new MedalListener(this));
        NetworkModule.GetInstance().RequestUserSystemMedal(LocalUser.Instance.localUserData.userNumber);
    }

    public void OnMedalResult(string _data)
    {
        try
        {
            medalImage1.GetComponent<Image>().sprite = MEDAL_1_OFF;
            medalImage1.GetComponent<Image>().preserveAspect = true;
            medalImage2.GetComponent<Image>().sprite = MEDAL_2_OFF;
            medalImage2.GetComponent<Image>().preserveAspect = true;
            medalImage3.GetComponent<Image>().sprite = MEDAL_3_OFF;
            medalImage3.GetComponent<Image>().preserveAspect = true;
            medalImage4.GetComponent<Image>().sprite = MEDAL_4_OFF;
            medalImage4.GetComponent<Image>().preserveAspect = true;
            medalImage5.GetComponent<Image>().sprite = MEDAL_5_OFF;
            medalImage5.GetComponent<Image>().preserveAspect = true;
            medalImage6.GetComponent<Image>().sprite = MEDAL_6_OFF;
            medalImage6.GetComponent<Image>().preserveAspect = true;
            medalImage7.GetComponent<Image>().sprite = MEDAL_7_OFF;
            medalImage7.GetComponent<Image>().preserveAspect = true;
            medalImage8.GetComponent<Image>().sprite = MEDAL_8_OFF;
            medalImage8.GetComponent<Image>().preserveAspect = true;
            medalImage9.GetComponent<Image>().sprite = MEDAL_9_OFF;
            medalImage9.GetComponent<Image>().preserveAspect = true;
            medalImage10.GetComponent<Image>().sprite = MEDAL_10_OFF;
            medalImage10.GetComponent<Image>().preserveAspect = true;
            medalImage11.GetComponent<Image>().sprite = MEDAL_11_OFF;
            medalImage11.GetComponent<Image>().preserveAspect = true;
            medalImage12.GetComponent<Image>().sprite = MEDAL_12_OFF;
            medalImage12.GetComponent<Image>().preserveAspect = true;
            medalText1.GetComponent<Text>().text = "0";
            medalText2.GetComponent<Text>().text = "0";
            medalText3.GetComponent<Text>().text = "0";
            medalText4.GetComponent<Text>().text = "0";
            medalText5.GetComponent<Text>().text = "0";
            medalText6.GetComponent<Text>().text = "0";
            medalText7.GetComponent<Text>().text = "0";
            medalText8.GetComponent<Text>().text = "0";
            medalText9.GetComponent<Text>().text = "0";
            medalText10.GetComponent<Text>().text = "0";
            medalText11.GetComponent<Text>().text = "0";
            medalText12.GetComponent<Text>().text = "0";
            JObject medalData = JObject.Parse(_data);
            JArray medalArray = (JArray) medalData.GetValue("medal_status");
            int rankIndex = 0;
            foreach (JObject medalObject in medalArray)
            {
                int medal_type = medalObject.GetValue("medal_type").Value<int>();
                int medal_count = medalObject.GetValue("medal_type").Value<int>();
                if (medal_count == 0) continue;
                switch (medal_type)
                {
                    case 1: // 선취점
                        medalImage1.GetComponent<Image>().sprite = MEDAL_1_ON;
                        medalText1.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 2: // 연파
                        medalImage2.GetComponent<Image>().sprite = MEDAL_2_ON;
                        medalText2.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 3: // 눈에는 눈, 이에는 이
                        medalImage3.GetComponent<Image>().sprite = MEDAL_3_ON;
                        medalText3.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 4: // 금강불괴
                        medalImage4.GetComponent<Image>().sprite = MEDAL_4_ON;
                        medalText4.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 5: // 나는 미치지 않았어
                        medalImage5.GetComponent<Image>().sprite = MEDAL_5_ON;
                        medalText5.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 6: // 어시스트
                        medalImage6.GetComponent<Image>().sprite = MEDAL_6_ON;
                        medalText6.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 7: // 화력의 증표 : 동
                        medalImage7.GetComponent<Image>().sprite = MEDAL_7_ON;
                        medalText7.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 8: // 화력의 증표 : 은
                        medalImage8.GetComponent<Image>().sprite = MEDAL_8_ON;
                        medalText8.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 9: // 화력의 증표 : 금
                        medalImage9.GetComponent<Image>().sprite = MEDAL_9_ON;
                        medalText9.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 10: // 수호의 증표 : 동
                        medalImage10.GetComponent<Image>().sprite = MEDAL_10_ON;
                        medalText10.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 11: // 수호의 증표 : 은
                        medalImage11.GetComponent<Image>().sprite = MEDAL_11_ON;
                        medalText11.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    case 12: // 수호의 증표 : 금
                        medalImage12.GetComponent<Image>().sprite = MEDAL_12_ON;
                        medalText12.GetComponent<Text>().text = medal_count.ToString();
                        break;
                    default:
                        break;
                }
            }
            
            int user_played = medalData.GetValue("user_played").Value<int>();
            int user_win = medalData.GetValue("user_win").Value<int>();
            int user_kill = medalData.GetValue("user_kill").Value<int>();
            int user_death = medalData.GetValue("user_death").Value<int>();
            int user_assist = medalData.GetValue("user_assist").Value<int>();
            int user_damage_give = medalData.GetValue("user_damage_give").Value<int>();
            int user_damage_take = medalData.GetValue("user_damage_take").Value<int>();
            int user_character_1 = medalData.GetValue("user_character_1").Value<int>();
            int user_character_2 = medalData.GetValue("user_character_2").Value<int>();
            int user_character_3 = medalData.GetValue("user_character_3").Value<int>();
            int user_character_1_win = medalData.GetValue("user_character_1_win").Value<int>();
            int user_character_2_win = medalData.GetValue("user_character_2_win").Value<int>();
            int user_character_3_win = medalData.GetValue("user_character_3_win").Value<int>();

            int successCount = 0;
            
            if (user_character_1_win >= 5)
            {
                achievementHolder7.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent7.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_1_win * 100 / 5;
                achievementPercentage7.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_1_win >= 20)
            {
                achievementHolder8.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent8.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_1_win * 100 / 20;
                achievementPercentage8.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_1_win >= 50)
            {
                achievementHolder9.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent9.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_1_win * 100 / 50;
                achievementPercentage9.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_2_win >= 5)
            {
                achievementHolder4.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent4.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_2_win * 100 / 5;
                achievementPercentage4.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_2_win >= 20)
            {
                achievementHolder5.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent5.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_2_win * 100 / 20;
                achievementPercentage5.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_2_win >= 50)
            {
                achievementHolder6.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent6.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_2_win * 100 / 50;
                achievementPercentage6.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_3_win >= 5)
            {
                achievementHolder1.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent1.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_3_win * 100 / 5;
                achievementPercentage1.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_3_win >= 20)
            {
                achievementHolder2.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent2.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_3_win * 100 / 20;
                achievementPercentage2.GetComponent<Text>().text = percent + "%";
            }
            
            if (user_character_3_win >= 50)
            {
                achievementHolder3.GetComponent<Image>().sprite = ACHIEVEMENT_CLEAR;
                achievementContent3.GetComponent<Text>().color = Color.white;
                successCount++;
            }
            else
            {
                int percent = user_character_3_win * 100 / 50;
                achievementPercentage3.GetComponent<Text>().text = percent + "%";
            }

            int totalPercentage = successCount * 100 / 9;
            float progressWidth = achievementProgressBackground.GetComponent<RectTransform>().rect.width;
            float foregroundWidth = progressWidth * totalPercentage / 100;
            achievementProgressForeground.GetComponent<RectTransform>().sizeDelta = new Vector2(foregroundWidth, 1);
            achievementProgressText.GetComponent<Text>().text = totalPercentage + "%";
            
            selectAchievement(1);
        }
        catch (Exception e)
        {
            Debug.LogWarning("RankManager ERROR : " + e.Message);
            Debug.LogWarning(_data);
        }
    }

    public void showDetailMedal(int index)
    {
        medalDetailPanel.SetActive(true);
        medalDetailImage.GetComponent<Image>().preserveAspect = true;
        switch (index)
        {
            case 1: // 선취점
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_1_ON;
                medalDetailText.GetComponent<Text>().text = "선취점";
                medalDetailMessage.GetComponent<Text>().text = "게임에서 가장 먼저 적을 처치";
                break;
            case 2: // 연파
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_2_ON;
                medalDetailText.GetComponent<Text>().text = "연파";
                medalDetailMessage.GetComponent<Text>().text = "5초 이내에 두 명의 적을 처치";
                break;
            case 3: // 눈에는 눈, 이에는 이
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_3_ON;
                medalDetailText.GetComponent<Text>().text = "눈에는 눈, 이에는 이";
                medalDetailMessage.GetComponent<Text>().text = "자신을 처치한 플레이어를 곧바로 처치";
                break;
            case 4: // 금강불괴
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_4_ON;
                medalDetailText.GetComponent<Text>().text = "금강불괴";
                medalDetailMessage.GetComponent<Text>().text = "자신의 최대 체력의 1.5배의 데미지를 누적으로 받고 생존";
                break;
            case 5: // 나는 미치지 않았어
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_5_ON;
                medalDetailText.GetComponent<Text>().text = "나는 미치지 않았어";
                medalDetailMessage.GetComponent<Text>().text = "게임 종료 시 0킬";
                break;
            case 6: // 어시스트
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_6_ON;
                medalDetailText.GetComponent<Text>().text = "어시스트";
                medalDetailMessage.GetComponent<Text>().text = "자신이 50% 이상 체력을 깎은 적을 아군이 처치";
                break;
            case 7: // 화력의 증표 : 동
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_7_ON;
                medalDetailText.GetComponent<Text>().text = "화력의 증표 : 동";
                medalDetailMessage.GetComponent<Text>().text = "[1vs1]\n자신의 총 데미지가 게임 총 데미지의 60% 이상\n[3vs3]\n자신의 총 데미지가 게임 총 데미지의 25% 이상";
                break;
            case 8: // 화력의 증표 : 은
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_8_ON;
                medalDetailText.GetComponent<Text>().text = "화력의 증표 : 은";
                medalDetailMessage.GetComponent<Text>().text = "[1vs1]\n자신의 총 데미지가 게임 총 데미지의 70% 이상\n[3vs3]\n자신의 총 데미지가 게임 총 데미지의 35% 이상";
                break;
            case 9: // 화력의 증표 : 금
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_9_ON;
                medalDetailText.GetComponent<Text>().text = "화력의 증표 : 금";
                medalDetailMessage.GetComponent<Text>().text = "[1vs1]\n자신의 총 데미지가 게임 총 데미지의 80% 이상\n[3vs3]\n자신의 총 데미지가 게임 총 데미지의 50% 이상";
                break;
            case 10: // 수호의 증표 : 동
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_10_ON;
                medalDetailText.GetComponent<Text>().text = "수호의 증표 : 동";
                medalDetailMessage.GetComponent<Text>().text = "[1vs1]\n자신이 받은 데미지가 게임 총 데미지의 60% 이상\n[3vs3]\n자신이 받은 데미지가 게임 총 데미지의 25% 이상";
                break;
            case 11: // 수호의 증표 : 은
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_11_ON;
                medalDetailText.GetComponent<Text>().text = "수호의 증표 : 은";
                medalDetailMessage.GetComponent<Text>().text = "[1vs1]\n자신이 받은 데미지가 게임 총 데미지의 70% 이상\n[3vs3]\n자신이 받은 데미지가 게임 총 데미지의 35% 이상";
                break;
            case 12: // 수호의 증표 : 금
                medalDetailImage.GetComponent<Image>().sprite = MEDAL_12_ON;
                medalDetailText.GetComponent<Text>().text = "수호의 증표 : 금";
                medalDetailMessage.GetComponent<Text>().text = "[1vs1]\n자신이 받은 데미지가 게임 총 데미지의 80% 이상\n[3vs3]\n자신이 받은 데미지가 게임 총 데미지의 50% 이상";
                break;
            default:
                break;
        }
    }

    public void hideDetailPanel()
    {
        medalDetailPanel.SetActive(false);
    }

    public void selectAchievement(int index)
    {
        switch (index)
        {
            case 1:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_BRONZE;
                achievementSelectedTitle.GetComponent<Text>().text = "견습닌자";
                achievementSelectedContent.GetComponent<Text>().text = "자몽으로 5회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage1.GetComponent<Text>().text;
                break;
            case 2:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_SILVER;
                achievementSelectedTitle.GetComponent<Text>().text = "숙련닌자";
                achievementSelectedContent.GetComponent<Text>().text = "자몽으로 20회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage2.GetComponent<Text>().text;
                break;
            case 3:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_GOLD;
                achievementSelectedTitle.GetComponent<Text>().text = "장인닌자";
                achievementSelectedContent.GetComponent<Text>().text = "자몽으로 50회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage3.GetComponent<Text>().text;
                break;
            case 4:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_BRONZE;
                achievementSelectedTitle.GetComponent<Text>().text = "아깽이";
                achievementSelectedContent.GetComponent<Text>().text = "티타냥으로 5회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage4.GetComponent<Text>().text;
                break;
            case 5:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_SILVER;
                achievementSelectedTitle.GetComponent<Text>().text = "캣초딩";
                achievementSelectedContent.GetComponent<Text>().text = "티타냥으로 20회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage5.GetComponent<Text>().text;
                break;
            case 6:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_GOLD;
                achievementSelectedTitle.GetComponent<Text>().text = "냥아치";
                achievementSelectedContent.GetComponent<Text>().text = "티타냥으로 50회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage6.GetComponent<Text>().text;
                break;
            case 7:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_BRONZE;
                achievementSelectedTitle.GetComponent<Text>().text = "퍼프 MK.I";
                achievementSelectedContent.GetComponent<Text>().text = "퍼프로 5회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage7.GetComponent<Text>().text;
                break;
            case 8:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_SILVER;
                achievementSelectedTitle.GetComponent<Text>().text = "퍼프 MK.IV";
                achievementSelectedContent.GetComponent<Text>().text = "퍼프로 20회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage8.GetComponent<Text>().text;
                break;
            case 9:
                achievementSelectedImage.GetComponent<Image>().sprite = ACHIEVEMENT_GOLD;
                achievementSelectedTitle.GetComponent<Text>().text = "퍼프 MK.IX";
                achievementSelectedContent.GetComponent<Text>().text = "퍼프로 50회 승리";
                achievementSelectedPercentage.GetComponent<Text>().text = achievementPercentage9.GetComponent<Text>().text;
                break;
            default:
                break;
        }
    }
    
}
