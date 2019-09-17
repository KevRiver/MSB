using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Handles all GUI effects and changes for Multiplayer scenes
	/// </summary>
	public class MultiplayerGUIManager : GUIManager 
	{
		[Header("Multiplayer Endgame")]
		/// the game over splash screen object
		public GameObject MPEndGameSplash;
		/// the game over text object
		public Text MPEndGameText;

		/// <summary>
		/// Shows the multiplayer endgame screen
		/// </summary>
		public virtual void ShowMultiplayerEndgame()
		{
			MPEndGameSplash.SetActive (true);
		}

		/// <summary>
		/// Sets the multiplayer endgame text.
		/// </summary>
		/// <param name="text">Text.</param>
		public virtual void SetMultiplayerEndgameText (string text)
		{
			MPEndGameText.text = text;
		}

	}
}