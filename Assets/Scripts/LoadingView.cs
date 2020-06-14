//#define LOADING_VIEW_DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MSBNetwork;
using Newtonsoft.Json.Linq;

public struct LoadingViewData
{
    public List<PlayerInfo> _players;
    public LoadingViewData(List<PlayerInfo> players)
    {
        _players = players;
    }
}

public class LoadingView : MonoBehaviour,MSB_View<LoadingViewData>
{
    public GameObject BluePanel;

    public GameObject RedPanel;
    public Text Versus;

    public Sprite[] PortraitsCache;
    
    public RectTransform[] BlueFrames;
    public RectTransform[] RedFrames;
    public float FramePadding;
    
    public Image[] BluePlayerPortraits;
    public Image[] RedPlayerPortraits;
    public Text[] BluePlayerNicks;
    public Text[] RedPlayerNicks;

    private Vector3 _blueCenter;
    private Vector3 _redCenter;

    private Animator _animator;
    /// <summary>
    /// 0 FrameIn / 1 FrameOut / 2 ScaleOut/ 3 ScaleIn
    /// </summary>
    public string[] Triggers;

    private void Awake()
    {
        LoadingViewData data = new LoadingViewData(GameInfo.Instance.players);
        Initialize(data);
        PlayIntroAnimation();
    }
    public void Initialize(LoadingViewData data)
    {
        _blueCenter = BlueFrames[1].GetComponent<RectTransform>().localPosition;
        _redCenter = RedFrames[1].GetComponent<RectTransform>().localPosition;
        _animator = GetComponent<Animator>();
        ApplyData(data);
    }

    public void ApplyData(LoadingViewData data)
    {
        //load player data
        
        List<PlayerInfo> players = data._players;
        int playerCnt = players.Count;
        int mid = playerCnt / 2;
        // image width
        float w = BluePlayerPortraits[0].rectTransform.sizeDelta.x;
        // distance between images
        float d = w + 2 * FramePadding;
       
        List<PlayerInfo> bluePlayers = new List<PlayerInfo>();
        for (int i = 0; i < mid; i++)
        {
            bluePlayers.Add(players[i]);
        }
        List<PlayerInfo> redPlayers = new List<PlayerInfo>();
        for (int i = mid; i < playerCnt; i++)
        {
            redPlayers.Add(players[i]);
        }
#if LOADING_VIEW_DEBUG
        foreach (var player in players)
        {
            Debug.LogFormat("{0} {1}", player.number.ToString(), player.nick);
        }
#endif

        int playerIndex = bluePlayers.Count > 1 ? 0 : 1;
        foreach (var player in bluePlayers)
        {
            BlueFrames[playerIndex].gameObject.SetActive(true);
            string nick = player.nick;
            Sprite sprite = PortraitsCache[player.weapon];
            BluePlayerNicks[playerIndex].text = nick;
            RectTransform rt = BluePlayerPortraits[playerIndex].rectTransform;
            Image image = BluePlayerPortraits[playerIndex];
            ResizePlayerPortrait(sprite, ref image, ref rt);
            ++playerIndex;
        }
       
        // Repositioning frames
        int portraits = mid;
        float px = _blueCenter.x + (portraits / 2) * d;
        px = (portraits % 2 == 0) ? px + (d / 2) : px;
        
        for (int p = 0; p < portraits; p++)
        {
            RectTransform rt = BlueFrames[p];
            Vector3 origin = rt.localPosition;
            rt.localPosition = new Vector3(px - p * d, origin.y, origin.z);
        }

        playerIndex = bluePlayers.Count > 1 ? 0 : 1;
        foreach (var player in redPlayers)
        {
            RedFrames[playerIndex].gameObject.SetActive(true);
            string nick = player.nick;
            Sprite sprite = PortraitsCache[player.weapon];
            RedPlayerNicks[playerIndex].text = nick;
            RectTransform rt = RedPlayerPortraits[playerIndex].rectTransform;
            Image image = RedPlayerPortraits[playerIndex];
            ResizePlayerPortrait(sprite, ref image, ref rt);
            ++playerIndex;
        }
        
        px = _redCenter.x - (portraits / 2) * d;
        px = (portraits % 2 == 0) ? px - (d / 2) : px;
        for (int p = 0; p < portraits; p++)
        {
            RectTransform rt = RedFrames[p];
            Vector3 origin = rt.localPosition;
            rt.localPosition = new Vector3(px + p * d, origin.y, origin.z);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sprite"> native sprite </param>
    /// <param name="portrait">portrait image </param>
    /// <param name="rt"> portrait rectTransform </param>
    private void ResizePlayerPortrait(Sprite sprite,ref Image portrait, ref RectTransform rt)
    {
        Vector2 origin = rt.sizeDelta;
        portrait.sprite = sprite;
            
        portrait.SetNativeSize();
        Vector2 native = rt.sizeDelta;
        // rescale by height
        float nx = origin.x * origin.y / native.y;
        float ny = origin.y;
        if (native.x > native.y)
        {
            nx = origin.x;
            ny = origin.y * origin.x / native.x;
        }
        rt.sizeDelta = new Vector2(nx,ny);
    }

    public void PlayIntroAnimation()
    {
        TriggerAnimation(Triggers[0]); //FrameIn
    }

    public void PlayOutroAnimation()
    {
        TriggerAnimation(Triggers[1]); //FrameOut
    }

    public void TriggerAnimation(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
}
