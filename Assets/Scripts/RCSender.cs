using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using MSBNetwork;

/// <summary>
/// 로컬 플레이어와 연결되어 동기화 정보를 보내는 오브젝트
/// </summary>
public class RCSender : MonoBehaviour, MMEventListener<MMGameEvent>
{
    private int _room;

    public MSB_Character _sender;
    private CorgiController _controller;
    private Weapon _weapon;

    private string posX;
    private string posY;
    private string posZ;
    private string speedX;
    private string speedY;
    private string isFacingRight;

    public void Initialize(MSB_Character sender)
    {
        _room = MSB_GameManager.Instance.RoomNum;

        _sender = sender;
        _controller = _sender.gameObject.GetComponent<CorgiController>();

        Transform weaponAttachment = _sender.transform.GetChild(0);
        _weapon = weaponAttachment.transform.GetComponentInChildren<Weapon>();
    }

    IEnumerator RequestUserMove()
    {
        while (true)
        {
            posX = (_sender.transform.position.x).ToString();
            posY = (_sender.transform.position.y).ToString();
            posZ = (_sender.transform.position.z).ToString();
            speedX = (_controller.Speed.x).ToString();
            speedY = (_controller.Speed.y).ToString();
            isFacingRight = _sender.IsFacingRight.ToString();

            string data = _sender.UserNum.ToString() + "," + posX + "," + posY + "," + posZ + "," + speedX + "," + speedY + "," + isFacingRight;
            NetworkModule.GetInstance().RequestGameUserMove(_room, data);
            yield return null;
        }
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                StartCoroutine(RequestUserMove());
                break;

            case "GameOver":
                StopCoroutine(RequestUserMove());
                break;
        }
    }
}
