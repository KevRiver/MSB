using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class ImageExt {

	static float _anchorX    = 0;
	static float _anchorY    = 0;
	static float _boundsMinX = 0;
	static float _boundsMinY = 0;
	static float _boundsMaxX = 0;
	static float _boundsMaxY = 0;

	public static void GetActiveBounds(this Image instance, ref float minX, ref float minY, ref float maxX, ref float maxY, bool includeChildren = false, System.Func<GameObject, Sprite, bool> shouldIgnoreSprite = null) {
		if (shouldIgnoreSprite == null || !shouldIgnoreSprite (instance.gameObject, instance.sprite)) {
			_anchorX = instance.transform.position.x / instance.canvas.referencePixelsPerUnit;
			_anchorY = instance.transform.position.y / instance.canvas.referencePixelsPerUnit;

			_boundsMinX = _anchorX + instance.sprite.bounds.min.x;
			_boundsMinY = _anchorY + instance.sprite.bounds.min.y;
			_boundsMaxX = _anchorX + instance.sprite.bounds.max.x;
			_boundsMaxY = _anchorY + instance.sprite.bounds.max.y;

			if (_boundsMinX < minX) minX = _boundsMinX;
			if (_boundsMinY < minY) minY = _boundsMinY;
			if (_boundsMaxX > maxX) maxX = _boundsMaxX;
			if (_boundsMaxY > maxY) maxY = _boundsMaxY;
		}

		if (!includeChildren)
			return;

		int childCount = instance.transform.childCount;

		for (int i = 0; i < childCount; i++) {
			Transform child      = instance.transform.GetChild (i);
			Image     childImage = child.GetComponent<Image> ();

			if (childImage) {
				GetActiveBounds (childImage, ref minX, ref minY, ref maxX, ref maxY, includeChildren, shouldIgnoreSprite);
			}
		}
	}

}
