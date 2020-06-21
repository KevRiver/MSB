#define LOADING_VIEW_DEBUG

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
            BluePlayerNicks[playerIndex].text = nick;
            
            Sprite sprite = PortraitsCache[player.weapon];
            Image image = BluePlayerPortraits[playerIndex];
            RectTransform rt = BluePlayerPortraits[playerIndex].rectTransform;
            ResizePlayerPortrait(sprite, ref image, ref rt);
            
            SpriteOutline spriteOutline = BluePlayerPortraits[playerIndex].GetComponent<SpriteOutline>();
            if(!spriteOutline)Debug.LogWarning("Can't find SpriteOutline component in the gameobject");
            else
            {
                spriteOutline.Regenerate();
            }
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
            RedPlayerNicks[playerIndex].text = nick;
            
            Sprite sprite = PortraitsCache[player.weapon];
            Image image = RedPlayerPortraits[playerIndex];
            RectTransform rt = RedPlayerPortraits[playerIndex].rectTransform;
            ResizePlayerPortrait(sprite, ref image, ref rt);

            SpriteOutline spriteOutline = RedPlayerPortraits[playerIndex].GetComponent<SpriteOutline>();
            if(!spriteOutline)Debug.LogWarning("Can't find SpriteOutline component in the gameobject");
            else
            {
                spriteOutline.Regenerate();
            }
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
    /// Resizing Portrait
    /// </summary>
    /// <param name="sprite"> native sprite </param>
    /// <param name="portrait">portrait image </param>
    /// <param name="rt"> portrait rectTransform </param>
    private void ResizePlayerPortrait(Sprite sprite,ref Image portrait, ref RectTransform rt)
    {
        Vector2 origin = rt.sizeDelta;
#if LOADING_VIEW_DEBUG
        Debug.LogWarning("ResizePlayerPortrait::origin : " + origin.x + " " + "origin.y");
#endif
        portrait.sprite = sprite;
            
        portrait.SetNativeSize();
        Vector2 native = rt.sizeDelta;
#if LOADING_VIEW_DEBUG
        Debug.LogWarning("ResizePlayerPortrait::native : " + native.x + " " + "native.y");
#endif
        // rescale by height
        float nx = origin.x;
        float ny = native.y * origin.x / native.x;
        if (native.x > native.y)
        {
            nx = native.x * origin.y / native.y;
            ny = origin.y;
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
