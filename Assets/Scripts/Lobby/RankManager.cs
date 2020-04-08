using System;
using System.Collections;
using System.Collections.Generic;
using MSBNetwork;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    public GameObject rankFirstNick;
    public GameObject rankFirstRankBadge;
    public GameObject rankFirstRankScore;
    public GameObject rankFirstCharacter;
    public GameObject rankFirstText;
    public GameObject rankFirstWinrate;
    public GameObject rankSecondNick;
    public GameObject rankSecondRankBadge;
    public GameObject rankSecondRankScore;
    public GameObject rankSecondCharacter;
    public GameObject rankSecondText;
    public GameObject rankSecondWinrate;
    public GameObject rankThirdNick;
    public GameObject rankThirdRankBadge;
    public GameObject rankThirdRankScore;
    public GameObject rankThirdCharacter;
    public GameObject rankThirdText;
    public GameObject rankThirdWinrate;
    public GameObject myRankPosition;
    public GameObject myRankHelper;
    public GameObject myCharacter;
    public GameObject myRankSub;
    public GameObject myNick;
    public GameObject myRankBadge;
    public GameObject myRankScore;

    public GameObject miniStatArea;
    public GameObject miniStatusKill;
    public GameObject miniStatusDeath;
    public GameObject miniStatusText;
    public GameObject miniStatusWinrate;
    public GameObject miniDamageBar;
    public GameObject miniDamageBarIndicator;
    public GameObject miniRankPosition;
    public GameObject miniRankHelper;
    public GameObject miniMyRankSub;
    public GameObject miniMyNick;
    public GameObject miniMyRankBadge;
    public GameObject miniMyRankScore;
    public GameObject miniWinnerRankSub;
    public GameObject miniWinnerNick;
    public GameObject miniWinnerRankBadge;
    public GameObject miniWinnerRankScore;
    public GameObject miniLoserRankSub;
    public GameObject miniLoserNick;
    public GameObject miniLoserRankBadge;
    public GameObject miniLoserRankScore;

    public Sprite BADGE_GOLD;
    public Sprite BADGE_SILVER;
    public Sprite BADGE_BRONZE;
    public Sprite CHARACTER_PURPLE;
    public Sprite CHARACTER_JAMON;
    public Sprite CHARACTER_TITANYAN;

    private JObject rankFirstJSON;
    private JObject rankSecondJSON;
    private JObject rankThirdJSON;
    private JObject myJSON;

    public class LeaderBoardListener : NetworkModule.OnSystemResultListener
    {
        private RankManager instance;
        
        public LeaderBoardListener(RankManager _instance)
        {
            instance = _instance;
        }
        
        public void OnSystemNickResult(bool _result, string _data)
        {
            
        }

        public void OnSystemRankResult(bool _result, string _data)
        {
            if (_result)
            {
                instance.OnRankResult(_data);
            }
        }
        
    }
    
    void Start()
    {
        //RequestRank();
    }
    
    void Update()
    {
        
    }

    public void RequestRank()
    {
        NetworkModule.GetInstance().AddOnEventSystem(new LeaderBoardListener(this));
        NetworkModule.GetInstance().RequestUserSystemRank(LocalUser.Instance.localUserData.userID);
    }

    public void OnRankResult(string _data)
    {
        try
        {
            JObject rankData = JObject.Parse(_data);
            JArray globalRankingData = (JArray) rankData.GetValue("global_ranking");
            int rankIndex = 0;
            foreach (JObject globalRankerData in globalRankingData)
            {
                if (rankIndex == 0)
                {
                    rankFirstJSON = globalRankerData;
                    string user_nick = rankFirstJSON.GetValue("user_nick").Value<string>();
                    rankFirstNick.GetComponent<Text>().text = user_nick;
                    int user_rank = rankFirstJSON.GetValue("user_rank").Value<int>();
                    if (user_rank < 800)
                    {
                        rankFirstRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
                    }
                    if(user_rank >= 800 && user_rank < 1200)
                    {
                        rankFirstRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
                    }
                    if(user_rank >= 1200)
                    {
                        rankFirstRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
                    }
                    rankFirstRankScore.GetComponent<Text>().text = user_rank.ToString();
                    int character1 = rankFirstJSON.GetValue("user_character_1").Value<int>();
                    int character2 = rankFirstJSON.GetValue("user_character_2").Value<int>();
                    int character3 = rankFirstJSON.GetValue("user_character_3").Value<int>();
                    if (character1 >= character2 && character1 >= character3)
                    {
                        rankFirstCharacter.GetComponent<Image>().sprite = CHARACTER_PURPLE;
                    }

                    if (character2 >= character1 && character2 >= character3)
                    {
                        rankFirstCharacter.GetComponent<Image>().sprite = CHARACTER_TITANYAN;
                    }

                    if (character3 >= character1 && character3 >= character2)
                    {
                        rankFirstCharacter.GetComponent<Image>().sprite = CHARACTER_JAMON;
                    }

                    int gameTotal = rankFirstJSON.GetValue("user_played").Value<int>();
                    int gameWin = rankFirstJSON.GetValue("user_win").Value<int>();
                    int gameLose = rankFirstJSON.GetValue("user_lose").Value<int>();
                    int gameDraw = gameTotal - gameWin - gameLose;
                    float gameWinrate = 0f;
                    if (gameTotal != 0)
                    {
                        gameWinrate = (float) gameWin / (float) gameTotal * 100;
                    }
                    rankFirstText.GetComponent<Text>().text = String.Format("{0}승 {1}무 {2}패", gameWin, gameDraw , gameLose);
                    rankFirstWinrate.GetComponent<Text>().text = String.Format("{0:F1}%", gameWinrate);
                }

                if (rankIndex == 1)
                {
                    rankSecondJSON = globalRankerData;
                    string user_nick = rankSecondJSON.GetValue("user_nick").Value<string>();
                    rankSecondNick.GetComponent<Text>().text = user_nick;
                    int user_rank = rankSecondJSON.GetValue("user_rank").Value<int>();
                    if (user_rank < 800)
                    {
                        rankSecondRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
                    }
                    if(user_rank >= 800 && user_rank < 1200)
                    {
                        rankSecondRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
                    }
                    if(user_rank >= 1200)
                    {
                        rankSecondRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
                    }
                    rankSecondRankScore.GetComponent<Text>().text = user_rank.ToString();
                    int character1 = rankSecondJSON.GetValue("user_character_1").Value<int>();
                    int character2 = rankSecondJSON.GetValue("user_character_2").Value<int>();
                    int character3 = rankSecondJSON.GetValue("user_character_3").Value<int>();
                    if (character1 >= character2 && character1 >= character3)
                    {
                        rankSecondCharacter.GetComponent<Image>().sprite = CHARACTER_PURPLE;
                    }

                    if (character2 >= character1 && character2 >= character3)
                    {
                        rankSecondCharacter.GetComponent<Image>().sprite = CHARACTER_TITANYAN;
                    }

                    if (character3 >= character1 && character3 >= character2)
                    {
                        rankSecondCharacter.GetComponent<Image>().sprite = CHARACTER_JAMON;
                    }

                    int gameTotal = rankSecondJSON.GetValue("user_played").Value<int>();
                    int gameWin = rankSecondJSON.GetValue("user_win").Value<int>();
                    int gameLose = rankSecondJSON.GetValue("user_lose").Value<int>();
                    int gameDraw = gameTotal - gameWin - gameLose;
                    float gameWinrate = 0f;
                    if (gameTotal != 0)
                    {
                        gameWinrate = (float) gameWin / (float) gameTotal * 100;
                    }
                    rankSecondText.GetComponent<Text>().text = String.Format("{0}승 {1}무 {2}패", gameWin, gameDraw , gameLose);
                    rankSecondWinrate.GetComponent<Text>().text = String.Format("{0:F1}%", gameWinrate);
                }

                if (rankIndex == 2)
                {
                    rankThirdJSON = globalRankerData;
                    string user_nick = rankThirdJSON.GetValue("user_nick").Value<string>();
                    rankThirdNick.GetComponent<Text>().text = user_nick;
                    int user_rank = rankThirdJSON.GetValue("user_rank").Value<int>();
                    if (user_rank < 800)
                    {
                        rankThirdRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
                    }
                    if(user_rank >= 800 && user_rank < 1200)
                    {
                        rankThirdRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
                    }
                    if(user_rank >= 1200)
                    {
                        rankThirdRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
                    }
                    rankThirdRankScore.GetComponent<Text>().text = user_rank.ToString();
                    int character1 = rankThirdJSON.GetValue("user_character_1").Value<int>();
                    int character2 = rankThirdJSON.GetValue("user_character_2").Value<int>();
                    int character3 = rankThirdJSON.GetValue("user_character_3").Value<int>();
                    if (character1 >= character2 && character1 >= character3)
                    {
                        rankThirdCharacter.GetComponent<Image>().sprite = CHARACTER_PURPLE;
                    }

                    if (character2 >= character1 && character2 >= character3)
                    {
                        rankThirdCharacter.GetComponent<Image>().sprite = CHARACTER_TITANYAN;
                    }

                    if (character3 >= character1 && character3 >= character2)
                    {
                        rankThirdCharacter.GetComponent<Image>().sprite = CHARACTER_JAMON;
                    }

                    int gameTotal = rankThirdJSON.GetValue("user_played").Value<int>();
                    int gameWin = rankThirdJSON.GetValue("user_win").Value<int>();
                    int gameLose = rankThirdJSON.GetValue("user_lose").Value<int>();
                    int gameDraw = gameTotal - gameWin - gameLose;
                    float gameWinrate = 0f;
                    if (gameTotal != 0)
                    {
                        gameWinrate = (float) gameWin / (float) gameTotal * 100;
                    }
                    rankThirdText.GetComponent<Text>().text = String.Format("{0}승 {1}무 {2}패", gameWin, gameDraw , gameLose);
                    rankThirdWinrate.GetComponent<Text>().text = String.Format("{0:F1}%", gameWinrate);
                }

                rankIndex++;
            }

            rankData.Remove("global_ranking");
            myJSON = rankData;
            int rankPosition = myJSON.GetValue("user_ranking").Value<int>();
            string rankPositionHelper = "th";
            if (rankPosition == 1) rankPositionHelper = "st";
            if (rankPosition == 2) rankPositionHelper = "nd";
            if (rankPosition == 3) rankPositionHelper = "rd";
            myRankPosition.GetComponent<Text>().text = rankPosition.ToString();
            miniRankPosition.GetComponent<Text>().text = rankPosition.ToString();
            myRankSub.GetComponent<Text>().text = rankPosition.ToString();
            miniMyRankSub.GetComponent<Text>().text = rankPosition.ToString();
            myRankHelper.GetComponent<Text>().text = rankPositionHelper;
            miniRankHelper.GetComponent<Text>().text = rankPositionHelper;
            int characterA = myJSON.GetValue("user_character_1").Value<int>();
            int characterB = myJSON.GetValue("user_character_2").Value<int>();
            int characterC = myJSON.GetValue("user_character_3").Value<int>();
            if (characterA >= characterB && characterA >= characterC)
            {
                myCharacter.GetComponent<Image>().sprite = CHARACTER_PURPLE;
            }

            if (characterB >= characterA && characterB >= characterC)
            {
                myCharacter.GetComponent<Image>().sprite = CHARACTER_TITANYAN;
            }

            if (characterC >= characterA && characterC >= characterB)
            {
                myCharacter.GetComponent<Image>().sprite = CHARACTER_JAMON;
            }

            int totalDamageGive = myJSON.GetValue("user_damage_give").Value<int>();
            int totalDamageTake = myJSON.GetValue("user_damage_take").Value<int>();
            float totalDamageRatio = 1f;
            if (totalDamageTake != 0)
            {
                totalDamageRatio = (float) totalDamageGive / (float) totalDamageTake;
            }
            int damageBarLength = (int) miniDamageBar.GetComponent<RectTransform>().sizeDelta.x;
            Debug.LogWarning("damageBarLength : " + damageBarLength);
            if (totalDamageRatio <= 0f) totalDamageRatio = 0.1f;
            if (totalDamageRatio >= 2f) totalDamageRatio = 1.9f;
            Debug.LogWarning("totalDamageRatio : " + totalDamageRatio);
            int amountX = (int) (damageBarLength / 2 * totalDamageRatio) - (damageBarLength / 2);
            Debug.LogWarning("amountX : " + amountX);
            miniDamageBarIndicator.GetComponent<RectTransform>().localPosition += new Vector3(amountX, 0);
            string nickname = myJSON.GetValue("user_nick").Value<string>();
            myNick.GetComponent<Text>().text = nickname;
            miniMyNick.GetComponent<Text>().text = nickname;
            int rank = myJSON.GetValue("user_rank").Value<int>();
            if (rank < 800)
            {
                myRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
                miniMyRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
            }
            if(rank >= 800 && rank < 1200)
            {
                myRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
                miniMyRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
            }
            if(rank >= 1200)
            {
                myRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
                miniMyRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
            }

            
            myRankScore.GetComponent<Text>().text = rank.ToString();
            miniMyRankScore.GetComponent<Text>().text = rank.ToString();
            int killCount = myJSON.GetValue("user_kill").Value<int>();
            miniStatusKill.GetComponent<Text>().text = killCount.ToString();
            int deathCount = myJSON.GetValue("user_death").Value<int>();
            miniStatusDeath.GetComponent<Text>().text = deathCount.ToString();
            int totalGame = myJSON.GetValue("user_played").Value<int>();
            int winGame = myJSON.GetValue("user_win").Value<int>();
            int loseGame = myJSON.GetValue("user_lose").Value<int>();
            int drawGame = totalGame - winGame - loseGame;
            float winRate = 0f;
            if (totalGame != 0)
            {
                winRate = (float) winGame / (float) totalGame * 100;
            }
            else
            {
                winRate = 50f;
            }
            miniStatusText.GetComponent<Text>().text = String.Format("{0}승 {1}무 {2}패", winGame, drawGame , loseGame);
            miniStatusWinrate.GetComponent<Text>().text = String.Format("{0:F1}%", winRate);
            int winnerRanking = rankPosition - 1;
            int loserRanking = rankPosition + 1;
            int winnerRank = myJSON.GetValue("winner_rank").Value<int>();
            int loserRank = myJSON.GetValue("loser_rank").Value<int>();
            string winnerNick = myJSON.GetValue("winner_nick").Value<string>();
            string loserNick = myJSON.GetValue("loser_nick").Value<string>();
            int winnerCharacter1 = myJSON.GetValue("winner_character_1").Value<int>();
            int winnerCharacter2 = myJSON.GetValue("winner_character_2").Value<int>();
            int winnerCharacter3 = myJSON.GetValue("winner_character_3").Value<int>();
            int loserCharacter1 = myJSON.GetValue("loser_character_1").Value<int>();
            int loserCharacter2 = myJSON.GetValue("loser_character_2").Value<int>();
            int loserCharacter3 = myJSON.GetValue("loser_character_3").Value<int>();
            miniWinnerRankSub.GetComponent<Text>().text = winnerRanking.ToString();
            miniLoserRankSub.GetComponent<Text>().text = loserRanking.ToString();
            Debug.LogWarning("RankManager DEBUG : WINNER NICK : " + winnerNick);
            Debug.LogWarning("RankManager DEBUG : LOSER NICK : " + loserNick);
            if (winnerNick == null || winnerNick.Trim().Length == 0) miniWinnerNick.GetComponent<Text>().text = "NONAME";
            else miniWinnerNick.GetComponent<Text>().text = winnerNick;
            if (loserNick == null || loserNick.Trim().Length == 0) miniLoserNick.GetComponent<Text>().text = "NONAME";
            else miniLoserNick.GetComponent<Text>().text = loserNick;
            if (winnerRank < 800)
            {
                miniWinnerRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
            }
            if(winnerRank >= 800 && winnerRank < 1200)
            {
                miniWinnerRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
            }
            if(winnerRank >= 1200)
            {
                miniWinnerRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
            }
            if (loserRank < 800)
            {
                miniLoserRankBadge.GetComponent<Image>().sprite = BADGE_BRONZE;
            }
            if(loserRank >= 800 && loserRank < 1200)
            {
                miniLoserRankBadge.GetComponent<Image>().sprite = BADGE_SILVER;
            }
            if(loserRank >= 1200)
            {
                miniLoserRankBadge.GetComponent<Image>().sprite = BADGE_GOLD;
            }
            miniWinnerRankScore.GetComponent<Text>().text = winnerRank.ToString();
            miniLoserRankScore.GetComponent<Text>().text = loserRank.ToString();
        }
        catch (Exception e)
        {
            Debug.LogWarning("RankManager ERROR : " + e.Message);
            Debug.LogWarning(_data);
        }
    }
    
}
