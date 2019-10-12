using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// This class handles what happens when the player reaches the level bounds.
	/// For each bound (above, below, left, right), you can define if the player will be killed, or if its movement will be constrained, or if nothing happens
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Core/Character Level Bounds")] 
	public class CharacterLevelBounds : MonoBehaviour 
	{
		public enum BoundsBehavior 
		{
			Nothing,
			Constrain,
			Kill
		}
		[Information("Here you can define what happens when the character reaches each side of the level bounds. The level bounds are defined in the LevelManager of each scene.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// what to do to the player when it reaches the top level bound
		public BoundsBehavior Top = BoundsBehavior.Constrain;
		/// what to do to the player when it reaches the bottom level bound
		public BoundsBehavior Bottom = BoundsBehavior.Kill;
		/// what to do to the player when it reaches the left level bound
		public BoundsBehavior Left = BoundsBehavior.Constrain;
		/// what to do to the player when it reaches the right level bound
		public BoundsBehavior Right = BoundsBehavior.Constrain;

	    protected Bounds _bounds;
	    protected CorgiController _controller;
		protected Character _character;
	    protected BoxCollider2D _boxCollider;

		
		/// <summary>
		/// Initialization
		/// </summary>
		public virtual void Start () 
		{
			_character = GetComponent<Character>();
			_controller = GetComponent<CorgiController>();
			_boxCollider = GetComponent<BoxCollider2D>();
			if (LevelManager.Instance != null)
			{
				_bounds = LevelManager.Instance.LevelBounds;
			}
		}
		
		/// <summary>
		/// Every frame, we check if the player is colliding with a level bound
		/// </summary>
		public virtual void Update () 
		{
			// if the player is dead, we do nothing
			if ( (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
				|| (LevelManager.Instance == null) )
			{
				return;	
			}

			if (_bounds.size != Vector3.zero)
			{		
				// when the player reaches a bound, we apply the specified bound behavior
				if ((Top != BoundsBehavior.Nothing) && (_controller.ColliderTopPosition.y > _bounds.max.y))
				{
					ApplyBoundsBehavior(Top, new Vector2(transform.position.x,_bounds.max.y - _controller.ColliderSize.y/2));
				}
									
				if ((Bottom != BoundsBehavior.Nothing) && (_controller.ColliderBottomPosition.y < _bounds.min.y))
				{
					ApplyBoundsBehavior(Bottom, new Vector2(transform.position.x, _bounds.min.y + _controller.ColliderSize.y/2));
				}					
				
				if ((Right != BoundsBehavior.Nothing) && (_controller.ColliderRightPosition.x > _bounds.max.x))
				{
					ApplyBoundsBehavior(Right, new Vector2(_bounds.max.x - _controller.ColliderSize.x/2, transform.position.y));		
				}					
				
				if ((Left != BoundsBehavior.Nothing) && (_controller.ColliderLeftPosition.x < _bounds.min.x))
				{
					ApplyBoundsBehavior(Left, new Vector2(_bounds.min.x + _controller.ColliderSize.x/2, transform.position.y));
				}					
			}			
		}

	    /// <summary>
	    /// Applies the specified bound behavior to the player
	    /// </summary>
	    /// <param name="behavior">Behavior.</param>
	    /// <param name="constrainedPosition">Constrained position.</param>
	    protected virtual void ApplyBoundsBehavior(BoundsBehavior behavior, Vector2 constrainedPosition)
		{
			if ( (_character == null)
				|| (LevelManager.Instance == null) )
			{
				return;	
			}

			if (behavior== BoundsBehavior.Kill)
			{
				if (_character.CharacterType == Character.CharacterTypes.Player)
				{
					LevelManager.Instance.KillPlayer (_character);
				}
				else
				{
					Health health = _character.gameObject.MMGetComponentNoAlloc<Health>();
					if (health != null)
					{
						health.Kill();
					}
				}
				return;
			}	

			if (behavior == BoundsBehavior.Constrain)
			{
				transform.position = constrainedPosition;
				return;	
			}
		}
	}
}