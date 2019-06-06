//
//  QueueButton
//  Created by 문주한 on 04/06/2019.
//
//  큐 입장시 싱글 & 멀티 선택 버튼
//  

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using SocketIO;

public class QueueButton : MonoBehaviour
{
    NetworkModule networkModule;
    SocketIOComponent socket;

    int skinID;
    int weaponID;

    void Start()
    {
        networkModule = GameObject.Find("NetworkModule").GetComponent<NetworkModule>();
    }

    // 케릭터 정보 받아오기
    public void getCharacterInfo(int _skinID, int _weaponID)
    {
        skinID = _skinID;
        weaponID = _weaponID;
    }

    public void sendSkinWeaponID()
    {
        socket = networkModule.get_socket();
        // SkinID and WeaponID 전송
        Debug.Log("Skin ID : " + skinID);
        Debug.Log("Weapon ID : " + weaponID);

        socket.On("soloMatched", soloMatched);

        JSONObject data = new JSONObject(JSONObject.Type.OBJECT);
        data.AddField("skinIndex", skinID);
        data.AddField("weaponIndex", weaponID);
        socket.Emit("matchMakeSolo", data);
    }

    public void soloMatched(SocketIOEvent e)
    {
        Debug.Log("soloMatched : " + e);
        JSONObject data = e.data;

        int gameRoomIndex = -1;
        try
        {
            gameRoomIndex = (int)data[0].n;
        }
        catch (Exception err) { };

        int position = -1;
        try
        {
            position = (int)data[1].n;
        }
        catch (Exception err) { };

        JSONObject userListJSON;
        try
        {
            userListJSON = data[2];

            GameObject.Find("UserData").GetComponent<UserData>().clearUserData();
            int i = 0;
            foreach (JSONObject userData in userListJSON.list)
            {
                //Debug.Log("Queue Handler log" + ++i);
                User player = new User();
                player.Num = (int)userData[0].n;
                player.Id = userData[1].str;
                GameObject.Find("UserData").GetComponent<UserData>().addUser(player);
            }

            SceneManager.LoadScene("GameScene");
        }
        catch (Exception err) { }

        GameObject.Find("UserData").GetComponent<UserData>().setRoomIndex(gameRoomIndex);
        GameObject.Find("UserData").GetComponent<UserData>().setPlayerIndex(position);
    }

    // 솔로 큐 버튼 선택
    public void selectSoloQueue()
    {
        sendSkinWeaponID();
    }

    // 멀티 큐 버튼 선택
    public void selectMultiQueue()
    {
        sendSkinWeaponID();
    }


}
