using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Audio;

namespace MoreMountains.CorgiEngine
{	
	[Serializable]
	public class SoundSettings
	{
		public bool MusicOn = true;
		public bool SfxOn = true;
	}

	/// <summary>
	/// This persistent singleton handles sound playing
	/// </summary>
	[AddComponentMenu("Corgi Engine/Managers/Sound Manager")]
	public class SoundManager : PersistentSingleton<SoundManager>,  MMEventListener<CorgiEngineEvent>, MMEventListener<MMGameEvent>
	{	
		[Header("Settings")]
		public SoundSettings Settings;

		[Header("Music")]
		/// the music volume
		[Range(0,1)]
		public float MusicVolume=0.3f;

		[Header("Sound Effects")]
		/// the sound fx volume
		[Range(0,1)]
		public float SfxVolume=1f;

		[Header("Pause")]
		public bool MuteSfxOnPause = true;

		protected const string _saveFolderName = "CorgiEngine/";
		protected const string _saveFileName = "sound.settings";
	    protected AudioSource _backgroundMusic;	
		protected List<AudioSource> _loopingSounds;
			
		/// <summary>
		/// Plays a background music.
		/// Only one background music can be active at a time.
		/// </summary>
		/// <param name="Clip">Your audio clip.</param>
		public virtual void PlayBackgroundMusic(AudioSource Music)
		{
			// if the music's been turned off, we do nothing and exit
			if (!Settings.MusicOn)
				return;
			// if we already had a background music playing, we stop it
			if (_backgroundMusic!=null)
				_backgroundMusic.Stop();
			// we set the background music clip
			_backgroundMusic=Music;
			// we set the music's volume
			_backgroundMusic.volume=MusicVolume;
			// we set the loop setting to true, the music will loop forever
			_backgroundMusic.loop=true;
			// we start playing the background music
			_backgroundMusic.Play();		
		}

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <returns>An audiosource</returns>
        /// <param name="sfx">The sound clip you want to play.</param>
        /// <param name="location">The location of the sound.</param>
        /// <param name="loop">If set to true, the sound will loop.</param>
        public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, bool loop = false)
        {
            if (!Settings.SfxOn)
                return null;
            // we create a temporary game object to host our audio source
            GameObject temporaryAudioHost = new GameObject("TempAudio");
            // we set the temp audio's position
            temporaryAudioHost.transform.position = location;
            // we add an audio source to that host
            AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;
            // we set the audio source volume to the one in parameters
            audioSource.volume = SfxVolume;
            // we set our loop setting
            audioSource.loop = loop;
            // we start playing the sound
            audioSource.Play();

            if (!loop)
            {
                // we destroy the host after the clip has played
                Destroy(temporaryAudioHost, sfx.length);
            }
            else
            {
                _loopingSounds.Add(audioSource);
            }

            // we return the audiosource reference
            return audioSource;
        }

        /// <summary>
        /// Advanced PlaySound method
        /// </summary>
        /// <param name="sfx"></param>
        /// <param name="location"></param>
        /// <param name="pitch"></param>
        /// <param name="pan"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="volumeMultiplier"></param>
        /// <param name="loop"></param>
        /// <param name="reuseSource"></param>
        /// <param name="audioGroup"></param>
        /// <param name="soundFadeInDuration"></param>
        /// <returns></returns>
        public virtual AudioSource PlaySound(AudioClip sfx, Vector3 location, float pitch, float pan, float spatialBlend = 0.0f, float volumeMultiplier = 1.0f, bool loop = false,
            AudioSource reuseSource = null, AudioMixerGroup audioGroup = null)
        {
            if (!Settings.SfxOn || !sfx)
            {
                return null;
            }

            var audioSource = reuseSource;
            GameObject temporaryAudioHost = null;

            if (audioSource == null)
            {
                // we create a temporary game object to host our audio source
                temporaryAudioHost = new GameObject("TempAudio");
                // we add an audio source to that host
                var newAudioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
                audioSource = newAudioSource;
            }
            // we set the temp audio's position
            audioSource.transform.position = location;

            audioSource.time = 0.0f; // Reset time in case it's a reusable one.

            // we set that audio source clip to the one in paramaters
            audioSource.clip = sfx;

            audioSource.pitch = pitch;
            audioSource.spatialBlend = spatialBlend;
            audioSource.panStereo = pan;

            // we set the audio source volume to the one in parameters
            audioSource.volume = SfxVolume * volumeMultiplier;
            // we set our loop setting
            audioSource.loop = loop;
            // Assign an audio mixer group.
            if (audioGroup)
                audioSource.outputAudioMixerGroup = audioGroup;


            // we start playing the sound
            audioSource.Play();

            if (!loop && !reuseSource)
            {
                // we destroy the host after the clip has played (if it not tag for reusability.
                Destroy(temporaryAudioHost, sfx.length);
            }

            if (loop)
            {
                _loopingSounds.Add(audioSource);
            }

            // we return the audiosource reference
            return audioSource;
        }


        /// <summary>
        /// Stops the looping sounds if there are any
        /// </summary>
        /// <param name="source">Source.</param>
        public virtual void StopLoopingSound(AudioSource source)
		{
			if (source != null)
			{
				_loopingSounds.Remove (source);
				Destroy(source.gameObject);
			}
		}

