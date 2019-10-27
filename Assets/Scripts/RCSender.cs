using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MSBNetwork;
using UnityEngine.Serialization;

/// <summary>
/// 로컬 플레이어와 연결되어 동기화 정보를 보내는 오브젝트
/// </summary>
public class RCSender : Singleton<RCSender>, MMEventListener<MMGameEvent>
{
    private int _room;

    public MSB_Character sender;
    private Rigidbody2D _rb;
    private CorgiController _controller;
    private Weapon _weapon;

    private string _userNum;
    private string _posX;
    private string _posY;
    private string _posZ;
    private string _speedX;
    private string _speedY;
    private string _isFacingRight;
    private string _rotZ;
    

    protected override void Awake()
    {
        base.Awake();
        gameObject.name = "RCSender";
    }

    public void Initialize(MSB_Character sender)
    {
        _room = GameInfo.Instance.room;

        this.sender = sender;
        _rb = this.sender.GetComponent<Rigidbody2D>();
        _controller = this.sender.gameObject.GetComponent<CorgiController>();
        if (_controller == null)
        {
            Debug.Log("RCSender corgicontroller is null");
        }
        _userNum = this.sender.UserNum.ToString();

        Transform weaponAttachment = this.sender.transform.GetChild(0);
        _weapon = weaponAttachment.transform.GetComponentInChildren<Weapon>();
        Debug.Log("RCSender Initialized");
    }

    public void StartRequest()
    {
        StartCoroutine(RequestUserMove());
    }

    public void StopRequest()
    {
        StopCoroutine(RequestUserMove());
    }

    IEnumerator RequestUserMove()
    {
        while (true)
        {
            _posX = (sender.transform.position.x).ToString();
            _posY = (sender.transform.position.y).ToString();
            _posZ = (sender.transform.position.z).ToString();
            _speedX = (_controller.Speed.x).ToString();
            _speedY = (_controller.Speed.y).ToString();
            _isFacingRight = sender.IsFacingRight.ToString();
            _rotZ = sender.transform.localRotation.z.ToString();

            string data = _userNum + "," + _posX + "," + _posY + "," + _posZ + "," + _speedX + "," + _speedY + "," + _isFacingRight + ","+ _rotZ;
            NetworkModule.GetInstance().RequestGameUserMove(_room, data);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RequestUserSync()
    {
        _rotZ = transform.rotation.z.ToString();
        var data = _userNum + "," + _rotZ;
        NetworkModule.GetInstance().RequestGameUserSync(_room, data);
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        Debug.Log(gameEvent.EventName + " Called");
        switch (gameEvent.EventName)
        {
            case "GameStart":
                Debug.Log("RCSender begin request");
                StartCoroutine(RequestUserMove());
                break;

            case "GameOver":
                StopCoroutine(RequestUserMove());
                break;
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        //StartCoroutine(RequestUserMove());
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        //StopCoroutine(RequestUserMove());
    }
}
