using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// A list of possible events used by the character
	/// </summary>
	public enum MMCharacterEventTypes
	{
		ButtonActivation,
		Jump
	}

	/// <summary>
	/// MMCharacterEvents are used in addition to the events triggered by the character's state machine, to signal stuff happening that is not necessarily linked to a change of state
	/// </summary>
	public struct MMCharacterEvent
	{
		public Character TargetCharacter;
		public MMCharacterEventTypes EventType;
		/// <summary>
		/// Initializes a new instance of the <see cref="MoreMountains.CorgiEngine.MMCharacterEvent"/> struct.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="eventType">Event type.</param>
		public MMCharacterEvent(Character character, MMCharacterEventTypes eventType)
		{
			TargetCharacter = character;
			EventType = eventType;
		}

        static MMCharacterEvent e;
        public static void Trigger(Character character, MMCharacterEventTypes eventType)
        {
            e.TargetCharacter = character;
            e.EventType = eventType;
            MMEventManager.TriggerEvent(e);
        }
    } 

	/// <summary>
	/// An event fired when something takes damage
	/// </summary>
	public struct MMDamageTakenEvent
	{
		public Character AffectedCharacter;
		public GameObject Instigator;
		public float CurrentHealth;
		public float DamageCaused;
		public float PreviousHealth;

		/// <summary>
		/// Initializes a new instance of the <see cref="MoreMountains.CorgiEngine.MMDamageTakenEvent"/> struct.
		/// </summary>
		/// <param name="affectedCharacter">Affected character.</param>
		/// <param name="instigator">Instigator.</param>
		/// <param name="currentHealth">Current health.</param>
		/// <param name="damageCaused">Damage caused.</param>
		/// <param name="previousHealth">Previous health.</param>
		public MMDamageTakenEvent(Character affectedCharacter, GameObject instigator, float currentHealth, float damageCaused, float previousHealth)
		{
			AffectedCharacter = affectedCharacter;
			Instigator = instigator;
			CurrentHealth = currentHealth;
			DamageCaused = damageCaused;
			PreviousHealth = previousHealth;
		}

        static MMDamageTakenEvent e;
        public static void Trigger(Character affectedCharacter, GameObject instigator, float currentHealth, float damageCaused, float previousHealth)
        {
            e.AffectedCharacter = affectedCharacter;
            e.Instigator = instigator;
            e.CurrentHealth = currentHealth;
            e.DamageCaused = damageCaused;
            e.PreviousHealth = previousHealth;
            MMEventManager.TriggerEvent(e);
        }
    }
}