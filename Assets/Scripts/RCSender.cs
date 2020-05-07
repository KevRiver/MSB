//#define RCSENDER_LOG_ON
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MSBNetwork;



/// <summary>
/// 로컬 플레이어와 연결되어 동기화 정보를 보내는 오브젝트
/// </summary>
public class RCSender : Singleton<RCSender>, MMEventListener<MMGameEvent>
{
    private int _room;
    private bool gameOn;

    public MSB_Character character;
    private CharacterSpin _characterSpin;
    private Rigidbody2D _rb;
    private CorgiController _controller;
    private Transform _characterModel;
    private Transform _weaponAttachment;
    private Weapon _weapon;

    private StringBuilder _stringBuilder;
    private StringBuilder _stringBuilder1;
    private int _userNum;
    private Vector3 _pos;
    private float _posX;
    private float _posY;
    private float _posZ;
    private float _speedX;
    private float _speedY;
    private bool _grounded;
    private bool _isFacingRight;
    private float _angleZ;
    private float _angleVelocity;
    private float _sendTime;
    
    private Quaternion _rot;
    private float _rotX;
    private float _rotY;
    private float _rotZ;
    private float _rotW;
    protected override void Awake()
    {
        base.Awake();
        gameObject.name = "RCSender";
        _stringBuilder = new StringBuilder();
        _stringBuilder1 = new StringBuilder();
    }

    public void Initialize(MSB_Character sender, int room)
    {
        _room = room;

        character = sender;
        _rb = character.GetComponent<Rigidbody2D>();
        _controller = character.gameObject.GetComponent<CorgiController>();
        _characterSpin = character.gameObject.GetComponent<CharacterSpin>();
        
        if (_controller == null)
        {
            #if RCSENDER_LOG_ON
            Debug.Log("RCSender corgicontroller is null");
            #endif
            
            return;
        }

        _userNum = this.character.UserNum;

        _characterModel = character.transform.GetChild(0);
        _weaponAttachment = _characterModel.GetChild(0);
        _weapon = _weaponAttachment.transform.GetComponentInChildren<Weapon>();
        #if RCSENDER_LOG_ON
        Debug.Log("RCSender Initialized");
        #endif
    }
    
    public void StartRequest()
    {
        StartCoroutine(RequestUserMove());
    }

    public void StopRequest()
    {
        StopCoroutine(RequestUserMove());
    }
    
    private void SendMoveData(int roomNum)
    {
        _pos = character.transform.position;
        _posX = _pos.x;
        _posY = _pos.y;
        _posZ = _pos.z;
        _speedX = _controller.Speed.x;
        _speedY = _controller.Speed.y;
        _isFacingRight = character.IsFacingRight;

        _stringBuilder.Append(_userNum);
        _stringBuilder.Append(",");
        _stringBuilder.Append(_posX);
        _stringBuilder.Append(",");
        _stringBuilder.Append(_posY);
        _stringBuilder.Append(",");
        _stringBuilder.Append(_posZ);
        _stringBuilder.Append(",");
        _stringBuilder.Append(_speedX);
        _stringBuilder.Append(",");
        _stringBuilder.Append(_speedY);
        _stringBuilder.Append(",");
        _stringBuilder.Append(_isFacingRight);

        string data = _stringBuilder.ToString();
        #if RCSENDER_LOG_ON && UNITY_EDITOR
        Debug.LogFormat("SendPositionData:: {0} {1}, {2}, {3}, {4}", ++(GlobalData.logCnt), _userNum, _posX, _posY,
            _speedX, _speedY);
        #endif
        NetworkModule.GetInstance().RequestGameUserMove(roomNum, data);
        _stringBuilder.Clear();
    }

    public float ImmediatePositionDataSendThreshold = 256f; // 15^2
    IEnumerator RequestUserMove()
    {
        int frameForSendCnt = 3;
        CorgiControllerState controllerState = _controller.State;
        // if character is active in hierachy, send message
        while (true)
        { 
            float sendDelay = 0f;
            if (!character.gameObject.activeInHierarchy)
            {
                // if character gameobject is not active in hierachy don't send data
                yield return null;
            }
            else
            {
                bool controllerColliding = (controllerState.IsCollidingBelow || controllerState.IsCollidingAbove ||
                                            controllerState.IsCollidingLeft || controllerState.IsCollidingRight);
                SendMoveData(_room);
                if (controllerColliding)
                {
#if RCSENDER_LOG_ON && UNITY_EDITOR
                    Debug.Log("RCSender::colliding");
#endif
                    // if controller colliding with object, immediately send next two frames' position data
                    sendDelay = 0f;
                    for (int i = frameForSendCnt; i > 0 && character.gameObject.activeInHierarchy; i--)
                    {
                        SendMoveData(_room);
                        yield return null;
                    }
                }
                sendDelay = _controller.Speed.sqrMagnitude > ImmediatePositionDataSendThreshold ? 0.033f : 0.1f;
                yield return new WaitForSeconds(sendDelay);
            }
        }
    }

    public void RequestUserSync()
    {
        _rot = _characterModel.transform.rotation;
        _stringBuilder1.Append(_userNum);
        _stringBuilder1.Append(",");
        _stringBuilder1.Append(_rot.x);
        _stringBuilder1.Append(",");
        _stringBuilder1.Append(_rot.y);
        _stringBuilder1.Append(",");
        _stringBuilder1.Append(_rot.z);
        _stringBuilder1.Append(",");
        _stringBuilder1.Append(_rot.w);
        var data = _stringBuilder1.ToString();
        NetworkModule.GetInstance().RequestGameUserSync(_room, data);
        _stringBuilder1.Clear();
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        //Debug.Log(gameEvent.EventName + " Called");
        switch (gameEvent.EventName)
        {
            case "GameStart":
                gameOn = true;
                StartRequest();
                break;

            case "GameOver":
                gameOn = false;
                StopRequest();
                break;
        }
    }
    
    public void RequestDamage(int target, int damage, string options)
    {
        NetworkModule.GetInstance().RequestGameUserActionDamage(_room, target, damage, options);
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }
}
