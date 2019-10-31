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
public class RCSender : Singleton<RCSender>, MMEventListener<MMGameEvent>,MMEventListener<MMDamageTakenEvent>
{
    private int _room;

    public MSB_Character character;
    private Rigidbody2D _rb;
    private CorgiController _controller;
    private Transform _characterModel;
    private Transform _weaponAttachment;
    private Weapon _weapon;

    private int _userNum;
    private Vector3 _pos;
    private string _posX;
    private string _posY;
    private string _posZ;
    private string _speedX;
    private string _speedY;
    private bool _grounded;
    private string _isFacingRight;
    
    private Quaternion _rot;
    private string _rotX;
    private string _rotY;
    private string _rotZ;
    private string _rotW;
    protected override void Awake()
    {
        base.Awake();
        gameObject.name = "RCSender";
    }

    public void Initialize(MSB_Character sender)
    {
        _room = GameInfo.Instance.room;

        character = sender;
        _rb = character.GetComponent<Rigidbody2D>();
        _controller = character.gameObject.GetComponent<CorgiController>();
        if (_controller == null)
        {
            Debug.Log("RCSender corgicontroller is null");
        }

        _userNum = this.character.UserNum;

        _characterModel = character.transform.GetChild(0);
        _weaponAttachment = _characterModel.GetChild(0);
        _weapon = _weaponAttachment.transform.GetComponentInChildren<Weapon>();
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
            _pos = character.transform.position;
            _posX = (_pos.x).ToString();
            _posY = (_pos.y).ToString();
            _posZ = (_pos.z).ToString();
            _speedX = (_controller.Speed.x).ToString();
            _speedY = (_controller.Speed.y).ToString();
            _isFacingRight = character.IsFacingRight.ToString();
            //_rotZ = sender.transform.localRotation.z.ToString();

            string data = _userNum.ToString() + "," + _posX + "," + _posY + "," + _posZ + "," + _speedX + "," + _speedY + "," + _isFacingRight;
            //Debug.LogWarning(data);
            NetworkModule.GetInstance().RequestGameUserMove(_room, data);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RequestUserSync()
    {
        _rot = _characterModel.transform.rotation;
        _rotX = _rot.x.ToString();
        _rotY = _rot.y.ToString();
        _rotZ = _rot.z.ToString();
        _rotW = _rot.w.ToString();
        var data = _userNum.ToString() + "," + _rotX + "," + _rotY + "," + _rotZ + "," + _rotW;
        NetworkModule.GetInstance().RequestGameUserSync(_room, data);
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        Debug.Log(gameEvent.EventName + " Called");
        switch (gameEvent.EventName)
        {
            case "GameStart":
                StartCoroutine(RequestUserMove());
                break;

            case "GameOver":
                StopCoroutine(RequestUserMove());
                break;
        }
    }

    private int _target;
    private int? _instigator;
    private int _causedDamage;
    private MSB_Character _msbInstigator;
    public void OnMMEvent(MMDamageTakenEvent eventType)
    {
        /*Debug.LogWarning("MMDamageTakenEvent occured");
        _instigator = 0;
        _msbInstigator = eventType.Instigator.GetComponent<MSB_Character>();
        _instigator = _msbInstigator?.UserNum;
        if (_instigator != _userNum)
        {
            Debug.LogWarning("Instigator is not local user");
            return;
        }
        _target = ((MSB_Character) (eventType.AffectedCharacter)).UserNum;
        Debug.LogWarning("target is " + _target);
        _causedDamage = (int)(eventType.DamageCaused);
        NetworkModule.GetInstance().RequestGameUserActionDamage(_room, _target, _causedDamage, "");*/
    }

    public void RequestDamage(int target, int damage, string options)
    {
        NetworkModule.GetInstance().RequestGameUserActionDamage(_room, target, damage, options);
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        this.MMEventStartListening<MMDamageTakenEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        this.MMEventStopListening<MMDamageTakenEvent>();
    }
}
