// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Outline"
{
	Properties
	{
		_Size                ("Size",                  Int)   = 1
		_BlurSize            ("Blur Size",             Int)   = 0
		_Color               ("Color",                 Color) = (1,1,1,1)
		_BlurAlphaMultiplier ("Blur Alpha Multiplier", Float) = 0.7
		_BlurAlphaChoke      ("Blur Alpha Choke",      Float) = 1
		[Toggle] _InvertBlur ("Invert Blur",           Int)   = 0
		_AlphaThreshold      ("Alpha Threshold",       Float) = 0.05
		_Buffer              ("Buffer",                Int)   = 0

		[PerRendererData] _MainTex             ("Sprite Texture",        2D)     = "white" {}
		//[HideInInspector] _Color               ("Tint",                  Color)  = (1,1,1,1)
		//[MaterialToggle]   PixelSnap           ("Pixel snap",            Float)  = 0
		//[HideInInspector] _RendererColor       ("RendererColor",         Color)  = (1,1,1,1)
		[HideInInspector] _Flip                ("Flip",                  Vector) = (1,1,1,1)
		//[PerRendererData] _AlphaTex            ("External Alpha",        2D)     = "white" {}
		//[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float)  = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"             = "Transparent"
			"IgnoreProjector"   = "True"
			"RenderType"        = "Transparent"
			"PreviewType"       = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment OutlineSpriteFrag
			//#pragma target 4.0
			//#pragma multi_compile_instancing
			//#pragma multi_compile _ PIXELSNAP_ON
			//#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			float4 _MainTex_TexelSize;
			int    _Size;
			int    _BlurSize;
			//float4 _Color;
			float  _BlurAlphaMultiplier;
			float  _BlurAlphaChoke;
			bool   _InvertBlur;
			float  _AlphaThreshold;
			int    _Buffer;

			float2 _inTexcoord;
			float4 _pixelTexcoord;
			float  _pixelAlpha;
			int    _blurThickness;
			int    _strokeThickness;
			fixed4 _clearColor;
			fixed4 _outColor;

			float Distance(int x1, int y1, int x2, int y2) {
				int deltaX = x2 - x1;
				int deltaY = y2 - y1;

				return sqrt (deltaX * deltaX + deltaY * deltaY);
			}

			float InverseLerp(float a, float b, float value) {
				return clamp ((value - a) / (b - a), 0, 1);
			}

			bool HasPixelAt(int x, int y) {
				_pixelTexcoord.x = _inTexcoord.x + (x * _MainTex_TexelSize.x);
				_pixelTexcoord.y = _inTexcoord.y + (y * _MainTex_TexelSize.y);

				_pixelAlpha = tex2Dlod (_MainTex, _pixelTexcoord).a;

				return (_pixelAlpha >= _AlphaThreshold);
			}

			bool TrySetPixelAt(int x, int y) {
				if (!HasPixelAt (x, y))
					return false;

				float pixelDistance = Distance (0, 0, x, y); // Get the distance from the current transparent pixel to the closest opaque pixel.

				pixelDistance -= _Buffer;

				if (pixelDistance <= 0 || pixelDistance > _Size)
					return true;

				float distancePercent = InverseLerp (_strokeThickness, _Size, pixelDistance);
				float chokeScalar     = 1;

				distancePercent = lerp (distancePercent, 1 - distancePercent, _InvertBlur);
				chokeScalar     = lerp (chokeScalar, distancePercent, _InvertBlur);

				float blurAlphaMultiplier = lerp (_BlurAlphaMultiplier, _BlurAlphaMultiplier / _Size, distancePercent);
				float blurAlphaChoke      = max (0.7, (pixelDistance - _strokeThickness) * _BlurAlphaChoke * chokeScalar) / 0.7;

				float alphaScalar = ceil (saturate (pixelDistance - _strokeThickness)); // Only blur pixels greater than the stroke thickness.

				_outColor.a   = lerp (_Color.a, _Color.a * blurAlphaMultiplier / blurAlphaChoke, alphaScalar);
				_outColor.rgb = _Color.rgb * _outColor.a;

				return true;
			}

			fixed4 OutlineSpriteFrag(v2f IN) : SV_Target {
				_inTexcoord = IN.texcoord;

				_blurThickness   = min (_BlurSize, _Size - 1);
				_strokeThickness = _Size - _blurThickness;

				int i, j, gridSize, dir = -1, maxGridRadius = _Size + _Buffer;

				for (int gridRadius = 1; gridRadius <= maxGridRadius; gridRadius++)
				{
					gridSize = gridRadius * 2;
					j        = 0;

					for (i = 0; i < gridSize; i++)
					{
						j   +=  i * dir;
						dir *= -1;

						if (TrySetPixelAt (0, 0)) // Do not fill any color for pixels that overlap the sprite(s).
							return _clearColor;

						if (TrySetPixelAt (-gridRadius, j) || TrySetPixelAt (j, gridRadius) || TrySetPixelAt (gridRadius, -j) || TrySetPixelAt (-j, -gridRadius))
							return _outColor;
					}
				}

				return _outColor;
			}
		ENDCG
		}
	}
}
