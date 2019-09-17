using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	

	[RequireComponent(typeof(Collider2D))]

	/// <summary>
	/// Add this class to an empty component. It will automatically add a boxcollider2d, set it to "is trigger". Then customize the dialogue zone
	/// through the inspector.
	/// </summary>

	public class DialogueZone : ButtonActivated 
	{	
		[Header("Dialogue Look")]
		/// the color of the text background.
		public Color TextBackgroundColor=Color.black;
		/// the color of the text
		public Color TextColor=Color.white;
		/// if true, the dialogue box will have a small, downward pointing arrow
		public bool ArrowVisible=true;
		/// the font that should be used to display the text
		public Font TextFont;
		/// the size of the font
		public int TextSize = 20;
		/// the text alignment in the box used to display the text
		public TextAnchor Alignment = TextAnchor.MiddleCenter;


		[Space(10)]
		
		[Header("Dialogue Speed (in seconds)")]
		/// the duration of the in and out fades
		public float FadeDuration=0.2f;
		/// the time between two dialogues 
		public float TransitionTime=0.2f;
			
		[Space(10)]	
		[Header("Dialogue Position")]
		/// the distance from the top of the box collider the dialogue box should appear at
		public float DistanceFromTop=0;
		/// if this is true, the dialogue boxes will follow the zone's position
		public bool BoxesFollowZone = false;
		
		[Space(10)]	
		[Header("Player Movement")]
		/// if this is set to true, the character will be able to move while dialogue is in progress
		public bool CanMoveWhileTalking = true;
		[Header("Press button to go from one message to the next ?")]
		public bool ButtonHandled=true;
		/// duration of the message. only considered if the box is not button handled
		[Header("Only if the dialogue is not button handled :")]
		[Range (1, 100)]
		public float MessageDuration=3f;
		
		[Space(10)]
		
		[Header("Activations")]
		/// true if can be activated more than once
		public bool ActivableMoreThanOnce=true;
		/// if the zone is activable more than once, how long should it remain inactive between up times ?
		[Range (1, 100)]
		public float InactiveTime=2f;
		
		[Space(10)]
		
		/// the dialogue lines
		[Multiline]
		public string[] Dialogue;

	    /// private variables
	    protected DialogueBox _dialogueBox;
	    protected bool _activated=false;
	    protected bool _playing=false;
	    protected int _currentIndex;
	    protected bool _activable=true;
		protected WaitForSeconds _transitionTimeWFS;
		protected WaitForSeconds _messageDurationWFS;
		protected WaitForSeconds _inactiveTimeWFS;

		/// <summary>
	    /// Determines whether this instance can show button prompt.
	    /// </summary>
	    /// <returns><c>true</c> if this instance can show prompt; otherwise, <c>false</c>.</returns>
	    /*public override bool CanShowPrompt()
	    {
			if ( (_buttonA==null) && _activable && !_playing )
	    	{
	    		return true;
	    	}
	    	return false;
	    }*/
	

	    /// <summary>
	    /// Initializes the dialogue zone
	    /// </summary>
		protected override void OnEnable () 
		{		
			base.OnEnable();
			_currentIndex=0;					
			_transitionTimeWFS = new WaitForSeconds (TransitionTime);
			_messageDurationWFS = new WaitForSeconds (MessageDuration);
			_inactiveTimeWFS = new WaitForSeconds (InactiveTime);
		}

		/// <summary>
		/// When the button is pressed we start the dialogue
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			if (_playing && !ButtonHandled)
			{
				return;
            }
            base.TriggerButtonAction ();
			StartDialogue();
			ActivateZone ();
		}
			
		/// <summary>
		/// When triggered, either by button press or simply entering the zone, starts the dialogue
		/// </summary>
		public virtual void StartDialogue()
        {
            // if the dialogue zone has no box collider, we do nothing and exit
            if (_collider==null)
            {
                return;
            }				
			
			// if the zone has already been activated and can't be activated more than once.
			if (_activated && !ActivableMoreThanOnce)
            {
                return;
            }				
				
			// if the zone is not activable, we do nothing and exit
			if (!_activable)
            {
                return;
            }				

            // if the player can't move while talking, we notify the game manager
            if (!CanMoveWhileTalking)
			{
				LevelManager.Instance.FreezeCharacters();
				if (ShouldUpdateState)
				{
					_characterButtonActivation.GetComponent<Character>().MovementState.ChangeState(CharacterStates.MovementStates.Idle);
				}
			}
										
			// if it's not already playing, we'll initialize the dialogue box
			if (!_playing)
			{	
				// we instantiate the dialogue box
				GameObject dialogueObject = (GameObject)Instantiate(Resources.Load("GUI/DialogueBox"));
				_dialogueBox = dialogueObject.GetComponent<DialogueBox>();		
				// we set its position
				_dialogueBox.transform.position=new Vector2(_collider.bounds.center.x,_collider.bounds.max.y+DistanceFromTop); 
				// we set the color's and background's colors
				_dialogueBox.ChangeColor(TextBackgroundColor,TextColor);
				// if it's a button handled dialogue, we turn the A prompt on
				_dialogueBox.ButtonActive(ButtonHandled);
				// if font settings have been specified, we set them

				if (BoxesFollowZone)
				{
					_dialogueBox.transform.SetParent (this.gameObject.transform);
				}

				if (TextFont != null)
				{
					_dialogueBox.DialogueText.font = TextFont;
				}
				if (TextSize != 0)
				{
					_dialogueBox.DialogueText.fontSize = TextSize;
				}
				_dialogueBox.DialogueText.alignment = Alignment;

				// if we don't want to show the arrow, we tell that to the dialogue box
				if (!ArrowVisible)
				{
					_dialogueBox.HideArrow();			
				}
				
				// the dialogue is now playing
				_playing=true;
			}
			// we start the next dialogue
			StartCoroutine(PlayNextDialogue());
		}

	    /// <summary>
	    /// Plays the next dialogue in the queue
	    /// </summary>
	    protected virtual IEnumerator PlayNextDialogue()
		{		
			// we check that the dialogue box still exists
			if (_dialogueBox == null) 
			{
				yield break;
			}
			// if this is not the first message
			if (_currentIndex!=0)
			{
				// we turn the message off
				_dialogueBox.FadeOut(FadeDuration);	
				// we wait for the specified transition time before playing the next dialogue
				yield return _transitionTimeWFS;
			}	
			
			// if we've reached the last dialogue line, we exit
			if (_currentIndex>=Dialogue.Length)
			{
				_currentIndex=0;
				Destroy(_dialogueBox.gameObject);
				_collider.enabled=false;
				// we set activated to true as the dialogue zone has now been turned on		
				_activated=true;
				// we let the player move again
				if (!CanMoveWhileTalking)
				{
					LevelManager.Instance.UnFreezeCharacters();
				}
				if ((_characterButtonActivation!=null))
				{				
					_characterButtonActivation.InButtonActivatedZone=false;
					_characterButtonActivation.ButtonActivatedZone=null;
				}
				// we turn the zone inactive for a while
				if (ActivableMoreThanOnce)
				{
					_activable=false;
					_playing=false;
					StartCoroutine(Reactivate());
				}
				else
				{
					gameObject.SetActive(false);
				}
				yield break;
			}

			// we check that the dialogue box still exists
			if (_dialogueBox.DialogueText!=null)
			{
				// every dialogue box starts with it fading in
				_dialogueBox.FadeIn(FadeDuration);
				// then we set the box's text with the current dialogue
				_dialogueBox.DialogueText.text=Dialogue[_currentIndex];
			}
			
			_currentIndex++;
			
			// if the zone is not button handled, we start a coroutine to autoplay the next dialogue
			if (!ButtonHandled)
			{
				StartCoroutine(AutoNextDialogue());
			}
		}

		/// <summary>
		/// Automatically goes to the next dialogue line
		/// </summary>
		/// <returns>The next dialogue.</returns>
	    protected virtual IEnumerator AutoNextDialogue()
		{
			// we wait for the duration of the message
			yield return _messageDurationWFS;
			StartCoroutine(PlayNextDialogue());
		}

		/// <summary>
		/// Reactivate the dialogue zone
		/// </summary>
	    protected virtual IEnumerator Reactivate()
		{
			yield return _inactiveTimeWFS;
			_collider.enabled=true;
			_activable=true;
			_playing=false;
			_currentIndex=0;

			if (AlwaysShowPrompt)
			{
				ShowPrompt();
			}
			
		}			    
	}
}