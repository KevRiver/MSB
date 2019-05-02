using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject hitmarkerSound;
    public GameObject hitmarker;
<<<<<<< HEAD
    Vector2 hitDirection;

    private void Start()
    {
        hitmarkerSound = GameObject.Find("hitmarkerSound");
=======
    private void Start()
    {
        hitmarkerSound = GameObject.Find("hitmarkerSound");
>>>>>>> origin/develop_Test
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
<<<<<<< HEAD
        // 공격한 상대가 플레이어면 
=======
        // 공격한 상대가 플레이어면
>>>>>>> origin/develop_Test
        if (other.gameObject.GetComponent("BasePlayer") != null)
        {
            hitmarkerSound.GetComponent<AudioSource>().Play();
            /* 히트마커 임시 삭제
<<<<<<< HEAD
               Vector3 playerPos = this.gameObject.transform.position;
               Instantiate(hitmarker, playerPos, Quaternion.identity);
               */
            // 공격한 사람이 자기자신 플레이어면
            GameObject playerObject = gameObject.transform.parent.gameObject;

            if (playerObject.GetComponent("Player") != null)
            {
                Debug.Log("Hit!");
                Vector2 hitDirection = new Vector2(0, 0);
                Vector2 contactPoint = new Vector2(0, 0);
                contactPoint = gameObject.GetComponent<BoxCollider2D>().bounds.ClosestPoint(other.transform.position);
                hitDirection.x = other.transform.position.x - contactPoint.x;
                hitDirection.y = other.transform.position.y - contactPoint.y;
                Debug.Log("HIT SENT TARGETX : " + other.transform.position.x + " CONTACTX : " + contactPoint.x);
                Debug.Log("HIT SENT TARGETY : " + other.transform.position.y + " CONTACTY : " + contactPoint.y);
                int targetUserIndex = other.gameObject.GetComponent<PlayerDetail>().Controller.Num;
                //playerObject.GetComponent<Player>().sendUserHit(targetUserIndex, hitDirection, Player.ACTION_TYPE.TYPE_ATTACK); 190429 주석처리함
            }

=======
            Vector3 playerPos = this.gameObject.transform.position;
            Instantiate(hitmarker, playerPos, Quaternion.identity);
            */          
            // 공격한 사람이 자기자신 플레이어면
            if (gameObject.GetComponent("Player") != null)
            {
                int targetUserIndex = other.gameObject.GetComponent<PlayerDetail>().Controller.Num;
                gameObject.GetComponent<Player>().sendUserHit(targetUserIndex, Player.ACTION_TYPE.TYPE_ATTACK);
            }

>>>>>>> origin/develop_Test
        }
    }
}
