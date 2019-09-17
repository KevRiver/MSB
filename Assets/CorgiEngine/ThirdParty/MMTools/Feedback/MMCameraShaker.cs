using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

namespace MoreMountains.Tools
{
	[Serializable]
	/// <summary>
	/// Camera shake properties
	/// </summary>
	public struct MMCameraShakeProperties
	{
		public float Duration;
		public float Amplitude;
		public float Frequency;

        public MMCameraShakeProperties(float duration, float amplitude, float frequency)
        {
            Duration = duration;
            Amplitude = amplitude;
            Frequency = frequency;
        }
    }

    public enum MMCameraZoomModes { For, Set, Reset }

    public struct MMCameraZoomEvent
    {
        public MMCameraZoomModes Mode;
        public float FieldOfView;
        public float TransitionDuration;
        public float Duration;

        public MMCameraZoomEvent(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration)
        {
            Mode = mode;
            FieldOfView = newFieldOfView;
            TransitionDuration = transitionDuration;
            Duration = duration;

        }
        static MMCameraZoomEvent e;
        public static void Trigger(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration)
        {
            e.Mode = mode;
            e.FieldOfView = newFieldOfView;
            e.TransitionDuration = transitionDuration;
            e.Duration = duration;
            MMEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// Camera shake event, trigger this to shake the camera
    /// </summary>
    public struct MMCameraShakeEvent
	{
		public MMCameraShakeProperties Properties;
		public MMCameraShakeEvent(float duration, float amplitude, float frequency)
		{
			Properties.Duration = duration;
			Properties.Amplitude = amplitude;
			Properties.Frequency = frequency;
        }
        static MMCameraShakeEvent e;
        public static void Trigger(float duration, float amplitude, float frequency)
        {
            e.Properties.Duration = duration;
            e.Properties.Amplitude = amplitude;
            e.Properties.Frequency = frequency;
            MMEventManager.TriggerEvent(e);
        }
    }

	[RequireComponent(typeof(MMShaker))]
	/// <summary>
	/// A class to add to your camera. It'll listen to MMCameraShakeEvents and will shake your camera accordingly
	/// </summary>
	public class MMCameraShaker : MonoBehaviour, MMEventListener<MMCameraShakeEvent>
	{
		protected MMShaker _shaker;

		/// <summary>
		/// On Awake, grabs the MMShaker component
		/// </summary>
		protected virtual void Awake()
		{
			_shaker = GetComponent<MMShaker>();
		}

		/// <summary>
		/// Shakes the camera for Duration seconds, by the desired amplitude and frequency
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="amplitude">Amplitude.</param>
		/// <param name="frequency">Frequency.</param>
		public virtual void ShakeCamera(float duration, float amplitude, float frequency)
		{
			_shaker.PositionAmplitudeMin = Vector3.one * -amplitude;
			_shaker.PositionAmplitudeMax = Vector3.one * amplitude;
			_shaker.PositionFrequencyMin = frequency;
			_shaker.PositionFrequencyMax = frequency;
			_shaker.ShakeDuration = duration;
		}

		/// <summary>
		/// When a MMCameraShakeEvent is caught, shakes the camera
		/// </summary>
		/// <param name="shakeEvent">Shake event.</param>
		public virtual void OnMMEvent(MMCameraShakeEvent shakeEvent)
		{
			this.ShakeCamera (shakeEvent.Properties.Duration, shakeEvent.Properties.Amplitude, shakeEvent.Properties.Frequency);
		}

		/// <summary>
		/// On enable, starts listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMCameraShakeEvent>();
		}

		/// <summary>
		/// On disable, stops listening to events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMCameraShakeEvent>();
		}

	}
}