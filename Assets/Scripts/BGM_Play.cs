using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
public class BGM_Play : MonoBehaviour
{
    public AudioClip SoundClip;

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
			
        SoundManager.Instance.PlayBackgroundMusic(_source);
    }
}
