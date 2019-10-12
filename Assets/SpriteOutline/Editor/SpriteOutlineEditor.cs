using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpriteOutline))]
public class SpriteOutlineEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUIStyle hr = new GUIStyle (GUI.skin.box);

		hr.border.top    = 0;
		hr.border.bottom = 0;
		hr.margin.top    = 0;
		hr.margin.bottom = 8;
		hr.stretchWidth  = true;
		hr.fixedHeight   = 1;

		Color originalColor = GUI.color;

		GUI.color = Color.black;
		GUILayout.Box ("", hr);
		GUI.color = originalColor;

		GUILayout.Label ("Editor Actions:");

		if (GUILayout.Button ("Regenerate")) {
			System.Array.ForEach (targets, target => {
				SpriteOutline outline = (SpriteOutline)target;
				outline.Regenerate ();
			});
		}

		if (GUILayout.Button ("Export")) {
			System.Array.ForEach (targets, target => {
				SpriteOutline outline = (SpriteOutline)target;
				outline.Export ();
			});
		}

		if (GUILayout.Button ("Clear")) {
			System.Array.ForEach (targets, target => {
				SpriteOutline outline = (SpriteOutline)target;
				outline.Clear ();
			});
		}

		EditorGUILayout.Space ();
	}

}
