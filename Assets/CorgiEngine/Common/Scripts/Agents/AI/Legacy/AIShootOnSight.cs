using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to a CorgiController2D and it will try to kill your player on sight.
	/// </summary>
	[RequireComponent(typeof(CharacterHandleWeapon))]
	[AddComponentMenu("Corgi Engine/Character/AI/AI Shoot on Sight")] 
	public class AIShootOnSight : MonoBehaviour 
	{
		[Header("Behaviour")]
		[Information("Add this component to a CorgiController2D and it will try to kill your player on sight. This component requires a CharacterShoot component, and will simply tell it to press the trigger whenever a Player crosses its sight.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// The maximum distance at which the AI can shoot at the player
		public float ShootDistance = 10f;
		/// The offset to apply to the shoot origin point (by default the position of the object)
		public Vector2 RaycastOriginOffset = new Vector2(0,0);
		/// The layers the agent will try to shoot at
		public LayerMask TargetLayerMask;

		// private stuff
	    protected Vector2 _direction;
	    protected Character _character;
		protected CharacterHandleWeapon _characterShoot;
		protected Vector2 _raycastOrigin;
		protected RaycastHit2D _raycast;

		/// <summary>
		/// on start we get our components
		/// </summary>
		protected virtual void Start () 
		{
			_character = GetComponent<Character>();
	    	_characterShoot = GetComponent<CharacterHandleWeapon>();
		}

	    /// <summary>
		/// Every frame, check for the player and try and kill it
	    /// </summary>
	    protected virtual void Update () 
		{
			if ( (_character == null) || (_characterShoot == null) ) { return; }

			if ((_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
				|| (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen))
			{
				_characterShoot.ShootStop();
				return;
			}

			// determine the direction of the raycast 
			_direction = (_character.IsFacingRight) ? transform.right : -transform.right;
						
			// we cast a ray in front of the agent to check for a Player
			_raycastOrigin.x = _character.IsFacingRight ? transform.position.x + RaycastOriginOffset.x : transform.position.x - RaycastOriginOffset.x;
			_raycastOrigin.y = transform.position.y + RaycastOriginOffset.y;
			_raycast = MMDebug.RayCast(_raycastOrigin,_direction,ShootDistance,TargetLayerMask,Color.yellow,true);

			// if the raycast has hit something, we shoot
			if (_raycast)
			{
				_characterShoot.ShootStart();
			}
			// otherwise we stop shooting
			else
			{
				_characterShoot.ShootStop();
			}

			if (_characterShoot.CurrentWeapon != null)
			{
				if (_characterShoot.CurrentWeapon.GetComponent<WeaponAim>() != null)
				{
					Vector3 direction = LevelManager.Instance.Players [0].transform.position - this.transform.position;
					_characterShoot.CurrentWeapon.GetComponent<WeaponAim> ().SetCurrentAim (direction);
				}
			}
		}
	}
}