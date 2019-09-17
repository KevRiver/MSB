using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	[AddComponentMenu("Corgi Engine/Environment/Surface Modifier")]
	/// <summary>
	/// Add this component to a platform and define its new friction or force which will be applied to any CorgiController that walks on it
	/// </summary>
	public class SurfaceModifier : MonoBehaviour 
	{
		[Header("Friction")]
		[Information("Set a friction between 0.01 and 0.99 to get a slippery surface (close to 0 is very slippery, close to 1 is less slippery).\nOr set it above 1 to get a sticky surface. The higher the value, the stickier the surface.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the amount of friction to apply to a CorgiController walking over this surface		
		public float Friction;

		[Header("Force")]
		[Information("Use these to add X or Y (or both) forces to any CorgiController that gets grounded on this surface. Adding a X force will create a treadmill (negative value > treadmill to the left, positive value > treadmill to the right). A positive y value will create a trampoline, a bouncy surface, or a jumper for example.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]
		/// the amount of force to add to a CorgiController walking over this surface
		public Vector2 AddedForce=Vector2.zero;

        protected CorgiController _controller;
        protected Character _character;

		/// <summary>
		/// Triggered when a CorgiController collides with the surface
		/// </summary>
		/// <param name="collider">Collider.</param>
		public virtual void OnTriggerStay2D(Collider2D collider)
		{
            _controller = collider.gameObject.GetComponentNoAlloc<CorgiController>();
            _character = collider.gameObject.GetComponentNoAlloc<Character>();
        }

        /// <summary>
        /// On trigger exit, we lose all reference to the controller and character
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            _controller = null;
            _character = null;
        }

        /// <summary>
        /// On Update, we make sure we have a controller and a live character, and if we do, we apply a force to it
        /// </summary>
        protected virtual void Update()
        {
            if (_controller == null)
            {
                return;
            }

            if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
            {
                _character = null;
                _controller = null;
                return;
            }

            _controller.AddHorizontalForce(AddedForce.x);
            _controller.AddVerticalForce(Mathf.Sqrt(2f * AddedForce.y * -_controller.Parameters.Gravity));
        }
	}
}