        /// <summary>
        /// Sets the music on/off setting based on the value in parameters
        /// This value will be saved, and any music played after that setting change will comply
        /// </summary>
        /// <param name="status"></param>
		protected virtual void SetMusic(bool status)
		{
			Settings.MusicOn = status;
			SaveSoundSettings ();
            if (status)
            {
                UnmuteBackgroundMusic();
            }
            else
            {
                MuteBackgroundMusic();
            }
		}

        /// <summary>
        /// Sets the SFX on/off setting based on the value in parameters
        /// This value will be saved, and any SFX played after that setting change will comply
        /// </summary>
        /// <param name="status"></param>
		protected virtual void SetSfx(bool status)
		{
			Settings.SfxOn = status;
			SaveSoundSettings ();
		}

        /// <summary>
        /// Sets the music setting to On
        /// </summary>
		public virtual void MusicOn() { SetMusic (true); }

        /// <summary>
        /// Sets the Music setting to Off
        /// </summary>
		public virtual void MusicOff() { SetMusic (false); }

        /// <summary>
        /// Sets the SFX setting to On
        /// </summary>
		public virtual void SfxOn() { SetSfx (true); }

        /// <summary>
        /// Sets the SFX setting to Off
        /// </summary>
		public virtual void SfxOff() { SetSfx (false); }

        /// <summary>
        /// Saves the sound settings to file
        /// </summary>
		protected virtual void SaveSoundSettings()
		{
			SaveLoadManager.Save(Settings, _saveFileName, _saveFolderName);
		}

        /// <summary>
        /// Loads the sound settings from file (if found)
        /// </summary>
		protected virtual void LoadSoundSettings()
		{
			SoundSettings settings = (SoundSettings)SaveLoadManager.Load(_saveFileName, _saveFolderName);
			if (settings != null)
			{
				Settings = settings;
			}
		}

        /// <summary>
        /// Resets the sound settings by destroying the save file
        /// </summary>
		protected virtual void ResetSoundSettings()
		{
			SaveLoadManager.DeleteSave(_saveFileName, _saveFolderName);
		}

        /// <summary>
        /// When we grab a sfx event, we play the corresponding sound
        /// </summary>
        /// <param name="sfxEvent"></param>
		public virtual void OnMMSfxEvent(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f)
        {
            PlaySound(clipToPlay, this.transform.position, pitch, 0.0f, 0.0f, volume, false, audioGroup: audioGroup);
        }

        /// <summary>
        /// Watches for game events to mute sfx if needed
        /// </summary>
        /// <param name="gameEvent"></param>
		public virtual void OnMMEvent(MMGameEvent gameEvent)
		{
			if (MuteSfxOnPause)
			{
				if (gameEvent.EventName == "inventoryOpens")
				{
					MuteAllSfx ();
				}
				if (gameEvent.EventName == "inventoryCloses")
				{
					UnmuteAllSfx ();
				}
			}
		} 

        /// <summary>
        /// Watches for pause events to cut the sound on pause
        /// </summary>
        /// <param name="engineEvent"></param>
		public virtual void OnMMEvent(CorgiEngineEvent engineEvent)
		{
			if (engineEvent.EventType == CorgiEngineEventTypes.Pause)
			{
				if (MuteSfxOnPause)
				{
					MuteAllSfx ();
				}
			}
			if (engineEvent.EventType == CorgiEngineEventTypes.UnPause)
			{
				if (MuteSfxOnPause)
				{
					UnmuteAllSfx ();
				}
			}
		} 

        /// <summary>
        /// Mutes all sfx currently playing
        /// </summary>
		protected virtual void MuteAllSfx()
		{
			foreach(AudioSource source in _loopingSounds)
			{
				if (source != null)
				{
					source.mute = true;	
				}
			}
		}

        /// <summary>
        /// Unmutes all sfx currently playing
        /// </summary>
		protected virtual void UnmuteAllSfx()
		{
			foreach(AudioSource source in _loopingSounds)
			{
				if (source != null)
				{
					source.mute = false;	
				}
			}
		}

        /// <summary>
        /// Unmutes the background music
        /// </summary>
        public virtual void UnmuteBackgroundMusic()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.mute = false;
            }
        }

        /// <summary>
        /// Mutes the background music
        /// </summary>
        public virtual void MuteBackgroundMusic()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.mute = true;
            }
        }

        /// <summary>
        /// On enable we start listening for events
        /// </summary>
        protected virtual void OnEnable()
        {
            MMSfxEvent.Register(OnMMSfxEvent);
            this.MMEventStartListening<MMGameEvent>();	
			this.MMEventStartListening<CorgiEngineEvent>();
			LoadSoundSettings ();
			_loopingSounds = new List<AudioSource> ();
		}

        /// <summary>
        /// On disable we stop listening for events
        /// </summary>
		protected virtual void OnDisable()
		{
			if (_enabled)
            {
                MMSfxEvent.Unregister(OnMMSfxEvent);
                this.MMEventStopListening<MMGameEvent>();	
				this.MMEventStopListening<CorgiEngineEvent>();	
			}
		}
	}
}