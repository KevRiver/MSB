using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    private int num;
    private string id;
    private string name;
    private int rank;
    private int blocked;
    private int money;
    private int gold;
    private int guild;

    public int Num
    {
        get
        {
            return num;
        }

        set
        {
            num = value;
        }
    }

    public string Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public int Rank
    {
        get
        {
            return rank;
        }

        set
        {
            rank = value;
        }
    }

    public int Blocked
    {
        get
        {
            return blocked;
        }

        set
        {
            blocked = value;
        }
    }

    public int Money
    {
        get
        {
            return money;
        }

        set
        {
            money = value;
        }
    }

    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }

    public int Guild
    {
        get
        {
            return guild;
        }

        set
        {
            guild = value;
        }
    }
}
