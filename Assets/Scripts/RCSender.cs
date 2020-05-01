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
        _angleZ = _characterModel.rotation.eulerAngles.z;
        _angleVelocity = _characterSpin.spinSpeed * _characterSpin.speedMultiplier;

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
        #if RCSENDER_LOG_ON
        Debug.Log(data);
        #endif
        NetworkModule.GetInstance().RequestGameUserMove(roomNum, data);
        _stringBuilder.Clear();
    }

    IEnumerator RequestUserMove()
    {
        while (true)
        {
            SendMoveData(_room);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private float SpeedThreshold = 400;
    IEnumerator ImmediateRequestUserMove()
    {
        while (_controller.Speed.sqrMagnitude > SpeedThreshold)
        {
            SendMoveData(_room);
            yield return null;
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
                StartCoroutine(RequestUserMove());
                break;

            case "GameOver":
                gameOn = false;
                StopCoroutine(RequestUserMove());
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
