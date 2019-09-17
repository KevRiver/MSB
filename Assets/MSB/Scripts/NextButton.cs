using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

public class NextButton : MonoBehaviour
{
    public InputField portInputField;
    public PortData portData;
    string port;

    public void GoLobbyScene()
    {
        port = portInputField.text;
        portData.SetPort(int.Parse(port));
        Debug.Log("Port : " + port);
        SceneManager.LoadScene("TempLobbyScene");
    }
}
