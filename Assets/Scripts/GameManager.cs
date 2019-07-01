using System.Collections;
using Cinemachine.Utility;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class GameManager : MonoBehaviour
{
    int gameRoomIndex;
    int gamePlayerIndex;
    ArrayList userlist;
    
    Controller controller;
    
    public GameObject PlayerPrefab;
    public ArrayList players = new ArrayList();

    public Hashtable mapHashtable = new Hashtable();

    private SocketIOComponent socketIO;
    private Cinemachine.CinemachineVirtualCamera cmVCam;

    public GameObject basicAtkRange;
    public GameObject skillRange;

    private void Awake()
    {
        //Spawning users
        UserData userData = GameObject.Find("UserData").GetComponent<UserData>();
        userlist = userData.getUserlist();
        
        gameRoomIndex = userData.getRoomIndex();
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        //gamePlayerIndex = userData.getPlayerIndex();

        cmVCam = GameObject.Find("CM_VCam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        

        GameObject networkmodule = GameObject.Find("NetworkModule");
        socketIO = networkmodule.GetComponent<NetworkModule>().get_socket();
    }

    void Start()
    {
        //Initialize Spawning Points

        
        //Debug.Log("AAAAA"+userlist);
        //Debug.Log(userlist[0]);
        //Debug.Log(userlist[1]);
        //int i = 0;
        foreach(User userData in userlist)
        {
            Debug.Log("Users");
            GameObject player = new GameObject();
            Debug.Log(userData.Num);
            player = Instantiate(PlayerPrefab, new Vector3(1, -2, -3), Quaternion.identity);
            player.GetComponent<PlayerDetail>().Controller = userData;
            //player.AddComponent<BasePlayer>();
            players.Add(player);
            AttachWeapon(player, 1);

            if (userData.Num == GameObject.Find("LocalPlayer").GetComponent<LocalPlayer>().getLocalPlayer().Num) //localPlayer의 index와 생성한 Player의 인덱스가 같으면
            {
                Debug.Log("localPlayer");
                controller.SetTargetObj(player);
                player.tag = "LocalPlayer";
                //Debug.Log(player.GetComponent<PlayerDetail>().Controller.Num);
                gamePlayerIndex = player.GetComponent<PlayerDetail>().Controller.Num;
                player.AddComponent<PlayerNetwork>();
                //player.AddComponent<Player>();
                
                Debug.Log(GameObject.Find("CM_VCam").transform.localPosition);
                cmVCam.Follow = player.transform;
                Debug.Log(GameObject.Find("CM_VCam").transform.localPosition);
                //cmVCam.LookAt = player.transform;
                GameObject.Find("CM_VCam").transform.localPosition = player.transform.localPosition;
                //GameObject.Find("Main Camera").GetComponent<followCamera>().setTarget(player.transform);
            } else
			{
			//	player.GetComponent<BasePlayer>().Rb.gravityScale = 0;
			}
        }

        socketIO.On("userGameMove", OnUserMove);
        socketIO.On("userGameAction", OnUserAction);
        socketIO.On("userBlockDestroy", OnBlockDestroy);
        socketIO.On("userGameHit", OnUserHit);
    }

    void Update()
    {
        
    }

    void OnUserMove(SocketIOEvent e)
    {
        //Debug.Log(e.name + " / " + e.data);
        JSONObject data = e.data;
        int userIndex = (int)data[0].n;
        JSONObject moveData = data[1];
        Vector3 newPosition = new Vector3(0, 0, 0);
        float toward;
        float velocityX, velocityY;
        newPosition.x = moveData[0].n;
        newPosition.y = moveData[1].n;
        newPosition.z = moveData[2].n;
        toward = moveData[3].n;
        velocityX = moveData[4].n;
        velocityY = moveData[5].n;
        
        //GameObject.Find
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerDetail>().Controller.Num == userIndex)
            {
                player.transform.SetPositionAndRotation(newPosition, Quaternion.identity);
                player.transform.localScale = new Vector3(toward, 0.5f);
				player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<PlayerMovement>().currentMoveSpeed = velocityX;
				player.GetComponent<PlayerMovement>().rb.AddForce(new Vector2(velocityX, velocityY));
				//Debug.Log("other player moved");
				break;
                //Debug.Log("other player moved");
            }
        }
    }

	void OnUserAction(SocketIOEvent e)
	{
		Debug.Log(e.name + " / " + e.data);
		JSONObject data = e.data;
		int userIndex = (int)data[0].n;
		JSONObject actionData = data[1];
		string actionTypeString = actionData[0].str;
		PlayerNetwork.ACTION_TYPE actionType = PlayerNetwork.ACTION_TYPE.TYPE_ATTACK;
		if (String.Equals(actionTypeString, PlayerNetwork.ACTION_TYPE.TYPE_ATTACK.ToString()))
		{
			actionType = PlayerNetwork.ACTION_TYPE.TYPE_ATTACK;
			Debug.Log("OnUserAction ATTACK");
		}
		if (String.Equals(actionTypeString, PlayerNetwork.ACTION_TYPE.TYPE_SKILL.ToString()))
		{
			actionType = PlayerNetwork.ACTION_TYPE.TYPE_SKILL;
			Debug.Log("OnUserAction SKILL");
		}

		foreach (GameObject player in players)
		{
			if (player.GetComponent<PlayerDetail>().Controller.Num == userIndex)
			{
				if (actionType == PlayerNetwork.ACTION_TYPE.TYPE_ATTACK)
				{
					player.GetComponent<PlayerAction>().Attack(new Vector3(actionData[1].n, actionData[2].n, actionData[3].n));
					Debug.Log("OnUserAction ATTACK Complete");
					break;
				}
				if (actionType == PlayerNetwork.ACTION_TYPE.TYPE_SKILL)
				{
					Debug.Log("OnUserAction SKILL Complete");
					//player.GetComponent<PlayerAction>().showSkillAnim();
					break;
				}
			}
		}
	}

	void OnBlockDestroy(SocketIOEvent e)
    {
        JSONObject data = e.data;
        int destroyedBlock = (int)data[1].n;

        GameObject targetBlock = (GameObject)mapHashtable[destroyedBlock];
        Debug.Log(mapHashtable[destroyedBlock]);
        Debug.Log(destroyedBlock);
        targetBlock.GetComponent<DestroyBlock>().destroyBlock();
    }

    void OnUserHit(SocketIOEvent e)
    {
        Debug.Log("onUserHit " + e.name + " / " + e.data);
        JSONObject data = e.data[1];
        float hitDirX, hitDirY;
        float dmg;

        if (gamePlayerIndex != (int)data[0].n)
        {
            Debug.Log("Not My Hit playerIndex" + gamePlayerIndex + " targetIndex" + (int)data[0].n);
            return;
        }

        hitDirX = data[2].n;
        hitDirY = data[3].n;
        dmg = data[4].n;

        foreach (GameObject p in players)
        {
            if (p.tag=="LocalPlayer")
            {

                p.GetComponent<PlayerMovement>().rb.AddForce(new Vector2(hitDirX, hitDirY) * 1000);
                p.GetComponent<PlayerHP>().TakeDamage(dmg);
                Debug.Log("OnUserHit Test");
            }
        }

    }

    public void sendUserMove(JSONObject data)
    {
        data.AddField("gameRoomIndex", gameRoomIndex);
        socketIO.Emit("userGameMove", data);
    }

    public void sendUserAction(JSONObject data)
    {
        data.AddField("gameRoomIndex", gameRoomIndex);
        socketIO.Emit("userGameAction", data);
		Debug.Log("sendUserAction finished");
	}

    public void sendBlockDestroy(JSONObject data)
    {
        data.AddField("gameRoomIndex", gameRoomIndex);
        socketIO.Emit("userBlockDestroy", data);
    }

    public void sendUserHit(JSONObject data)
    {
        data.AddField("gameRoomIndex", gameRoomIndex);
        socketIO.Emit("userGameHit", data);
    }

    public void AttachWeapon(GameObject _player, int _index)    //플레이어 오브젝트에 달아줄 무기의 인덱스
    {
        Debug.Log("Attach Weapon called");
        //float posX = 0;
        //float posY = 0;
        switch (_index)
        {
            case 1:
                //WeaponAxis의 자식으로 소드 프리팹을 붙혀준다
                //weapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/Sword"), weaponAxis.transform) as GameObject;
                Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/Sword"), _player.GetComponent<PlayerObjects>().weaponAxis.transform);

                //소드 프리팹의 기본공격 범위와 스킬영역 범위
                basicAtkRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/BasicAtkRange"), _player.GetComponent<PlayerObjects>().aimAxis.transform) as GameObject;
                basicAtkRange.GetComponent<SpriteRenderer>().enabled = false;
                skillRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/SwordPrefab/SkillRange"), _player.GetComponent<PlayerObjects>().aimAxis.transform) as GameObject;
                skillRange.GetComponent<SpriteRenderer>().enabled = false;
                break;
            case 2:
                //활 프리팹을 붙혀준다
                Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/BowPrefab/Bow"), _player.GetComponent<PlayerObjects>().weaponAxis.transform);

                //활 프리팹의 기본공격 범위와 스킬영역 범위
                basicAtkRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/BowPrefab/BasicAtkRange"), _player.GetComponent<PlayerObjects>().aimAxis.transform) as GameObject;
                basicAtkRange.GetComponent<SpriteRenderer>().enabled = false;
                skillRange = Instantiate(Resources.Load<GameObject>("Prefabs/Weapon/BowPrefab/SkillRange"), _player.GetComponent<PlayerObjects>().aimAxis.transform) as GameObject;
                skillRange.GetComponent<SpriteRenderer>().enabled = false;
                break;
            default:
                break;
        }
    }
}
