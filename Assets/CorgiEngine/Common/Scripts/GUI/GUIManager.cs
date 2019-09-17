using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Handles all GUI effects and changes
	/// </summary>
	public class GUIManager : Singleton<GUIManager>, MMEventListener<LevelNameEvent>, MMEventListener<ControlsModeEvent>
    {
        [Header("Bindings")]
		/// the game object that contains the heads up display (avatar, health, points...)
		public GameObject HUD;
		/// the jetpack bar
		public MMProgressBar[] HealthBars;
		/// the jetpack bar
		public MMProgressBar[] JetPackBars;
		/// the panels and bars used to display current weapon ammo
		public AmmoDisplay[] AmmoDisplays;
		/// the pause screen game object
		public GameObject PauseScreen;	
		/// the time splash gameobject
		public GameObject TimeSplash;
		/// The mobile buttons
		public CanvasGroup Buttons;
		/// The mobile arrows
		public CanvasGroup Arrows;
		/// The mobile movement joystick
		public CanvasGroup Joystick;
		/// the points counter
		public Text PointsText;
		/// the level display
		public Text LevelText;

        [Header("Settings")]
        /// the pattern to apply when displaying the score
        public string PointsPattern = "000000";

        protected float _initialJoystickAlpha;
		protected float _initialButtonsAlpha;

		/// <summary>
		/// Initialization
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			if (Joystick!=null)
			{
				_initialJoystickAlpha=Joystick.alpha;
			}
			if (Buttons!=null)
			{
				_initialButtonsAlpha=Buttons.alpha;
			}
		}

	    /// <summary>
	    /// Initialization
	    /// </summary>
	    protected virtual void Start()
		{
			RefreshPoints();
		}

	    /// <summary>
	    /// Sets the HUD active or inactive
	    /// </summary>
	    /// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
	    public virtual void SetHUDActive(bool state)
	    {
	        if (HUD!= null)
	        { 
	            HUD.SetActive(state);
	        }
	        if (PointsText!= null)
	        { 
	            PointsText.enabled = state;
	        }
	        if (LevelText!= null)
	        { 
	            LevelText.enabled = state;
	        }
	    }

	    /// <summary>
	    /// Sets the avatar active or inactive
	    /// </summary>
	    /// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
	    public virtual void SetAvatarActive(bool state)
	    {
	        if (HUD != null)
	        {
	            HUD.SetActive(state);
	        }
	    }

		/// <summary>
		/// Called by the input manager, this method turns controls visible or not depending on what's been chosen
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		/// <param name="movementControl">Movement control.</param>
		public virtual void SetMobileControlsActive(bool state, InputManager.MovementControls movementControl = InputManager.MovementControls.Joystick)
		{
			if (Joystick != null)
			{
				Joystick.gameObject.SetActive(state);
				if (state && movementControl == InputManager.MovementControls.Joystick)
				{
					Joystick.alpha=_initialJoystickAlpha;
				}
				else
				{
					Joystick.alpha=0;
					Joystick.gameObject.SetActive (false);
				}
			}

			if (Arrows != null)
			{
				Arrows.gameObject.SetActive(state);
				if (state && movementControl == InputManager.MovementControls.Arrows)
				{
					Arrows.alpha=_initialJoystickAlpha;
				}
				else
				{
					Arrows.alpha=0;
					Arrows.gameObject.SetActive (false);
				}
			}

			if (Buttons != null)
			{
				Buttons.gameObject.SetActive(state);
				if (state)
				{
					Buttons.alpha=_initialButtonsAlpha;
				}
				else
				{
					Buttons.alpha=0;
					Buttons.gameObject.SetActive (false);
				}
			}
		}

		/// <summary>
		/// Sets the pause.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetPause(bool state)
		{
	        if (PauseScreen!= null)
	        { 
	    		PauseScreen.SetActive(state);
				EventSystem.current.sendNavigationEvents=true;
	        }
	    }

		/// <summary>
		/// Sets the jetpackbar active or not.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetJetpackBar(bool state, string playerID)
		{
			if (JetPackBars == null)
			{
				return;
			}

			foreach (MMProgressBar jetpackBar in JetPackBars)
			{
				if (jetpackBar != null)
		        { 
		        	if (jetpackBar.PlayerID == playerID)
		        	{
						jetpackBar.gameObject.SetActive(state);
		        	}					
		        }
			}	        
	    }

		/// <summary>
		/// Sets the ammo displays active or not
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		/// <param name="playerID">Player I.</param>
		public virtual void SetAmmoDisplays(bool state, string playerID)
		{
			if (AmmoDisplays == null)
			{
				return;
			}

			foreach (AmmoDisplay ammoDisplay in AmmoDisplays)
			{
				if (ammoDisplay != null)
				{ 
					if (ammoDisplay.PlayerID == playerID)
					{
						ammoDisplay.gameObject.SetActive(state);
					}					
				}
			}
		}

		/// <summary>
		/// Sets the time splash.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, turns the timesplash on.</param>
		public virtual void SetTimeSplash(bool state)
		{
	        if (TimeSplash != null)
	        {
	            TimeSplash.SetActive(state);
	        }
		}
		
		/// <summary>
		/// Sets the text to the game manager's points.
		/// </summary>
		public virtual void RefreshPoints()
		{
	        if (PointsText!= null)
	        { 
	    		PointsText.text = GameManager.Instance.Points.ToString(PointsPattern);
	        }
	    }

	    /// <summary>
	    /// Updates the health bar.
	    /// </summary>
	    /// <param name="currentHealth">Current health.</param>
	    /// <param name="minHealth">Minimum health.</param>
	    /// <param name="maxHealth">Max health.</param>
	    /// <param name="playerID">Player I.</param>
	    public virtual void UpdateHealthBar(float currentHealth,float minHealth,float maxHealth,string playerID)
	    {
			if (HealthBars == null) { return; }
			if (HealthBars.Length <= 0)	{ return; }

			foreach (MMProgressBar healthBar in HealthBars)
	    	{
				if (healthBar == null) { continue; }
				if (healthBar.PlayerID == playerID)
				{
					healthBar.UpdateBar(currentHealth,minHealth,maxHealth);
				}
	    	}

	    }

	    /// <summary>
	    /// Updates the jetpack bar.
	    /// </summary>
	    /// <param name="currentFuel">Current fuel.</param>
	    /// <param name="minFuel">Minimum fuel.</param>
	    /// <param name="maxFuel">Max fuel.</param>
	    /// <param name="playerID">Player I.</param>
		public virtual void UpdateJetpackBar(float currentFuel, float minFuel, float maxFuel,string playerID)
		{
			if (JetPackBars == null)
			{
				return;
			}

			foreach (MMProgressBar jetpackBar in JetPackBars)
	    	{
				if (jetpackBar == null) { return; }
				if (jetpackBar.PlayerID == playerID)
				{
					jetpackBar.UpdateBar(currentFuel,minFuel,maxFuel);	
		    	}    
			}
	    }

		/// <summary>
		/// Updates the (optional) ammo displays.
		/// </summary>
		/// <param name="magazineBased">If set to <c>true</c> magazine based.</param>
		/// <param name="totalAmmo">Total ammo.</param>
		/// <param name="maxAmmo">Max ammo.</param>
		/// <param name="ammoInMagazine">Ammo in magazine.</param>
		/// <param name="magazineSize">Magazine size.</param>
		/// <param name="playerID">Player I.</param>
		/// <param name="displayTotal">If set to <c>true</c> display total.</param>
		public virtual void UpdateAmmoDisplays(bool magazineBased, int totalAmmo, int maxAmmo, int ammoInMagazine, int magazineSize, string playerID, bool displayTotal)
		{
			if (AmmoDisplays == null)
			{
				return;
			}

			foreach (AmmoDisplay ammoDisplay in AmmoDisplays)
			{
				if (ammoDisplay == null) { return; }
				if (ammoDisplay.PlayerID == playerID)
				{
					ammoDisplay.UpdateAmmoDisplays (magazineBased, totalAmmo, maxAmmo, ammoInMagazine, magazineSize, displayTotal);
				}    
			}
		}
		
		/// <summary>
		/// Sets the level name in the HUD
		/// </summary>
		public virtual void SetLevelName(string name)
		{
	        if (LevelText!= null)
	        { 
	    		LevelText.text=name;
	        }
	    }

        /// <summary>
        /// When we catch a level name event, we change our level's name in the GUI
        /// </summary>
        /// <param name="levelNameEvent"></param>
        public virtual void OnMMEvent(LevelNameEvent levelNameEvent)
        {
            SetLevelName(levelNameEvent.LevelName);
        }

        public virtual void OnMMEvent(ControlsModeEvent controlsModeEvent)
        {
            SetMobileControlsActive(controlsModeEvent.Status, controlsModeEvent.MovementControl);
        }

        /// <summary>
        /// On enable, we start listening to events
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<LevelNameEvent>();
            this.MMEventStartListening<ControlsModeEvent>();
        }

        /// <summary>
        /// On disable, we stop listening to events
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<LevelNameEvent>();
            this.MMEventStopListening<ControlsModeEvent>();
        }
    }
}