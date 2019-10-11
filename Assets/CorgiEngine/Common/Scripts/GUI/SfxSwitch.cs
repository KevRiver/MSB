using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class SfxSwitch : MonoBehaviour
    {
        public virtual void On()
        {
            SoundManager.Instance.SfxOn();
        }

        public virtual void Off()
        {
            SoundManager.Instance.SfxOff();
        }
    }
}
