using UnityEngine;
using System.Collections;

namespace MoreMountains.Tools
{	
	public class MMShaker : MMWiggle 
	{
		public float ShakeDuration;

		public void Shake(float shakeDuration) 
		{
			ShakeDuration += shakeDuration;
		}

        public void Restart()
        {
            StartWiggleCoroutines();
        }

		protected override void Update()
		{
			base.Update();

			_wiggling = (ShakeDuration > 0f);
			if (_wiggling)
			{
				ShakeDuration -= Time.deltaTime;
			}
			else
			{
				ShakeDuration = 0f;
			}
		}
	}
}