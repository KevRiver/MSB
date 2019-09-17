using UnityEngine;
using System.Collections;
using System;
using MoreMountains.Tools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MoreMountains.MMInterface
{	
	/// <summary>
	/// A class to handle a carousel of UI elements, placed in an HorizontalLayoutGroup. 
	/// All elements of the carousel need to have the same width.
	/// </summary>
	public class MMCarousel : MonoBehaviour
	{
		[Header("Binding")]
		/// the layout group that contains all carousel's elements
		public HorizontalLayoutGroup Content;

		public Camera UICamera;

		[Header("Optional Buttons Binding")]
		/// the button that moves the carousel to the left
		public MMTouchButton LeftButton;
		/// the button that moves the carousel to the right
		public MMTouchButton RightButton;

		[Header("Carousel Setup")]
		/// the initial and current index
		public int CurrentIndex = 0;
		/// the number of items in the carousel that should be moved every time
		public int Pagination = 1;
		/// the percentage of distance that, when reached, will stop movement
		public float ThresholdInPercent = 1f;

		[Header("Speed")]
		/// the duration (in seconds) of the carousel's movement 
		public float MoveDuration = 0.05f;

		[Header("Focus")]
		/// Bind here the carousel item that should have focus initially
		public GameObject InitialFocus;

		[Header("Keyboard/Gamepad")]
		/// the number 
		//public int 


		//protected float ElementWidth { get { return (Content.minWidth - (Content.spacing * (Content.flexibleWidth - 1))) / Content.flexibleWidth; }}
		protected float _elementWidth;
		protected int _contentLength = 0;
		protected float _spacing;
		protected Vector2 _initialPosition;
		protected RectTransform _rectTransform;

		protected bool _lerping = false;
		protected float _lerpStartedTimestamp;
		protected Vector2 _startPosition;
		protected Vector2 _targetPosition;

		/// <summary>
		/// On Start we initialize our carousel
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// Initializes the carousel, grabs the rect transform, computes the elements' dimensions, and inits position
		/// </summary>
		protected virtual void Initialization()
		{
			_rectTransform = Content.gameObject.GetComponent<RectTransform> ();
			_initialPosition = _rectTransform.anchoredPosition;

			// we compute the Content's element width
			_contentLength = 0;
			foreach (Transform tr in Content.transform) 
			{ 
				_elementWidth = tr.gameObject.GetComponentNoAlloc<RectTransform>().sizeDelta.x;
				_contentLength++;
			}
			_spacing = Content.spacing;

			// we position our carousel at the desired initial index
			_rectTransform.anchoredPosition = DeterminePosition ();

			if (InitialFocus != null)
			{
				EventSystem.current.SetSelectedGameObject(InitialFocus, null);
			}

		}

		/// <summary>
		/// Moves the carousel to the left.
		/// </summary>
		public virtual void MoveLeft()
		{
			if (!CanMoveLeft())
			{
				return;
			}
			else
			{				
				CurrentIndex -= Pagination;
				MoveToCurrentIndex ();	
			}
		}

		/// <summary>
		/// Moves the carousel to the right.
		/// </summary>
		public virtual void MoveRight()
		{
			if (!CanMoveRight())
			{
				return;
			}
			else
			{
				CurrentIndex += Pagination;
				MoveToCurrentIndex ();	
			}
		}

		/// <summary>
		/// Initiates movement to the current index
		/// </summary>
		protected virtual void MoveToCurrentIndex ()
		{
			_startPosition = _rectTransform.anchoredPosition;
			_targetPosition = DeterminePosition ();
			_lerping = true;
			_lerpStartedTimestamp = Time.time;
		}

		/// <summary>
		/// Determines the target position based on the current index value.
		/// </summary>
		/// <returns>The position.</returns>
		protected virtual Vector2 DeterminePosition()
		{
			return _initialPosition - (Vector2.right * CurrentIndex * (_elementWidth + _spacing));
		}

		public virtual bool CanMoveLeft()
		{
			return (CurrentIndex - Pagination >= 0);
				
		}

		/// <summary>
		/// Determines whether this carousel can move right.
		/// </summary>
		/// <returns><c>true</c> if this instance can move right; otherwise, <c>false</c>.</returns>
		public virtual bool CanMoveRight()
		{
			return (CurrentIndex + Pagination < _contentLength);
		}

		/// <summary>
		/// On Update we move the carousel if required, and handles button states
		/// </summary>
		protected virtual void Update()
		{
			if (_lerping)
			{
				LerpPosition ();
			}
			HandleButtons ();
			HandleFocus ();
		}

		protected virtual void HandleFocus()
		{
			if (!_lerping && Time.timeSinceLevelLoad > 0.5f)
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					if (UICamera.WorldToScreenPoint(EventSystem.current.currentSelectedGameObject.transform.position).x < 0)
					{
						MoveLeft ();
					}
					if (UICamera.WorldToScreenPoint(EventSystem.current.currentSelectedGameObject.transform.position).x > Screen.width)
					{
						MoveRight ();
					}	
				}
			}
		}

		/// <summary>
		/// Handles the buttons, enabling and disabling them if needed
		/// </summary>
		protected virtual void HandleButtons()
		{
			if (LeftButton != null) 
			{ 
				if (CanMoveLeft())
				{
					LeftButton.EnableButton (); 
				}
				else
				{
					LeftButton.DisableButton (); 
				}	
			}
			if (RightButton != null) 
			{ 
				if (CanMoveRight())
				{
					RightButton.EnableButton (); 
				}
				else
				{
					RightButton.DisableButton (); 
				}	
			}
		}

		/// <summary>
		/// Lerps the carousel's position.
		/// </summary>
		protected virtual void LerpPosition()
		{
			float timeSinceStarted = Time.time - _lerpStartedTimestamp;
			float percentageComplete = timeSinceStarted / MoveDuration;

			_rectTransform.anchoredPosition = Vector2.Lerp (_startPosition, _targetPosition, percentageComplete);

			//When we've completed the lerp, we set _isLerping to false
			if(percentageComplete >= ThresholdInPercent)
			{
				_lerping = false;
			}
		}
	}
}