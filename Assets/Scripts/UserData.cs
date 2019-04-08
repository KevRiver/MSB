using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    ArrayList m_userlist;
    int gameRoomIndex;
    int gamePlayerIndex;

    private void Awake()
    {
        m_userlist = new ArrayList();
        Debug.Log("Arraylist m_userlist 가 만들어졌습니다");
        DontDestroyOnLoad(gameObject);
    }

    public void addUser(User user)
    {
        m_userlist.Add(user);
        Debug.Log(user.Id + "add to UserData");
    }

    public ArrayList getUserlist()
    {
        return m_userlist;
    }

    public void setRoomIndex(int gameRoomIndex)
    {
        this.gameRoomIndex = gameRoomIndex;
    }

    public void setPlayerIndex(int playerIndex)
    {
        this.gamePlayerIndex = playerIndex;
    }

    public int getRoomIndex()
    {
        return gameRoomIndex;
    }

    public int getPlayerIndex()
    {
        return gamePlayerIndex;
    }

    public void clearUserData()
    {
        m_userlist.Clear();
    }
}
