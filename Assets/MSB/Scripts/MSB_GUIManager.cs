using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.EventSystems;

public class MSB_GUIManager : MonoBehaviour
{
    public const int RED = 0;
    public const int BLUE = 1;

    private static MSB_GUIManager _instance;
    public static MSB_GUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MSB_GUIManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "GUIManager";
                    _instance = obj.AddComponent<MSB_GUIManager>();
                }
            }
            return _instance;
        }
    }

    public List<ScoreSign> scoreSigns;
    public Timer timer;
    public List<CanvasGroup> controllers;
    public GameObject resultWindow;
    public Text GameStartCounter;
    public Text Result;
}
