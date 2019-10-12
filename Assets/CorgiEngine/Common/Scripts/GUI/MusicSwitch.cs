using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class MusicSwitch : MonoBehaviour
    {
        public virtual void On()
        {
            SoundManager.Instance.MusicOn();
        }

        public virtual void Off()
        {
            SoundManager.Instance.MusicOff();
        }        
    }
}
