using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MSBNetwork;

public class HealPack : MonoBehaviour
{
    const int SCORE_ITEM = 0;
    const int HEAL_PACK = 1;

    public int healPackIndex;
    HealEventListener healListener;

    class HealEventListener : NetworkModule.OnGameEventListener
    {
        public GameObject healObject;
        public HealPack healPack;
        public void OnGameEventDamage(int from, int to, int amount, string option) { }

        public void OnGameEventHealth(int num, int health) { }

        public void OnGameEventItem(int type,int num, int action)
        {
            //Debug.LogWarning("***OnGameEventItem***");
            if (type == SCORE_ITEM)
            {
                return;
            }
            if (healObject.GetComponent<HealPack>() == null)
            {
                return;
            }
            healPack = healObject.GetComponent<HealPack>();

            if (healPack.healPackIndex != num)
            {
                return;
            }

            if (action == 0)
            {
                healObject.SetActive(true);
            }
            else
            {
                // action이 user number인 플레이어에게 먹은 효과(애니메이션) 추가할 것
                healObject.SetActive(false);
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
            NetworkModule.GetInstance().RequestGameUserActionItem(MSB_GameManager.Instance.roomIndex, HEAL_PACK, healPackIndex);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healListener = new HealEventListener();
        healListener.healObject = this.gameObject;
        NetworkModule.GetInstance().AddOnEventGameEvent(healListener);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MSB_Character>() != null && collision.gameObject.GetComponent<MSB_Character>().isLocalUser == true)
            OnPlayerInteract();
    }

    void OnDestroy()
    {
        // 나중에
    }
}