using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.Tools
{	
	public class MMProgressBarDemoAuto : MonoBehaviour 
	{

		public float CurrentValue = 0f;
		public float MinValue = 0f;
		public float MaxValue = 100f;
		public float Speed = 1f;

		protected float _direction = 1f;
		protected MMProgressBar _progressBar;

		protected virtual void Start()
		{
			Initialization ();
		}

		protected virtual void Initialization()
		{
			_progressBar = GetComponent<MMProgressBar> ();
		}

		protected virtual void Update()
		{
			_progressBar.UpdateBar (CurrentValue, MinValue, MaxValue);
			CurrentValue += Speed * Time.deltaTime * _direction;
			if ((CurrentValue <= MinValue) || (CurrentValue >= MaxValue))
			{
				_direction *= -1;
			}
		}
	}
}
