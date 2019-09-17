using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MoreMountains.Tools;

namespace MoreMountains.MMInterface
{	
	/// <summary>
	/// A component to handle popups, their opening and closing
	/// </summary>
	public class MMPopup : MonoBehaviour 
	{
		/// true if the popup is currently open
		public bool CurrentlyOpen = false;

		[Header("Fader")]
		public float FaderOpenDuration = 0.2f;
		public float FaderCloseDuration = 0.2f;
		public float FaderOpacity = 0.8f;

		protected Animator _animator;

		/// <summary>
		/// On Start, we initialize our popup
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// On Init, we grab our animator and store it for future use
		/// </summary>
		protected virtual void Initialization()
		{
			_animator = GetComponent<Animator> ();
		}

		/// <summary>
		/// On update, we update our animator parameter
		/// </summary>
		protected virtual void Update()
		{
			if (_animator != null)
			{
				_animator.SetBool ("Closed", !CurrentlyOpen);
			}
		}

		/// <summary>
		/// Opens the popup
		/// </summary>
		public virtual void Open()
		{
			if (CurrentlyOpen)
			{
				return;
			}

			MMFadeEvent.Trigger(FaderOpenDuration, FaderOpacity);
			_animator.SetTrigger ("Open");
			CurrentlyOpen = true;
		}

		/// <summary>
		/// Closes the popup
		/// </summary>
		public virtual void Close()
		{
			if (!CurrentlyOpen)
			{
				return;
			}

			MMFadeEvent.Trigger(FaderCloseDuration, 0f);
			_animator.SetTrigger ("Close");
			CurrentlyOpen = false;
		}

	}
}
