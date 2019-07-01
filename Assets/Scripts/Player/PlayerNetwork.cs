using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;



public class PlayerNetwork : MonoBehaviour
{
    private GameManager gameManager;
    public int playerID;
    public int gameRoomIndex;

    public enum ACTION_TYPE
    {
        TYPE_ATTACK, TYPE_SKILL, TYPE_HIT
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        StartCoroutine("syncUserMove");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator syncUserMove()
    {
        while (true)
        {
            sendUserMove();
            yield return new WaitForSeconds(0.016f);
        }
    }

    public void sendUserMove()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("positionX", this.gameObject.transform.position.x);
        jsonData.AddField("positionY", this.gameObject.transform.position.y);
        jsonData.AddField("positionZ", this.gameObject.transform.position.z);
        jsonData.AddField("toward", this.gameObject.transform.localScale.x);
        jsonData.AddField("forceX", this.gameObject.GetComponent<Rigidbody2D>().velocity.x);
        jsonData.AddField("forceY", this.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        gameManager.sendUserMove(jsonData);
    }

	public void sendUserAttack(Vector3 aimVector)
	{
		JSONObject jsonData = new JSONObject();
		jsonData.AddField("action", ACTION_TYPE.TYPE_ATTACK.ToString());
		jsonData.AddField("aimVectorX", aimVector.x);    //애니메이션 실행되는 방향
		jsonData.AddField("aimVectorY", aimVector.y);
		jsonData.AddField("aimVectorZ", aimVector.z);
		gameManager.sendUserAction(jsonData);
		Debug.Log("userGameAction SENT");
	}

	public void sendUserSkill()
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("action", ACTION_TYPE.TYPE_SKILL.ToString());
        //jsonData.AddField("mel") 스킬 애니메이션에 필요한 것들을 가져와야댐
        gameManager.sendUserAction(jsonData);
        Debug.Log("userGameAction SENT");
    }

    public void sendUserHit(int targetUserIndex, float dmg, Vector2 hitDirection, ACTION_TYPE actionType)
    {
        JSONObject jsonData = new JSONObject();
        jsonData.AddField("target", targetUserIndex);
        jsonData.AddField("type", actionType.ToString());
        jsonData.AddField("hitDirectionX", hitDirection.x);
        jsonData.AddField("hitDirectionY", hitDirection.y);
        jsonData.AddField("Damage", dmg);
        //jsonData.AddField("damage", dmg);   //적용되는 데미지
        //jsonData.AddField("mel") 어떻게 누구를 때렸는지, CC기가 적용되는지 안되는지
        // 상대방이 어떻게 제어되는지까지 각 무기마다 다 다르기 때문에 Weapon을 가져오고
        gameManager.sendUserHit(jsonData);
        Debug.Log("userGameHit SENT");
    }
}
