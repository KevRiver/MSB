using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace MoreMountains.Tools
{
    /// <summary>
    /// A class used to control the intensity of a light
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class LightController : MonoBehaviour
    {
        public float Intensity = 1f;
        public float Multiplier = 1f;

        protected Light _targetLight;

        protected virtual void Start()
        {
            _targetLight = this.gameObject.GetComponent<Light>();
        }

        protected virtual void Update()
        {
            _targetLight.intensity = Intensity * Multiplier;
        }
    }
}
