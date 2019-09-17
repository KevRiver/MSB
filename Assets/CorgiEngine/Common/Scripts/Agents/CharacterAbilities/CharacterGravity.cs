using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this class to a character and it'll be able to have a different gravity than the default one, and will be able to change it via Gravity Zones and Gravity Points
	/// Animator parameters : none
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Character Gravity")] 
	public class CharacterGravity : CharacterAbility 
	{
		/// the various ways you can modify the existing character forces when entering or exiting a gravity zone. Reset will set them at 0, Nothing will do nothing (use at your own risk), and Adapt will rotate the force to match the new gravity
		public enum TransitionForcesModes { Reset, Adapt, Nothing }

		public override string HelpBoxText() { return "This component allows your character to have a different gravity than the default, vertical one. It will also allow it to be affected by gravity zones and points, that will override its default gravity settings."; }

		[Header("Influence")]
		[Information("Here you can decide whether or not this character is subject to Gravity Points and Gravity Zones, or if it should ignore them.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// If this is true, this Character's gravity will be overridden when close to Gravity Points
		public bool SubjectToGravityPoints = true;
		/// If this is true, this Character will have its gravity affected when crossing Gravity Zones
		public bool SubjectToGravityZones = true;

		[Header("Gravity")]
		[Information("Here you can define the initial gravity angle, and whether or not horizontal and/or vertical input should be reversed when upside down.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// if this is true, horizontal movement input will be reversed when upside down
		public bool ReverseHorizontalInputWhenUpsideDown = false;
		/// if this is true, vertical movement input will be reversed when upside down
		public bool ReverseVerticalInputWhenUpsideDown = false;
        /// if this is false, input won't be reversed while on a gravity point (you'll usually want to keep it false so you can run around the point
        public bool ReverseInputOnGravityPoints = false;
		[Range(-180,180)]
		/// the initial gravity angle, that's the gravity angle your character will start at
		public float InitialGravityAngle = 0f;

		[Header("[Experimental] Rotation")]
		[Information("Here you can set a rotation speed that will be applied to your character when changing zones. A speed of 0 means instant rotation. Be careful with that setting, as it'll gradually change the gravity direction of your character, which may not be what you want. You can also specify the Inactive Buffer Duration, which is the time during which a zone is disabled on entry/exit to accomodate for the rotation's duration. As a general rule, slow speeds will require higher inactive buffer duration, so the lower the speed, the higher the inactive buffer duration should be. A rotation speed of 1.6 and a buffer duration of 0.3 usually works well, but feel free to play with these settings.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the speed at which the Character rotates to match its new gravity's rotation. 0 means instant.
		public float RotationSpeed = 0f;
		/// the duration (in seconds) during which a zone is ignored when entered/exited, right after the enter/exit, to accomodate for rotation times. If you have a slow rotation speed, increase this.
		public float InactiveBufferDuration = 0.1f; 

		[Header("Transition")]
		[Information("Here you can define a transition mode that will impact the way your character enters or exits gravity altered zones. Reset will kill all current movement and state, which is the safest option. Adapt will keep your force going and rotate it, and Nothing will not affect existing forces, which may have weird consequences depending on the context.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// if this is set to true, forces will be reset when entering/exiting gravity zones.
		public TransitionForcesModes TransitionForcesMode = TransitionForcesModes.Reset;
        /// if this is true, the character state will be reset to idle when entering the zone
        public bool ResetCharacterStateOnGravityChange = true;

		[Header("Debug")]
		[Information("Here you can chose to have the Ability draw the current gravity target angle.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// If this is true, will draw an arrow showing the current target gravity direction
		public bool DrawDebugGravityArrow = true;

		/// the current target gravity angle
		public float GravityAngle { get{ return _gravityOverridden ? _overrideGravityAngle : _defaultGravityAngle; } }
		/// the current target gravity vector
		public Vector2 GravityDirectionVector { get { return MMMaths.RotateVector2 (Vector2.down, GravityAngle);	}}

		protected List<GravityPoint> _gravityPoints;
		protected GravityPoint _closestGravityPoint = null;
		protected Vector2 _gravityPointDirection = Vector2.zero;

		protected bool _inAGravityZone = false;
		protected GravityZone _currentGravityZone = null;

		protected float _defaultGravityAngle = 0f;
		protected float _currentGravityAngle;
		protected float _overrideGravityAngle = 0f;

		protected bool _gravityOverridden = false;

		protected float _rotationDirection = 0f;
		protected const float _rotationSpeedMultiplier = 1000f;

		protected Vector3 _newRotationAngle = Vector3.zero;

		protected float _entryTimeStampZones = 0f;
		protected float _entryTimeStampPoints = 0f;
		protected GravityPoint _lastGravityPoint = null;
		protected GravityPoint _newGravityPoint = null;

		protected float _previousGravityAngle;
		protected GravityZone _cachedGravityZone = null;


		/// <summary>
		/// On Initialization, we rotate our character to match the initial gravity angle and store it
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization ();

			// we rotate our character based on our InitialGravityAngle
			_newRotationAngle.z = InitialGravityAngle;
			transform.localEulerAngles = _newRotationAngle;


			// we override our default gravity angle with the initial one and cache it
			_defaultGravityAngle = InitialGravityAngle;
			_currentGravityAngle = _defaultGravityAngle;
			_previousGravityAngle = _defaultGravityAngle;

			_gravityPoints = new List<GravityPoint> ();
			UpdateGravityPointsList ();
		}

		/// <summary>
		/// On process we draw debug and rotate our character if needed
		/// </summary>
		public virtual void Update()
		{
			DrawGravityDebug ();
			CleanGravityZones ();
			ComputeGravityPoints ();
			UpdateGravity ();
		}

		/// <summary>
		/// Safety measures to make sure we aren't applying the wrong gravity at any given time
		/// </summary>
		protected virtual void CleanGravityZones()
		{
			if (Time.time - _entryTimeStampPoints < InactiveBufferDuration) 
			{
				return;
			}
			if ((_inAGravityZone) && (_currentGravityZone == null))
			{
				ExitGravityZone (null);
			}
			if ((!_inAGravityZone) && (_currentGravityZone != null))
			{
				SetGravityZone (_currentGravityZone);
			}
		}

		/// <summary>
		/// Finds the closest gravity point and changes the gravity if needed
		/// </summary>
		protected virtual void ComputeGravityPoints ()
		{
			// if we're not affected by gravity points, we do nothing and exit
			if (!SubjectToGravityPoints) { return; }
			// if we're in a gravity zone, we do nothing and exit
			if (_inAGravityZone) { return; }

			// we grab the closest gravity point
			_closestGravityPoint = GetClosestGravityPoint ();

			// if it's not null
			if (_closestGravityPoint != null)
			{
				// our new gravity point becomes the closest if we didn't have one already, otherwise we stay on the last gravity point met for now
				_newGravityPoint = (_lastGravityPoint == null) ? _closestGravityPoint : _lastGravityPoint;
				// if we've got a new gravity point
				if ((_lastGravityPoint != _closestGravityPoint) && (_lastGravityPoint != null))
				{
					// if we haven't entered a new gravity point in a while, we switch to that new gravity point
					if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration) 
					{
						_entryTimeStampPoints = Time.time;
						_newGravityPoint = _closestGravityPoint;
						Transition(true, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
						StartRotating ();
					}
				}
				// if we didn't have a gravity point last time, we switch to this new one
				if (_lastGravityPoint == null)
				{
					if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
					{
						_entryTimeStampPoints = Time.time;
						_newGravityPoint = _closestGravityPoint;
						Transition(true, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
						StartRotating ();
					}
				}
				// we override our gravity
				_gravityPointDirection = _newGravityPoint.transform.position - _controller.ColliderCenterPosition;
				float gravityAngle = 180 - MMMaths.AngleBetween (Vector2.up, _gravityPointDirection);
				_gravityOverridden = true;
				_overrideGravityAngle = gravityAngle;
				_lastGravityPoint = _newGravityPoint;
			}
			else			
			{
				// if we don't have a gravity point in range, our gravity is not overridden
				if (Time.time - _entryTimeStampPoints >= InactiveBufferDuration)
				{
					if (_lastGravityPoint != null)
					{
						Transition(false, _newGravityPoint.transform.position - _controller.ColliderCenterPosition);
						StartRotating ();
					}
					_entryTimeStampPoints = Time.time;
					_gravityOverridden = false;
					_lastGravityPoint = null;
				}
			}
		}

		/// <summary>
		/// Gets the closest gravity point out of all the ones stored in the GravityPoints array
		/// </summary>
		/// <returns>The closest gravity point.</returns>
		protected virtual GravityPoint GetClosestGravityPoint()
		{	
			if (_gravityPoints.Count == 0)
			{
				return null;
			}
			
			GravityPoint closestGravityPoint = null;
			float closestDistanceSqr = Mathf.Infinity;
			Vector3 currentPosition = _controller.ColliderCenterPosition;

			foreach(GravityPoint gravityPoint in _gravityPoints)
			{
				Vector3 directionToTarget = gravityPoint.transform.position - currentPosition;
				float dSqrToTarget = directionToTarget.sqrMagnitude;

				// if we're outside of this point's zone of effect, we do nothing and exit
				if (directionToTarget.magnitude > gravityPoint.DistanceOfEffect)
				{
					continue;
				}

				if(dSqrToTarget < closestDistanceSqr)
				{
					closestDistanceSqr = dSqrToTarget;
					closestGravityPoint = gravityPoint;
				}
			}

			return closestGravityPoint;
		}

		/// <summary>
		/// Rotates the Character to match the current target gravity angle
		/// </summary>
		protected virtual void UpdateGravity()
		{
			if (RotationSpeed == 0)
			{
				_currentGravityAngle = GravityAngle;
			}
			else
			{
				float gravityAngle = GravityAngle;
				// if there's a 180° difference between both angles, we force the rotation angle depending on the direction of the character
				if (Mathf.DeltaAngle(_currentGravityAngle,gravityAngle) == 180)
				{
					
						_currentGravityAngle = _currentGravityAngle % 360;


						if (_rotationDirection > 0) 
						{
							_currentGravityAngle += 0.1f;
						}
						else
						{
							_currentGravityAngle -= 0.1f;							
						}				
				}

				if (Mathf.DeltaAngle(_currentGravityAngle,gravityAngle) > 0)
				{
					if(Mathf.Abs(Mathf.DeltaAngle(_currentGravityAngle,gravityAngle)) < Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier)
					{
						_currentGravityAngle = gravityAngle;
					}
					else
					{
						_currentGravityAngle += Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier;	
					}
				}
				else
				{					
					if(Mathf.Abs(Mathf.DeltaAngle(_currentGravityAngle,gravityAngle)) < Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier)
					{
						_currentGravityAngle = gravityAngle;
					}
					else
					{
						_currentGravityAngle -= Time.deltaTime * RotationSpeed * _rotationSpeedMultiplier;		
					}
				}

			}
			_newRotationAngle.z = _currentGravityAngle;
			transform.localEulerAngles = _newRotationAngle;			
		}

		/// <summary>
		/// Called at Init, this grabs all the Gravity Points in the scene and stores them. 
		/// You can also call this method if you decide to add gravity points at runtime to refresh the list (they'll be ignored otherwise).
		/// </summary>
		public virtual void UpdateGravityPointsList ()
		{
			if (_gravityPoints.Count != 0)
			{
				_gravityPoints.Clear ();
			}

			_gravityPoints.AddRange(FindObjectsOfType (typeof(GravityPoint)) as GravityPoint[]);
		}

		/// <summary>
		/// When entering a zone, checks if it's a Gravity Zone, and if yes, sets gravity accordingly
		/// </summary>
		/// <param name="collider">Collider.</param>
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{			
			// if this is not a gravity zone, we do nothing and exit
			GravityZone gravityZone = collider.gameObject.GetComponentNoAlloc<GravityZone> ();
			if ((gravityZone == null) || !SubjectToGravityZones) { return; }

			// if we've entered another zone before exiting the one we are in, we cache the previous one to prevent glitches later
			if (_currentGravityZone != null && _currentGravityZone != gravityZone)
			{
				_cachedGravityZone = _currentGravityZone;
			}

			// we store our new gravity zone
			_currentGravityZone = gravityZone;

			// if we're over our inactive buffer, we set it
			if (Time.time - _entryTimeStampZones > InactiveBufferDuration) 
			{	
				SetGravityZone(gravityZone);	
			}
		}

		/// <summary>
		/// When exiting a zone, checks if it was a Gravity Zone, and if yes, resets gravity accordingly
		/// </summary>
		/// <param name="collider">Collider.</param>
		protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			// if this is not a gravity zone, we do nothing and exit
			GravityZone gravityZone = collider.gameObject.GetComponentNoAlloc<GravityZone> ();
			if ((gravityZone == null) || !SubjectToGravityZones) { return; }

			// if the zone we are leaving is the one we had cached, we reset our stored grav zone
			if (gravityZone == _cachedGravityZone)
			{
				_cachedGravityZone = null;
			}

			// if the zone we are leaving is the current active one or if we have one in cache, we don't trigger exit for this zone or apply the cached gravity
			if (_currentGravityZone != gravityZone || _cachedGravityZone != null)
			{
				if (_cachedGravityZone != null)
				{
					_currentGravityZone = _cachedGravityZone;
					if (Time.time - _entryTimeStampZones > InactiveBufferDuration) 
					{							
						SetGravityZone(_currentGravityZone);	
					}
				}
				return;
			}

			// we're not in a gravity zone anymore
			_currentGravityZone = null;

			// we apply our inactive buffer duration
			if (Time.time - _entryTimeStampZones > InactiveBufferDuration) 
			{	
				ExitGravityZone (gravityZone);	
			}
		}

		/// <summary>
		/// Sets the specified gravity zone as the current one, applying its gravity properties to our character
		/// </summary>
		/// <param name="gravityZone">Gravity zone.</param>
		protected virtual void SetGravityZone(GravityZone gravityZone)
		{
			// we apply our inactive buffer duration
			_entryTimeStampZones = Time.time;

			// we override our gravity
			_gravityOverridden = true;
			_overrideGravityAngle = gravityZone.GravityDirectionAngle;
			_inAGravityZone = true;

			StartRotating ();

			// we transition our character's state
			Transition(true, gravityZone.GravityDirectionVector);
		}

		/// <summary>
		/// When exiting a gravity zone we reset our gravity and handle transition
		/// </summary>
		/// <param name="gravityZone">Gravity zone.</param>
		protected virtual void ExitGravityZone(GravityZone gravityZone)
		{
			_entryTimeStampZones = Time.time;

			// we reset our gravity
			_gravityOverridden = false;
			_inAGravityZone = false;

			StartRotating ();

			// we transition our character's state
			if (gravityZone)
			{
				Transition (false, gravityZone.GravityDirectionVector);
			}				
		}

		/// <summary>
		/// Triggers the need to rotate the character
		/// </summary>
		protected virtual void StartRotating()
		{
			_rotationDirection = _character.IsFacingRight ? 1 : -1;
		}

		/// <summary>
		/// Handles the existing forces on a character when entering/exiting a zone
		/// </summary>
		/// <param name="entering">If set to <c>true</c> entering.</param>
		/// <param name="gravityDirection">Gravity direction.</param>
		protected virtual void Transition(bool entering, Vector2 gravityDirection)
		{
			float gravityAngle = 180 - MMMaths.AngleBetween (Vector2.up, gravityDirection);
			if (TransitionForcesMode == TransitionForcesModes.Nothing)
			{
				return;
			}
			if (TransitionForcesMode == TransitionForcesModes.Reset)
			{
				_controller.SetForce (Vector2.zero);	
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}
			if (TransitionForcesMode == TransitionForcesModes.Adapt)
			{
				// the angle is calculated depending on if the player enters or exits a zone and takes _previousGravityAngle as parameter if you glide over from one zone to another
				float rotationAngle = entering ? _previousGravityAngle - gravityAngle : gravityAngle - _defaultGravityAngle;

				_controller.SetForce(Quaternion.Euler(0, 0, rotationAngle) * _controller.Speed);
			}
            if (ResetCharacterStateOnGravityChange)
            {
                if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
                {
                    _character.gameObject.GetComponentNoAlloc<CharacterDash>().StopDash();
                }
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }
			_previousGravityAngle = entering ? gravityAngle : _defaultGravityAngle;
		}

		/// <summary>
		/// Returns true if conditions to reverse input are met
		/// </summary>
		/// <returns><c>true</c>, if reverse input was shoulded, <c>false</c> otherwise.</returns>
		public virtual bool ShouldReverseInput()
		{
			bool reverseInput = false;

            if (!ReverseInputOnGravityPoints && (_closestGravityPoint != null))
            {
                return false;
            }

			if (!ReverseHorizontalInputWhenUpsideDown)
            {
                return reverseInput;
            }

			reverseInput = ((GravityAngle < -90) || (GravityAngle > 90));

			return reverseInput;
		}

		/// <summary>
		/// Sets the default gravity angle to the one specified in parameters
		/// </summary>
		/// <param name="newAngle">New angle.</param>
		public virtual void SetGravityAngle(float newAngle)
		{
			_defaultGravityAngle = newAngle;
		}

		/// <summary>
		/// Resets the gravity to its default value.
		/// </summary>
		public virtual void ResetGravityToDefault()
		{
			_gravityOverridden = false;
		}

        /// <summary>
        /// This gets called when the player dies
        /// </summary>
        protected override void OnRespawn()
        {
            base.OnRespawn();
            ResetGravityToDefault();
        }

        /// <summary>
        /// If authorized to, draws a debug arrow showing the direction of the current target gravity
        /// </summary>
        protected virtual void DrawGravityDebug()
		{
			if (!DrawDebugGravityArrow)
			{
				return;
			}
			MMDebug.DebugDrawArrow(_controller.ColliderCenterPosition, GravityDirectionVector, MoreMountains.Tools.Colors.Cyan, _controller.Height() * 1.5f, 0.2f, 35f);
		}
	}
}
