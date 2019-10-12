using UnityEngine;
using System.Collections;

public static class MathfExt {

	public static float Distance(float x1, float y1, float x2, float y2) {
		float deltaX = x2 - x1;
		float deltaY = y2 - y1;

		return Mathf.Sqrt (deltaX * deltaX + deltaY * deltaY);
	}

}
