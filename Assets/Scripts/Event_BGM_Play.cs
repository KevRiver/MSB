using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
public class Event_BGM_Play : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public AudioClip SoundClip; // backgroundmusic
    public AudioClip SoundClip1;// event music

    private AudioSource _source;
    // Start is called before the first frame update
    void Start()
    {
        _source = gameObject.AddComponent<AudioSource>() as AudioSource;	
        _source.playOnAwake=false;
        _source.spatialBlend=0;
        _source.rolloffMode = AudioRolloffMode.Logarithmic;
        _source.loop=true;	
		
        _source.clip=SoundClip;
			
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        switch (eventType.EventName)
        {
            case "GameStart":
                SoundManager.Instance.PlayBackgroundMusic(_source);
                // play bgm
                break;
            case "GameOver":
                SoundManager.Instance.MusicOff();
                // stop bgm
                break;
        }
    }
}
