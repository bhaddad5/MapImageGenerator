Shader "Custom/GroundShader" {
	Properties {
		_LookupSize("Texture Size", Float) = 56
		_TexSize("Texture Size", Float) = 512
		_LookupTex("Lookup (RGB)", 2D) = "white" {}
		_GrassTex ("Grass (RGB)", 2D) = "white" {}
		_SandTex("Sand (RGB)", 2D) = "white" {}
		_MountainsTex("Mountains (RGB)", 2D) = "white" {}
		_SwampTex("Swamp (RGB)", 2D) = "white" {}
		_FertileTex("Fertile (RGB)", 2D) = "white" {}
		_ForestTex("Forest (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _LookupTex;
		sampler2D _GrassTex;
		sampler2D _SandTex;
		sampler2D _MountainsTex;
		sampler2D _SwampTex;
		sampler2D _FertileTex;
		sampler2D _ForestTex;
		float _LookupSize;
		float _TexSize;

		struct Input {
			float2 uv_LookupTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		bool Equals (float f1, float f2) {
			float maxDiff = 0.001f;
			return (f1 + maxDiff > f2) && (f1 - maxDiff < f2);
		}

		fixed4 LookupColor(float2 uv, float2 lookupUv) {
			float textureScale = 10;

			fixed4 c = tex2D(_LookupTex, lookupUv);
			if (c.r == 1 && c.g == 1 && c.b == 0)
				c = tex2D(_FertileTex, uv * textureScale);
			if (c.r == 0 && c.g == 0 && c.b == 0)
				c = tex2D(_GrassTex, uv * textureScale);
			if (c.r == 0 && c.g == 0 && c.b == 1)
				c = tex2D(_SandTex, uv * textureScale);
			if (c.r == 0 && Equals(c.g, 0.5098) && c.b == 0)
				c = tex2D(_ForestTex, uv * textureScale);
			if (Equals(c.r, .564) && Equals(c.g, .360) && c.b == 0)
				c = tex2D(_MountainsTex, uv * textureScale);
			if (c.r == 0 && Equals(c.g, 0.737) && Equals(c.b, .415))
				c = tex2D(_SwampTex, uv * textureScale);
			return c;
		}

		fixed4 GetAdjustedColor(float2 uvIn, float2 lookupOffset, float LookupPixelWidth)
		{
			float2 offsetFromLookupPos = float2(fmod(uvIn.x, LookupPixelWidth), fmod(uvIn.y, LookupPixelWidth));
			float2 center = uvIn - offsetFromLookupPos + float2(LookupPixelWidth / 2, LookupPixelWidth / 2) + lookupOffset;
			float distFromCenter = length(uvIn - center);
			float cStrength = max(0, (1 - (distFromCenter / LookupPixelWidth)));
			fixed4 c = LookupColor(uvIn, uvIn + lookupOffset) * cStrength;
			return c;
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float LookupPixelWidth = (1 / _LookupSize);
			
			fixed4 c1 = GetAdjustedColor(IN.uv_LookupTex, float2(0, 0), LookupPixelWidth);
			fixed4 c2 = GetAdjustedColor(IN.uv_LookupTex, float2(LookupPixelWidth, 0), LookupPixelWidth);
			fixed4 c3 = GetAdjustedColor(IN.uv_LookupTex, float2(-LookupPixelWidth, 0), LookupPixelWidth);
			fixed4 c4 = GetAdjustedColor(IN.uv_LookupTex, float2(0, LookupPixelWidth), LookupPixelWidth);
			fixed4 c5 = GetAdjustedColor(IN.uv_LookupTex, float2(0, -LookupPixelWidth), LookupPixelWidth);
			fixed4 c6 = GetAdjustedColor(IN.uv_LookupTex, float2(LookupPixelWidth, LookupPixelWidth), LookupPixelWidth);
			fixed4 c7 = GetAdjustedColor(IN.uv_LookupTex, float2(-LookupPixelWidth, LookupPixelWidth), LookupPixelWidth);
			fixed4 c8 = GetAdjustedColor(IN.uv_LookupTex, float2(LookupPixelWidth, -LookupPixelWidth), LookupPixelWidth);
			fixed4 c9 = GetAdjustedColor(IN.uv_LookupTex, float2(-LookupPixelWidth, -LookupPixelWidth), LookupPixelWidth);

			float combinedAlpha = c1.a + c2.a + c3.a + c4.a + c5.a + c6.a + c7.a + c8.a + c9.a;

			fixed4 c = c1/ combinedAlpha + c2/ combinedAlpha + c3/ combinedAlpha + c4/ combinedAlpha + c5/ combinedAlpha + c6 / combinedAlpha + c7 / combinedAlpha + c8 / combinedAlpha + c9 / combinedAlpha;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = c.a;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
