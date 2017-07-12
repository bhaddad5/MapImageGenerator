Shader "Custom/GroundShader" {
	Properties {
		_LookupWidth("Texture Size", Float) = 56
		_LookupTex("Lookup (RGB)", 2D) = "white" {}
		_Color0 ("Color0", Vector) = (1,1,1)
		_Tex0 ("Texture0 (RGB)", 2D) = "white" {}
		_Color1("Color1", Vector) = (1,1,1)
		_Tex1("Texture1 (RGB)", 2D) = "white" {}
		_Color2("Color2", Vector) = (1,1,1)
		_Tex2("Texture2 (RGB)", 2D) = "white" {}
		_Color3("Color3", Vector) = (1,1,1)
		_Tex3("Texture3 (RGB)", 2D) = "white" {}
		_Color4("Color4", Vector) = (1,1,1)
		_Tex4("Texture4 (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
		Blend One One
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float _LookupWidth;
		sampler2D _LookupTex;
		float3 _Color0;
		sampler2D _Tex0;
		float3 _Color1;
		sampler2D _Tex1;
		float3 _Color2;
		sampler2D _Tex2;
		float3 _Color3;
		sampler2D _Tex3;
		float3 _Color4;
		sampler2D _Tex4;

		struct Input {
			float2 uv_LookupTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		bool Equals(float f1, float f2) {
			float maxDiff = 0.01f;
			return (f1 + maxDiff > f2) && (f1 - maxDiff < f2);
		}

		bool ColorEquals(fixed4 colorToCheck, float3 lookupColor) {
			return Equals(colorToCheck.r, lookupColor.r) && Equals(colorToCheck.g, lookupColor.g) && Equals(colorToCheck.b, lookupColor.b);
		}

		fixed4 LookupColor(float2 uv, float2 lookupUv) {
			float textureScale = 10;
			fixed4 lookup = tex2D(_LookupTex, lookupUv);
			fixed4 c = fixed4(0, 0, 0, 1);
			if (ColorEquals(lookup, _Color0))
				c = tex2D(_Tex0, uv * textureScale);
			if (ColorEquals(lookup, _Color1))
				c = tex2D(_Tex1, uv * textureScale);
			if (ColorEquals(lookup, _Color2))
				c = tex2D(_Tex2, uv * textureScale);
			if (ColorEquals(lookup, _Color3))
				c = tex2D(_Tex3, uv * textureScale);
			if (ColorEquals(lookup, _Color4))
				c = tex2D(_Tex4, uv * textureScale);
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

		fixed4 ZeroOutColor(fixed4 c) {
			if (c.r == 0 && c.g == 0 && c.b == 0)
				return fixed4(0, 0, 0, 0);
			return c;
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float LookupPixelWidth = (1 / _LookupWidth);
			
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

			c1 = ZeroOutColor(c1);
			c2 = ZeroOutColor(c2);
			c3 = ZeroOutColor(c3);
			c4 = ZeroOutColor(c4);
			c5 = ZeroOutColor(c5);
			c6 = ZeroOutColor(c6);
			c7 = ZeroOutColor(c7);
			c8 = ZeroOutColor(c8);
			c9 = ZeroOutColor(c9);

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
