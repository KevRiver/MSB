using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// Level selector GUI.
	/// </summary>
	public class LevelSelectorGUI : MonoBehaviour 
	{
		/// the panel object that contains the name of the level
		public Image LevelNamePanel;
		/// the text where the level name should be displayed
		public Text LevelNameText;
		/// the offset to apply to the level name 
		public Vector2 LevelNameOffset;

		/// <summary>
		/// On start, disables the HUD and optionally the level name panels
		/// </summary>
		protected virtual void Start ()
	    {
	       GUIManager.Instance.SetHUDActive(false);

	       if (LevelNamePanel!=null && LevelNameText!=null)
			{
				LevelNamePanel.enabled=false;
				LevelNameText.enabled=false;
	       }
	    }

		/// <summary>
		/// Sets the name of the level to the one in parameters
		/// </summary>
		/// <param name="levelName">Level name.</param>
	    public virtual void SetLevelName(string levelName)
		{
			LevelNameText.text=levelName;
			LevelNamePanel.enabled=true;
			LevelNameText.enabled=true;
		}

		/// <summary>
		/// Turns the name of the level off.
		/// </summary>
		public virtual void TurnOffLevelName()
		{
			LevelNamePanel.enabled=false;
			LevelNameText.enabled=false;
		}
	}

}