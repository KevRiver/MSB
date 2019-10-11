using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this component to your Cinemachine Virtual Camera to have it shake when calling its ShakeCamera methods.
    /// </summary>
    public class MMCinemachineCameraShaker : MonoBehaviour
    {
        [Header("Settings")]
        /// the channel to receive events on
        public int Channel = 0;
        /// The default amplitude that will be applied to your shakes if you don't specify one
        public float DefaultShakeAmplitude = .5f;
        /// The default frequency that will be applied to your shakes if you don't specify one
        public float DefaultShakeFrequency = 10f;
        [MMFReadOnly]
        /// the amplitude of the camera's noise when it's idle
        public float IdleAmplitude;
        [MMFReadOnly]
        /// the frequency of the camera's noise when it's idle
        public float IdleFrequency = 1f;

        public float LerpSpeed = 5f;

        [Header("Test")]
        public float TestDuration = 0.3f;
        public float TestAmplitude = 2f;
        public float TestFrequency = 20f;

        [MMFInspectorButton("TestShake")]
        public bool TestShakeButton;

        protected Vector3 _initialPosition;
        protected Quaternion _initialRotation;

        protected Cinemachine.CinemachineBasicMultiChannelPerlin _perlin;
        protected Cinemachine.CinemachineVirtualCamera _virtualCamera;

        protected float _targetAmplitude;
        protected float _targetFrequency;

        private Coroutine _shakeCoroutine;

        /// <summary>
        /// On awake we grab our components
        /// </summary>
        protected virtual void Awake()
        {
            _virtualCamera = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            _perlin = _virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }

        /// <summary>
        /// On Start we reset our camera to apply our base amplitude and frequency
        /// </summary>
        protected virtual void Start()
        {
            if (_perlin != null)
            {
                IdleAmplitude = _perlin.m_AmplitudeGain;
                IdleFrequency = _perlin.m_FrequencyGain;
            }            

            _targetAmplitude = IdleAmplitude;
            _targetFrequency = IdleFrequency;
        }

        protected virtual void Update()
        {
            if (_perlin != null)
            {
                _perlin.m_AmplitudeGain = _targetAmplitude;
                _perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, _targetFrequency, Time.deltaTime * LerpSpeed);
            }
        }

        /// <summary>
        /// Use this method to shake the camera for the specified duration (in seconds) with the default amplitude and frequency
        /// </summary>
        /// <param name="duration">Duration.</param>
        public virtual void ShakeCamera(float duration, bool infinite)
        {
            StartCoroutine(ShakeCameraCo(duration, DefaultShakeAmplitude, DefaultShakeFrequency, infinite));
        }

        /// <summary>
        /// Use this method to shake the camera for the specified duration (in seconds), amplitude and frequency
        /// </summary>
        /// <param name="duration">Duration.</param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="frequency">Frequency.</param>
        public virtual void ShakeCamera(float duration, float amplitude, float frequency, bool infinite)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }
            _shakeCoroutine = StartCoroutine(ShakeCameraCo(duration, amplitude, frequency, infinite));
        }

        /// <summary>
        /// This coroutine will shake the 
        /// </summary>
        /// <returns>The camera co.</returns>
        /// <param name="duration">Duration.</param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="frequency">Frequency.</param>
        protected virtual IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency, bool infinite)
        {
            _targetAmplitude  = amplitude;
            _targetFrequency = frequency;
            if (!infinite)
            {
                yield return new WaitForSeconds(duration);
                CameraReset();
            }                        
        }

        /// <summary>
        /// Resets the camera's noise values to their idle values
        /// </summary>
        public virtual void CameraReset()
        {
            _targetAmplitude = IdleAmplitude;
            _targetFrequency = IdleFrequency;
        }

        public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, bool infinite, int channel)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }
            this.ShakeCamera(duration, amplitude, frequency, infinite);
        }

        public virtual void OnCameraShakeStopEvent(int channel)
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }            
            CameraReset();
        }

        protected virtual void OnEnable()
        {
            MMCameraShakeEvent.Register(OnCameraShakeEvent);
            MMCameraShakeStopEvent.Register(OnCameraShakeStopEvent);
        }

        protected virtual void OnDisable()
        {
            MMCameraShakeEvent.Unregister(OnCameraShakeEvent);
            MMCameraShakeStopEvent.Unregister(OnCameraShakeStopEvent);
        }

        protected virtual void TestShake()
        {
            MMCameraShakeEvent.Trigger(TestDuration, TestAmplitude, TestFrequency, false, 0);
        }
    }
}