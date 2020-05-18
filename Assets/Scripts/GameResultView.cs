using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct GameResultViewData
{
    public int _savedRank;
    public int _rank;
    public int _blueScore;
    public int _redScore;
    public string _result;

    public GameResultViewData(int savedRank,int rank, int blueScore, int redScore,string result)
    {
        _savedRank = savedRank;
        _rank = rank;
        _blueScore = blueScore;
        _redScore = redScore;
        _result = result;
    }
}

public class GameResultView : MonoBehaviour, MSB_View<GameResultViewData>
{
    private const int BRONZE = 0;
    private const int SILVER = 1;
    private const int GOLD = 2;
    
    private const int DOWN = 0;
    private const int UP = 1;

    private int savedRankPoint;
    private MSB_GameManager.Team _team;
    
    [Header("Resource Cache")]
    public Sprite[] RankBadgeSprites;
    public Sprite[] RankDeltaSprites;

    [Header("View Component Ref")]
    public GameObject Score;
    public Text BlueScore;
    public Text RedScore;
    public Text Result;
    public Text Rank;
    public Image RankPanel;
    public Image RankBadge;
    public Text RankDelta;
    public Image RankDeltaImage;
    public Button NextButton;
    
    private void NextButtonClicked()
    {
        Debug.LogWarning("GameResultView NextButton Clicked");
        MSB_GUIManager guiManager = MSB_GUIManager.Instance;
        guiManager.ViewActive(0,false);
        guiManager.ViewActive(1, true);
        guiManager.SetCover(Color.black, 1f); 
    }



    public void Initialize(GameResultViewData data)
    {

        MSB_GUIManager guiManager = MSB_GUIManager.Instance;
        guiManager.LoadLobbyScene();
        guiManager.CoverActive(true);
        ApplyData(data);
        NextButton.onClick.AddListener(NextButtonClicked);
    }

    private readonly Func<int, int> BadgeIndex = rank =>
    {
        if (rank <= 800) return BRONZE;
        if (rank <= 1200) return SILVER;
        return GOLD;
    };
    public void ApplyData(GameResultViewData data)
    {
        int blueScore = data._blueScore;
        int redScore = data._redScore;
        int rank = data._rank;
        int rankDelta = rank - data._savedRank;
        int badgeIndex = BadgeIndex(rank);
        string result = data._result;
        Vector2 origin = RankBadge.rectTransform.sizeDelta;
        BlueScore.text = blueScore.ToString();
        RedScore.text = redScore.ToString();
        Result.text = result;
        Rank.text = rank.ToString();
        
        // set ui image native size
        RectTransform rt = RankBadge.rectTransform;
        RankBadge.sprite = RankBadgeSprites[badgeIndex];
        RankBadge.SetNativeSize();
        Vector2 native = rt.sizeDelta;
        
        // rescale with origin height;
        float ny = origin.y;
        float nx = native.x * origin.y / native.y;
        RankBadge.rectTransform.sizeDelta = new Vector2(nx,ny);
        
        RankDeltaImage.sprite = rankDelta >= 0 ? RankDeltaSprites[UP] : RankDeltaSprites[DOWN];
        RankDelta.text = Mathf.Abs(rankDelta).ToString();
    }
}
