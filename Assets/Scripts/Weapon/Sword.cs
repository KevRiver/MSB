using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    GameManager gameManager;
    GameObject weaponObj;
    Transform parentTransform;
    //public GameObject weaponPrefab;

    public Vector3 attachPosition;
    public Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, 20));

    //public Animation attackAnim;
    //public Animation skillAnim;
    public float attackDmg;
    public float skillDmg;

    BoxCollider2D weaponCollider;

    public void PlayAttackAnim()
    {
        Debug.Log("PlayAttackAnim called");
        gameObject.GetComponent<Animation>().Play();
    }

    public void PlaySkillAnim()
    {
        //skillAnim.Play();
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private void Start()
    {
        //weaponCollider = gameObject.GetComponent<BoxCollider2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        foreach (GameObject player in gameManager.players)
        {
            if (player.GetComponent("Player") != null)
            {
                parentTransform = player.transform;
            }
        }
        //gameObject.transform.parent = parentTransform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent("BasePlayer") != null)
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
                //playerObject.GetComponent<Player>().sendUserHit(targetUserIndex, hitDirection, Player.ACTION_TYPE.TYPE_ATTACK);
            }
        }
    }
}
