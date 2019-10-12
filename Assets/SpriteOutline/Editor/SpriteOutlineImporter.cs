using UnityEngine;
using UnityEditor;

public class SpriteOutlineImporter : AssetPostprocessor {

	void OnPreprocessTexture() {
		TextureImporter importer = (TextureImporter)assetImporter;

		if (!assetPath.Contains (SpriteOutline.IMAGE_EXT))
			return;

		importer.filterMode          = FilterMode.Point;
		importer.alphaIsTransparency = false;
	}

}
