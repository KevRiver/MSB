#define ACHIEVE_LOG_ON
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MSBNetwork;

public struct AchievementViewData
{
    public int medalIndex;

    public AchievementViewData(int medalIndex)
    {
        this.medalIndex = medalIndex;
    }
}

public class AchievementView : MonoBehaviour,MSB_View<AchievementViewData>
{
    public Image AchievementBadge;
    public Text AchievementMessage;
    public Text AchievementTitle;
    public Button NextButton;
    [Header("Resource Cache")]
    public Sprite[] AchievementBadges;

    private const int NONE = 0;
    private const int ATTBRONZE = 1;
    private const int ATTSILVER = 2;
    private const int ATTGOLD = 3;
    private const int DEFBRONZE = 4;
    private const int DEFSILVER = 5;
    private const int DEFGOLD = 6;
    public void Initialize(AchievementViewData data)
    {
        ApplyData(data);
        NextButton.onClick.AddListener(NextButtonOnClickMethod);
    }

    private void NextButtonOnClickMethod()
    {
        AsyncSceneManager.ActivateScene();
    }

    public void ApplyData(AchievementViewData data)
    {
#if ACHIEVE_LOG_ON
        Debug.LogFormat("Medal Index : {0}",data.medalIndex);
#endif
        int medalIndex = data.medalIndex;
        int achievementBadgeIndex = 0;
        string achievementMessage = "증표 획득";
        string achievementTitle = "";
        Vector2 origin = AchievementBadge.rectTransform.sizeDelta;
        switch (medalIndex)
        {
            case 7:
                achievementBadgeIndex = ATTBRONZE;
                achievementTitle = "화력의 증표 : 동";
                break;
            case 8:
                achievementBadgeIndex = ATTSILVER;
                achievementTitle = "화력의 증표 : 은";
                break;
            case 9:
                achievementBadgeIndex = ATTGOLD;
                achievementTitle = "화력의 증표 : 금";
                break;
            case 10:
                achievementBadgeIndex = DEFBRONZE;
                achievementTitle = "수호의 증표 : 동";
                break;
            case 11:
                achievementBadgeIndex = DEFSILVER;
                achievementTitle = "수호의 증표 : 은";
                break;
            case 12:
                achievementBadgeIndex = DEFGOLD;
                achievementTitle = "수호의 증표 : 금";
                break;
            default:
                achievementBadgeIndex = NONE;
                achievementMessage = "획득한 증표가 없습니다";
                break;
        }
        AchievementBadge.sprite = AchievementBadges[achievementBadgeIndex];
        AchievementBadge.SetNativeSize();

        RectTransform rt = AchievementBadge.rectTransform;
        Vector2 native = rt.sizeDelta;
        float ny = origin.y;
        float nx = native.x * origin.y / native.y;
        rt.sizeDelta = new Vector2(nx, ny);
        
        AchievementMessage.text = achievementMessage;
        AchievementTitle.text = achievementTitle;
    }
}
