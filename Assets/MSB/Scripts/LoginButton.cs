using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSBNetwork;

public class LoginButton: MonoBehaviour
{
    NetworkModule networkManager;

    public InputField idInput;
    public InputField pwInput;

    private string id;
    private string pw;

    public GameObject currentUIObj;
    public GameObject nextUIObj;   

    // Start is called before the first frame update
    void Start()
    {        
    }

    public void OnLoginButtonClicked()
    {
        id = idInput.text;
        pw = pwInput.text;

        NetworkModule.GetInstance().RequestUserLogin(id, pw);

        if (nextUIObj != null)
            StartCoroutine(NextUIObjOn());
        else
            Debug.Log("Next Panel 에 게임 오브젝트를 등록하세요");
    }

    public void CurrentUIObjOff()
    {
        currentUIObj.transform.GetChild(0).gameObject.SetActive(false);
    }

    public IEnumerator NextUIObjOn()
    {
        yield return new WaitForSeconds(1.0f);
        nextUIObj.transform.GetChild(0).gameObject.SetActive(true);        
        CurrentUIObjOff();
    }
}
