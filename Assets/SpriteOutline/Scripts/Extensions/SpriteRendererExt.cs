using UnityEngine;
using System.Collections;

public static class SpriteRendererExt {

	public static void GetActiveBounds(this SpriteRenderer instance, ref float minX, ref float minY, ref float maxX, ref float maxY, bool includeChildren = false, System.Func<GameObject, Sprite, bool> shouldIgnoreSprite = null) {
		if (shouldIgnoreSprite == null || !shouldIgnoreSprite (instance.gameObject, instance.sprite)) {
			if (instance.bounds.min.x < minX) minX = instance.bounds.min.x;
			if (instance.bounds.min.y < minY) minY = instance.bounds.min.y;
			if (instance.bounds.max.x > maxX) maxX = instance.bounds.max.x;
			if (instance.bounds.max.y > maxY) maxY = instance.bounds.max.y;
		}

		if (!includeChildren)
			return;

		int childCount = instance.transform.childCount;

		for (int i = 0; i < childCount; i++) {
			Transform      child               = instance.transform.GetChild (i);
			SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer> ();

			if (childSpriteRenderer) {
				GetActiveBounds (childSpriteRenderer, ref minX, ref minY, ref maxX, ref maxY, includeChildren, shouldIgnoreSprite);
			}
		}
	}

}
