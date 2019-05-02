using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject hitmarkerSound;
    public GameObject hitmarker;
    private void Start()
    {
        hitmarkerSound = GameObject.Find("hitmarkerSound");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 공격한 상대가 플레이어면
        if (other.gameObject.GetComponent("BasePlayer") != null)
        {
            Debug.Log("Hit !");
            hitmarkerSound.GetComponent<AudioSource>().Play();
            /* 히트마커 임시 삭제
            Vector3 playerPos = this.gameObject.transform.position;
            Instantiate(hitmarker, playerPos, Quaternion.identity);
            */          
            // 공격한 사람이 자기자신 플레이어면
            if (gameObject.GetComponent("Player") != null)
            {
                int targetUserIndex = other.gameObject.GetComponent<PlayerDetail>().Controller.Num;
                gameObject.GetComponent<Player>().sendUserHit(targetUserIndex, Player.ACTION_TYPE.TYPE_ATTACK);
            }

        }
    }
}
