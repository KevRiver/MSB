using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a GameObject to have it play a background music when instanciated.
	/// Careful : only one background music will be played at a time.
	/// </summary>
	public class BackgroundMusic : PersistentHumbleSingleton<BackgroundMusic>
	{
		/// the background music
		public AudioClip SoundClip ;

	    protected AudioSource _source;

	    /// <summary>
	    /// Gets the AudioSource associated to that GameObject, and asks the GameManager to play it.
	    /// </summary>
	    protected virtual void Start () 
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
}