using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// This class describes how the Corgi Engine demo achievements are triggered.
	/// It extends the base class MMAchievementRules
	/// It listens for different event types
	/// </summary>
	public class AchievementRules : MMAchievementRules, 
									MMEventListener<MMGameEvent>, 
									MMEventListener<MMCharacterEvent>, 
									MMEventListener<CorgiEngineEvent>,
									MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>,
									MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
									MMEventListener<PickableItemEvent>
	{
		/// <summary>
		/// When we catch an MMGameEvent, we do stuff based on its name
		/// </summary>
		/// <param name="gameEvent">Game event.</param>
		public override void OnMMEvent(MMGameEvent gameEvent)
		{

			base.OnMMEvent (gameEvent);

		}

		public virtual void OnMMEvent(MMCharacterEvent characterEvent)
		{
			if (characterEvent.TargetCharacter.CharacterType == Character.CharacterTypes.Player)
			{
				switch (characterEvent.EventType)
				{
					case MMCharacterEventTypes.Jump:
						MMAchievementManager.AddProgress ("JumpAround", 1);
						break;
				}	
			}
		}

		public virtual void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
		{
			switch (corgiEngineEvent.EventType)
			{
				case CorgiEngineEventTypes.LevelEnd:
					MMAchievementManager.UnlockAchievement ("PrincessInAnotherCastle");
					break;
				case CorgiEngineEventTypes.PlayerDeath:
					MMAchievementManager.UnlockAchievement ("DeathIsOnlyTheBeginning");
					break;
			}
		}

		public virtual void OnMMEvent(PickableItemEvent pickableItemEvent)
		{
			if (pickableItemEvent.PickedItem != null)
			{
				if (pickableItemEvent.PickedItem.GetComponent<Coin>() != null)
				{
					MMAchievementManager.AddProgress ("MoneyMoneyMoney", 1);
				}
				if (pickableItemEvent.PickedItem.GetComponent<Stimpack>() != null)
				{
					MMAchievementManager.UnlockAchievement ("Medic");
				}
			}
		}

		public virtual void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> movementEvent)
		{
			/*switch (movementEvent.NewState)
			{

			}*/
		}

		public virtual void OnMMEvent(MMStateChangeEvent<CharacterStates.CharacterConditions> conditionEvent)
		{
			/*switch (conditionEvent.NewState)
			{

			}*/
		}

		/// <summary>
		/// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable ();
			this.MMEventStartListening<MMCharacterEvent>();
			this.MMEventStartListening<CorgiEngineEvent>();
			this.MMEventStartListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
			this.MMEventStartListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
			this.MMEventStartListening<PickableItemEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable ();
			this.MMEventStopListening<MMCharacterEvent>();
			this.MMEventStopListening<CorgiEngineEvent>();
			this.MMEventStopListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
			this.MMEventStopListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
			this.MMEventStopListening<PickableItemEvent>();
		}
	}
}