using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    ArrayList m_userlist;

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

    public void clearUserData()
    {
        m_userlist.Clear();
    }
}
