using UnityEngine;
using System.Collections;
using System;
using MoreMountains.Tools;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MoreMountains.MMInterface
{	
	/// <summary>
	/// A component to handle switches 
	/// </summary>
	public class MMSwitch : MMTouchButton 
	{
		[Header("Switch")]
		/// a SpriteReplace to represent the switch knob
		public MMSpriteReplace SwitchKnob;
		/// the possible states of the switch
		public enum SwitchStates { Left, Right }
		/// the current state of the switch
		public SwitchStates CurrentSwitchState { get; set; }
		/// the state the switch should start in
		public SwitchStates InitialState = SwitchStates.Left;

		[Header("Binding")]
		/// the methods to call when the switch is turned on
		public UnityEvent SwitchOn;
		/// the methods to call when the switch is turned off
		public UnityEvent SwitchOff;

		/// <summary>
		/// On init, we set our current switch state
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization ();
			CurrentSwitchState = InitialState;
			InitializeState ();
		}

		public virtual void InitializeState()
		{
			if (CurrentSwitchState == SwitchStates.Left)
			{
				_animator.Play ("RollLeft");
			}
			else
			{
				_animator.Play ("RollRight");
			}
		}

		/// <summary>
		/// Use this method to go from one state to the other
		/// </summary>
		public virtual void SwitchState()
		{
			if (CurrentSwitchState == SwitchStates.Left)
			{
				CurrentSwitchState = SwitchStates.Right;
				_animator.SetTrigger ("Right");
				if (SwitchOn != null)
				{
					SwitchOn.Invoke();
				}		
			}
			else
			{
				CurrentSwitchState = SwitchStates.Left;				
				_animator.SetTrigger ("Left");
				if (SwitchOff != null)
				{
					SwitchOff.Invoke();
				}	
			}
		}		
	}
}
