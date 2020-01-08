using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientUserData
{
    public int userNumber;
    public string userID;
    public string userNick;
    public int userRank;
    public int userMoney = 0;
    public int userCash = 0;
    public int userWeapon;
    public int userSkin;

    public ClientUserData() { }
    public ClientUserData(int _userNumber, string _userID, string _userNick, int _userWeapon, int _userSkin)
    {
        userNumber = _userNumber;
        userID = _userID;
        userNick = _userNick;
        userWeapon = _userWeapon;
        userSkin = _userSkin;
    }
}
