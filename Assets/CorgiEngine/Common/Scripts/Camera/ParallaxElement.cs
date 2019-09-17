using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{	
	[ExecuteInEditMode]
	/// <summary>
	/// Add this to a GameObject to have it move in parallax 
	/// </summary>

	// This script is based on David Dion-Paquet's great article on http://www.gamasutra.com/blogs/DavidDionPaquet/20140601/218766/Creating_a_parallax_system_in_Unity3D_is_harder_than_it_seems.php

	[AddComponentMenu("Corgi Engine/Camera/Parallax Element")]
	public class ParallaxElement : MonoBehaviour 
	{
		[Header("Behaviour")]
		[Information("This component will make this GameObject move in parallax (when the camera moves) if the camera's CameraController component has been set to move parallax elements. Here you can determine the relative horizontal and vertical speed, and in which direction the element should move.",MoreMountains.Tools.InformationAttribute.InformationType.Info,false)]

		/// horizontal speed of the layer
		public float HorizontalSpeed;
		/// vertical speed of the layer
		public float VerticalSpeed;
		/// defines if the layer moves in the same direction as the camera or not
		public bool MoveInOppositeDirection = true;

		// private stuff
		protected Vector3 _previousCameraPosition;
	    protected bool _previousMoveParallax;
	    protected ParallaxCamera _parallaxCamera;
	    protected CameraController _camera;
	    protected Transform _cameraTransform;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void OnEnable() 
		{
			if (Camera.main == null)
				return;
				
			_camera = Camera.main.GetComponent<CameraController>();
			if (_camera != null)
			{
				_parallaxCamera = _camera.GetComponent<ParallaxCamera>();
				_cameraTransform = _camera.transform;		
				_previousCameraPosition = _cameraTransform.position;	
			}
		}

	    /// <summary>
	    /// Every frame, we move the parallax layer according to the camera's position
	    /// </summary>
	    protected virtual void LateUpdate () 
		{
			if (_parallaxCamera==null)
				return;
		
			if(_parallaxCamera.MoveParallax && !_previousMoveParallax)
				_previousCameraPosition = _cameraTransform.position;

			_previousMoveParallax = _parallaxCamera.MoveParallax;

			if(!Application.isPlaying && !_parallaxCamera.MoveParallax)
				return;

			Vector3 distance = _cameraTransform.position - _previousCameraPosition;
			float direction = (MoveInOppositeDirection) ? -1f : 1f;
			transform.position += Vector3.Scale(distance, new Vector3(HorizontalSpeed, VerticalSpeed)) * direction;

			_previousCameraPosition = _cameraTransform.position;
		}
	}
}