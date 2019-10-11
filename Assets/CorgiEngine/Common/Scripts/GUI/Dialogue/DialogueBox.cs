using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Dialogue box class. Don't add this directly to your game, look at DialogueZone instead.
	/// </summary>
	public class DialogueBox : MonoBehaviour 
	{
		/// the text panel background
		public Image TextPanel;
		/// the arrow pointing down on the dialogue box
		public Image TextPanelArrowDown;
		/// the text to display
		public Text DialogueText;
		/// the Button A prompt
		public GameObject ButtonA;

	    protected Color _backgroundColor;
	    protected Color _textColor;
	    protected SpriteRenderer _buttonSpriteRenderer;
		
		/// <summary>
		/// Changes the text.
		/// </summary>
		/// <param name="newText">New text.</param>
		public virtual void ChangeText(string newText)
		{
			DialogueText.text = newText;
		}
		
		/// <summary>
		/// Activates the ButtonA prompt
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		public virtual void ButtonActive(bool state)
		{
			ButtonA.SetActive(state);
		}
		
		/// <summary>
		/// Changes the color of the dialogue box to the ones in parameters
		/// </summary>
		/// <param name="backgroundColor">Background color.</param>
		/// <param name="textColor">Text color.</param>
		public virtual void ChangeColor(Color backgroundColor, Color textColor)
		{		
			_backgroundColor=backgroundColor;
			_textColor=textColor;
		
			Color newBackgroundColor=new Color(_backgroundColor.r,_backgroundColor.g,_backgroundColor.b,0);
			Color newTextColor=new Color(_textColor.r,_textColor.g,_textColor.b,0);
			
			TextPanel.color=newBackgroundColor;
			TextPanelArrowDown.color=newBackgroundColor;
			DialogueText.color=newTextColor;
			
			_buttonSpriteRenderer=ButtonA.GetComponent<SpriteRenderer>();
			_buttonSpriteRenderer.material.color=new Color(1f,1f,1f,0f);
		}
		
		/// <summary>
		/// Fades the dialogue box in.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public virtual void FadeIn(float duration)
		{	
			if (TextPanel != null) 
			{
				StartCoroutine (MMFade.FadeImage (TextPanel, duration, _backgroundColor));
			}
			if (TextPanelArrowDown != null) 
			{
				StartCoroutine (MMFade.FadeImage (TextPanelArrowDown, duration, _backgroundColor));
			}
			if (DialogueText != null) 
			{
				StartCoroutine (MMFade.FadeText (DialogueText, duration, _textColor));
			}
			if (_buttonSpriteRenderer != null) 
			{
				StartCoroutine (MMFade.FadeSprite (_buttonSpriteRenderer, duration, new Color (1f, 1f, 1f, 1f)));			
			}
		}
		
		/// <summary>
		/// Fades the dialogue box out.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public virtual void FadeOut(float duration)
		{				
			Color newBackgroundColor=new Color(_backgroundColor.r,_backgroundColor.g,_backgroundColor.b,0);
			Color newTextColor=new Color(_textColor.r,_textColor.g,_textColor.b,0);
		
			StartCoroutine(MMFade.FadeImage(TextPanel, duration,newBackgroundColor));
			StartCoroutine(MMFade.FadeImage(TextPanelArrowDown,duration,newBackgroundColor));
			StartCoroutine(MMFade.FadeText(DialogueText,duration,newTextColor));
			StartCoroutine(MMFade.FadeSprite(_buttonSpriteRenderer,duration,new Color(1f,1f,1f,0f)));			
		}
		
		/// <summary>
		/// Hides the dialogue box arrow.
		/// </summary>
		public virtual void HideArrow()
		{
			TextPanelArrowDown.enabled=false;
		}
	}
}