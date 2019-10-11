using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Audio;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("")]
    [FeedbackPath("Sounds/Corgi Engine Sound")]
    [FeedbackHelp("This feedback lets you play a sound through the Corgi Engine's SoundManager")]
    public class MMFeedbackCorgiEngineSound : MMFeedback
    {
        [Header("Corgi Engine Sound")]
        public AudioClip SoundFX;
        public bool Loop = false;
        
        protected AudioSource _audioSource;

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                if (SoundFX != null)
                {
                    _audioSource = SoundManager.Instance.PlaySound(SoundFX, transform.position, Loop);
                }
            }
        }
        
        /// <summary>
        /// This method describes what happens when the feedback gets stopped
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Loop)
            {
                SoundManager.Instance.StopLoopingSound(_audioSource);
            }
            _audioSource = null;
        }
    }
}
