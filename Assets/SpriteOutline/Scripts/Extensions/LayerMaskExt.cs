using UnityEngine;
using System.Collections;

public static class LayerMaskExt {

	public static bool ContainsLayer(this LayerMask layers, int layer) {
		return layers == (layers | (1 << layer));
	}

}
