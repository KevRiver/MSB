using UnityEngine;
using UnityEditor;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{

	[CustomEditor (typeof(LevelManager))]
	[InitializeOnLoad]

	/// <summary>
	/// Adds custom labels to the CorgiController inspector
	/// </summary>

	public class LevelManagerEditor : Editor 
	{
		[DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
		static void DrawGameObjectName(LevelManager levelManager, GizmoType gizmoType)
		{   
			GUIStyle style = new GUIStyle();
			Vector3 v3FrontTopLeft;

			if (levelManager.LevelBounds.size!=Vector3.zero)
			{
		        style.normal.textColor = Color.yellow;		 
				v3FrontTopLeft = new Vector3(levelManager.LevelBounds.center.x - levelManager.LevelBounds.extents.x, levelManager.LevelBounds.center.y + levelManager.LevelBounds.extents.y + 1, levelManager.LevelBounds.center.z - levelManager.LevelBounds.extents.z);  // Front top left corner
				Handles.Label(v3FrontTopLeft, "Level Bounds", style);
				MMDebug.DrawHandlesBounds(levelManager.LevelBounds,Color.yellow);
			}
		}
	}
}