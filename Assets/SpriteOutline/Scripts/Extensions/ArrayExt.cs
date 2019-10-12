using UnityEngine;
using System.Collections;

public static class ArrayExt {

	public static bool Contains<T>(this T[] haystack, T needle) {
		int length = haystack.Length;

		for (int i = 0; i < length; i++) {
			if (haystack [i] != null && haystack [i].Equals (needle))
				return true;
		}

		return false;
	}

}
