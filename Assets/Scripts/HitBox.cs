using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public void HitAnimStart()
    {
        gameObject.GetComponent<Animation>().Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.gameObject.GetComponent("BasePlayer") != null)
        {
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
                playerObject.GetComponent<Player>().sendUserHit(targetUserIndex, hitDirection, Player.ACTION_TYPE.TYPE_ATTACK); 0429주석처리함
            }
        }*/
    }
}
