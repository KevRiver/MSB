using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;

namespace MoreMountains.Tools
{
    [Serializable]
    public class BlinkPhase
    {
        public float PhaseDuration = 1f;
        public float OffDuration = 0.2f;
        public float OnDuration = 0.1f;
    }

    /// <summary>
    /// Add this class to a GameObject to make it rotate on itself
    /// </summary>
    public class MMBlink : MonoBehaviour
    {
        public enum Methods { SetGameObjectActive, MaterialAlpha, MaterialEmissionIntensity }

        [Header("Blink Method")]
        public Methods Method = Methods.SetGameObjectActive;

        [EnumCondition("Method", (int)Methods.SetGameObjectActive)]
        public GameObject TargetGameObject;

        [EnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity)]
        public Renderer TargetRenderer;
        [EnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity)]
        public float OffValue = 0f;
        [EnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity)]
        public float OnValue = 1f;
        [EnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity)]
        public bool LerpValue = true;
        [EnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity)]
        public MMTween.MMTweenCurve Curve = MMTween.MMTweenCurve.EaseInCubic;

        [Header("Activation")]

        public bool ResetCounterOnEnable = true;

        [InspectorButton("ResetCounter")]
        public bool ResetCounterButton;

        [Header("Blink Settings")]
        public List<BlinkPhase> Phases;

        [Header("Debug")]
        [ReadOnly]
        public bool Active = false;
        [ReadOnly]
        public int CurrentPhaseIndex = 0;
        [ReadOnly]
        public float LastBlinkAt = 0f;
        [ReadOnly]
        public float CurrentPhaseStartedAt = 0f;

        protected float _currentBlinkDuration;
        protected int _propertyID;

        protected Color _initialColor;
        protected Color _currentColor;
                
        protected virtual void Update()
        {
            DetermineState();
            Blink();
        }

        protected virtual void DetermineState()
        {
            DetermineCurrentPhase();

            if (Active)
            {
                if (Time.time - LastBlinkAt > Phases[CurrentPhaseIndex].OnDuration)
                {
                    Active = false;
                    LastBlinkAt = Time.time;
                }
            }
            else
            {
                if (Time.time - LastBlinkAt > Phases[CurrentPhaseIndex].OffDuration)
                {
                    Active = true;
                    LastBlinkAt = Time.time;
                }
            }
            _currentBlinkDuration = Active ? Phases[CurrentPhaseIndex].OnDuration : Phases[CurrentPhaseIndex].OffDuration;
        }

        protected virtual void Blink()
        {

            float currentValue = _currentColor.a;
            float initialValue = Active ? OffValue : OnValue;
            float targetValue = Active ? OnValue : OffValue;
            float newValue = targetValue;

            if (LerpValue)
            {
                newValue = MMTween.Tween(Time.time - LastBlinkAt, 0f, _currentBlinkDuration, initialValue, targetValue, Curve);
            }
            else
            {
                newValue = targetValue;
            }

            switch (Method)
            {
                case Methods.SetGameObjectActive:
                    TargetGameObject.SetActive(Active);
                    break;
                case Methods.MaterialAlpha:
                    _currentColor.a = newValue;
                    TargetRenderer.material.SetColor(_propertyID, _currentColor);
                    break;
                case Methods.MaterialEmissionIntensity:
                    _currentColor = _initialColor * newValue;
                    TargetRenderer.material.SetColor(_propertyID, _currentColor);
                    break;
            }
            
        }

        /// <summary>
        /// Determines the current phase index based on phase durations
        /// </summary>
        protected virtual void DetermineCurrentPhase()
        {
            // if the phase duration is null or less, we'll be in that phase forever, and return
            if (Phases[CurrentPhaseIndex].PhaseDuration <= 0)
            {
                return;
            }
            // if the phase's duration is elapsed, we move to the next phase
            if (Time.time - CurrentPhaseStartedAt > Phases[CurrentPhaseIndex].PhaseDuration)
            {
                CurrentPhaseIndex++;
                CurrentPhaseStartedAt = Time.time;
            }
            if (CurrentPhaseIndex > Phases.Count -1)
            {
                CurrentPhaseIndex = 0;
            }
        }
        

        protected virtual void OnEnable()
        {
            if (ResetCounterOnEnable)
            {
                ResetCounter();
            }            
        }

        public virtual void ResetCounter()
        {
            CurrentPhaseStartedAt = Time.time;
            CurrentPhaseIndex = 0;

            if (Method == Methods.MaterialAlpha)
            {
                _propertyID = Shader.PropertyToID("_Color");
                _initialColor = TargetRenderer.material.GetColor(_propertyID);
                _currentColor = _initialColor;
            }
            if (Method == Methods.MaterialEmissionIntensity)
            {
                _propertyID = Shader.PropertyToID("_EmissionColor");
                _initialColor = TargetRenderer.material.GetColor(_propertyID);
                _currentColor = _initialColor;
            }
        }

    }
}
