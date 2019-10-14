using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class PlayerInfo
{
    public int number;
    public string id;
    public string nick;
    public int weapon;
    public int skin;

    public PlayerInfo(int _number, string _id, string _nick, int _weapon, int _skin)
    {
        number = _number;
        id = _id;
        nick = _nick;
        weapon = _weapon;
        skin = _skin;
    }
}

public class GameInfo : PersistentSingleton<GameInfo>
{
    public int room;
    public List<PlayerInfo> players;

    protected override void Awake()
    {
        Debug.Log("GameInfo Awake");
        base.Awake();

        Debug.Log("PlayerInfo List Initialize");
        players = new List<PlayerInfo>();

        Debug.Log("Put Data at List");
        PlayerInfo Qon = new PlayerInfo(0, "Qon", "Qon", 0, 0);
        players.Add(Qon);
        PlayerInfo Lime = new PlayerInfo(1, "Lime", "Lime", 1, 0);
        players.Add(Lime);
    }
}
