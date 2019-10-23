using System.Collections;
using System.Collections.Generic;
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

    public MSB_Character _sender;
    private Rigidbody2D _rb;
    private CorgiController _controller;
    private Weapon _weapon;

    private string posX;
    private string posY;
    private string posZ;
    private string speedX;
    private string speedY;
    private string isFacingRight;

    protected override void Awake()
    {
        base.Awake();
        gameObject.name = "RCSender";
    }

    public void Initialize(MSB_Character sender)
    {
        _room = MSB_GameManager.Instance.RoomNum;

        _sender = sender;
        _rb = _sender.GetComponent<Rigidbody2D>();
        _controller = _sender.gameObject.GetComponent<CorgiController>();


        Transform weaponAttachment = _sender.transform.GetChild(0);
        _weapon = weaponAttachment.transform.GetComponentInChildren<Weapon>();
        Debug.Log("RCSender Initialized");
    }

    IEnumerator RequestUserMove()
    {
        while (true)
        {
            posX = (_sender.transform.position.x).ToString();
            posY = (_sender.transform.position.y).ToString();
            posZ = (_sender.transform.position.z).ToString();
            speedX = (_rb.velocity.x).ToString();
            speedY = (_rb.velocity.y).ToString();
            isFacingRight = _sender.IsFacingRight.ToString();

            string data = _sender.UserNum.ToString() + "," + posX + "," + posY + "," + posZ + "," + speedX + "," + speedY + "," + isFacingRight;
            NetworkModule.GetInstance().RequestGameUserMove(_room, data);
            yield return new WaitForSeconds(0.1f);
        }
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

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        StartCoroutine(RequestUserMove());
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        StopCoroutine(RequestUserMove());
    }
}
