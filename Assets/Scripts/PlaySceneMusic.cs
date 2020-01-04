using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
public class PlaySceneMusic : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public AudioClip SoundClip; // backgroundmusic
    public AudioClip SoundClip1;// event music

    private AudioSource _source;
    // Start is called before the first frame update

    private void OnEnable()
    {
        SoundManager.Instance.MusicOn();
        this.MMEventStartListening();
    }
    
    private void Initialize()
    {
        _source = gameObject.AddComponent<AudioSource>() as AudioSource;	
        _source.playOnAwake=false;
        _source.spatialBlend=0;
        _source.rolloffMode = AudioRolloffMode.Logarithmic;
        _source.loop = false;
        _source.clip = SoundClip;
    }

    private void OnDisable()
    {
        SoundManager.Instance.MusicOff();
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                Initialize();
                SoundManager.Instance.PlayBackgroundMusic(_source);
                // play bgm
                break;
            
            case "HurryUp":
                SoundManager.Instance.PlaySound(SoundClip1, transform.position, true);
                break;
            
            case "GameOver":
                _source.Stop();
                Destroy(GameObject.Find("TempAudio"));
                // stop bgm
                break;
        }
    }
}
