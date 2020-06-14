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

    [Header("View Component Ref")]
    public GameObject Score;
    public Text BlueScore;
    public Text RedScore;
    public Text Result;
    public Text Rank;
    public int IntRankTemp;
    public int IntRank;
    public Image RankPanel;
    public Image RankBadge;
    public Text RankDelta;
    public Image RankDeltaImage;
    public Button NextButton;

    private Animator _animator;
    [Header("Animation Trigger")]
    public string Trigger;
    private void NextButtonClicked()
    {
        MSB_GUIManager guiManager = MSB_GUIManager.Instance;
        StartCoroutine(ViewTransition());
    }

    private IEnumerator ViewTransition()
    {
        MSB_GUIManager guiManager = MSB_GUIManager.Instance;
        yield return StartCoroutine(guiManager.SecondCoverFadeIn(1.0f));
        guiManager.ViewActive(1, true);
        guiManager.ViewActive(0,false);
    }

    public void Initialize(GameResultViewData data)
    {
        StartCoroutine(AsyncLoadScene("Scenes/Lobby"));
        MSB_GUIManager guiManager = MSB_GUIManager.Instance;
        guiManager.CoverActive(true);
        ApplyData(data);
        _animator = GetComponent<Animator>();
        _animator.SetTrigger(Trigger);
        NextButton.onClick.AddListener(NextButtonClicked);
    }

    public void StartScoreChange()
    {
        StartCoroutine(ScoreChange());
    }

    private IEnumerator ScoreChange()
    {
        // IntRank : savedRank / IntRankTemp : changedRank
        int sign = IntRank - IntRankTemp > 0 ? -1 : 1;
        while (IntRank != IntRankTemp)
        {
            IntRank += sign;
            Rank.text = IntRank.ToString();
            yield return null;
        }
    }

    IEnumerator AsyncLoadScene(string sceneName)
    {
        AsyncSceneManager.operation = SceneManager.LoadSceneAsync(sceneName);
        AsyncOperation ao = AsyncSceneManager.operation;
        ao.allowSceneActivation = false;
        while (!ao.isDone)
        {
            if (ao.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }
        NextButton.gameObject.SetActive(true);
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
        int rank = data._savedRank;
        IntRankTemp = data._rank;
        IntRank = data._savedRank;
        int rankDelta = IntRankTemp - data._savedRank;
        int badgeIndex = BadgeIndex(data._rank);
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
        
        // rescale by origin height;
        float ny = origin.y;
        float nx = native.x * origin.y / native.y;
        rt.sizeDelta = new Vector2(nx,ny);

        RectTransform rankDeltaRectTransform = RankDeltaImage.rectTransform;
        if (rankDelta < 0)
        {
            rankDeltaRectTransform.Rotate(Vector3.right, 180f);
            RankDelta.color = new Color(255f / 255f, 75f / 255f, 0f);
        }
        RankDelta.text = Mathf.Abs(rankDelta).ToString();
    }
}
