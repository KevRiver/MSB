using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;

public class ScoreItem : MonoBehaviour
{
    const int SCORE_ITEM = 0;
    const int HEAL_PACK = 1;


    public int itemIndex;
    ScoreItemListener scoreItemListener;

    class ScoreItemListener : NetworkModule.OnGameEventListener
    {
        public GameObject scoreItemObj;
        public ScoreItem scoreItem;
        public void OnGameEventDamage(int from, int to, int amount, string option) { }

        public void OnGameEventHealth(int num, int health) { }

        
        public void OnGameEventItem(int type, int num, int action)
        {
            //Debug.LogWarning("***OnGameEventItem***");
            if (type == HEAL_PACK)
            {
                return;
            }

            if (scoreItemObj.GetComponent<ScoreItem>() == null)
            {
                return;
            }
            scoreItem = scoreItemObj.GetComponent<ScoreItem>();


            if (scoreItem.itemIndex != num)
            {
                return;
            }

            if (action == 0)
            {
                scoreItemObj.SetActive(true);
            }
            else
            {
                scoreItemObj.SetActive(false);
            }
        }

        public void OnGameEventKill(int from, int to, string option) { }

        public void OnGameEventObject(int num, int health) { }

        public void OnGameEventRespawn(int num, int time) { }
    }

    void OnPlayerInteract()
    {
        if (this.gameObject.activeSelf == true)
        {
            Debug.Log("ItemIndex : " + itemIndex);
            NetworkModule.GetInstance().RequestGameUserActionItem(MSB_GameManager.Instance.roomIndex, SCORE_ITEM, itemIndex);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreItemListener = new ScoreItemListener();
        scoreItemListener.scoreItemObj = this.gameObject;
        NetworkModule.GetInstance().AddOnEventGameEvent(scoreItemListener);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MSB_Character colliderMSBCharacter = collision.GetComponent<MSB_Character>();
        if (colliderMSBCharacter == null)
        {
            return;
        }

        if (!colliderMSBCharacter.isLocalUser)
        {
            return;
        }

        OnPlayerInteract();
    }
}
