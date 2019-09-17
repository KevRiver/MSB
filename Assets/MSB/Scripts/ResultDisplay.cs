using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultDisplay : MonoBehaviour
{
    public bool result;
    public string kill;
    public string point;

    public Text playerName;
    public Text killValue;
    public Text pointValue;

    public GameObject winImage;
    public GameObject loseImage;
    public GameObject draw;

    const int SKIN_PURPLE = 0;
    const int SKIN_ORANGE = 1;
    public Sprite[] sprites;
    public Image playerImage;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject resultObj = GameObject.Find("ResultData");
        ResultData resultData = resultObj.GetComponent<ResultData>();
        if (resultData != null)
        {
            Debug.LogWarning("ResultData : " + resultData.kill + " , " + resultData.point);
        }

        LocalUser localUser = LocalUser.Instance;

        if (resultData == null || localUser == null)
        {
            Debug.LogWarning("Data NULL");
            return;
        }

        int skinIndex = localUser.localUserData.userWeapon;
        playerImage.sprite = sprites[skinIndex];

        switch (resultData.result)
        {
            case ResultData.Result.WIN:
                winImage.SetActive(true);
                break;
            case ResultData.Result.DRAW:
                draw.SetActive(true);
                break;
            case ResultData.Result.LOSE:
                loseImage.SetActive(true);
                break;
            default:
                Debug.LogError("Strange Value");
                break;
        }
        playerName.text = LocalUser.Instance.localUserData.userNick;
        killValue.text = resultData.kill.ToString();
        pointValue.text = resultData.point.ToString();

        Destroy(resultObj);

    }
    
}
