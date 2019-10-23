using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class PlayerInfo
{
    public int room;
    public int number;
    public string id;
    public string nick;
    public int weapon;
    public int skin;

    public PlayerInfo(int _room, int _number, string _id, string _nick, int _weapon, int _skin)
    {
        room = _room;
        number = _number;
        id = _id;
        nick = _nick;
        weapon = _weapon;
        skin = _skin;
    }
}
/// <summary>
/// 게임방 번호, 매칭된 유저 정보, 맵 정보를 저장하는 객체
/// </summary>
public class GameInfo : PersistentSingleton<GameInfo>
{
    // 게임에 대한 정보를 갖고 있다
    // PlayScene으로 로드하기전에 네트워크 매니저에 의해 생성된다
    // 
    public int room = 0;
    public List<PlayerInfo> players;
    private LocalUser localUser;

    protected override void Awake()
    {
        //Debug.Log("GameInfo Awake");
        base.Awake();
        gameObject.name = "GameInfo";

        localUser = LocalUser.Instance;
        players = new List<PlayerInfo>();
    }
}
