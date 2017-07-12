Shader "Custom/BordersShader" {
	Properties {
		_LookupWidth("Texture Size", Float) = 56
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		float _LookupWidth;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		float GetAlpha(float relevantAxis, float2 lookupOffset, float LookupPixelWidth, float2 uvIn, fixed4 c)
		{
			fixed4 cNeighbor = tex2D(_MainTex, uvIn + lookupOffset);
			if ((c.r != cNeighbor.r || c.g != cNeighbor.g || c.b != cNeighbor.b) &&
				!(cNeighbor.r == 0 && cNeighbor.g == 0 && cNeighbor.b == 1) &&
				!(c.r == 0 && c.g == 0 && c.b == 1) &&
				!(c.r == 0 && c.g == 0 && c.b == 0))
				return max(0, relevantAxis / (LookupPixelWidth / 2));
			return 0;
		}

		float GetCornerAlpha(float2 center, float2 cornerOffset, float2 lookupTexturePos, float LookupPixelWidth, float2 uvIn, fixed4 c)
		{
			float2 corner = center + cornerOffset;
			float distFromCorner = length(uvIn - corner);
			return GetAlpha(LookupPixelWidth / 2 - distFromCorner, lookupTexturePos, LookupPixelWidth, uvIn, c);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

			float LookupPixelWidth = (1 / _LookupWidth);

			float2 offsetFromLookupPos = float2(fmod(IN.uv_MainTex.x, LookupPixelWidth), fmod(IN.uv_MainTex.y, LookupPixelWidth));
			float2 center = IN.uv_MainTex - offsetFromLookupPos + float2(LookupPixelWidth / 2, LookupPixelWidth / 2);
			float2 offsetFromCenter = IN.uv_MainTex - center;

			float alpha = 0;

			float rightAlpha = GetAlpha(offsetFromCenter.x, float2(LookupPixelWidth, 0), LookupPixelWidth, IN.uv_MainTex, c);
			float leftAlpha = GetAlpha(-offsetFromCenter.x, float2(-LookupPixelWidth, 0), LookupPixelWidth, IN.uv_MainTex, c);
			float topAlpha = GetAlpha(offsetFromCenter.y, float2(0, LookupPixelWidth), LookupPixelWidth, IN.uv_MainTex, c);
			float bottomAlpha = GetAlpha(-offsetFromCenter.y, float2(0, -LookupPixelWidth), LookupPixelWidth, IN.uv_MainTex, c);

			alpha = max(max(max(rightAlpha, leftAlpha), topAlpha), bottomAlpha);

			if (offsetFromCenter.x > 0 && offsetFromCenter.y > 0 && topAlpha == 0 && rightAlpha == 0)
			{
				alpha += GetCornerAlpha(center, float2(LookupPixelWidth / 2, LookupPixelWidth / 2), float2(LookupPixelWidth, LookupPixelWidth), LookupPixelWidth, IN.uv_MainTex, c);
			}
			if (offsetFromCenter.x > 0 && offsetFromCenter.y < 0 && bottomAlpha == 0 && rightAlpha == 0)
			{
				alpha += GetCornerAlpha(center, float2(LookupPixelWidth / 2, -LookupPixelWidth / 2), float2(LookupPixelWidth, -LookupPixelWidth), LookupPixelWidth, IN.uv_MainTex, c);
			}
			if (offsetFromCenter.x < 0 && offsetFromCenter.y > 0 && topAlpha == 0 && leftAlpha == 0)
			{
				alpha += GetCornerAlpha(center, float2(-LookupPixelWidth / 2, LookupPixelWidth / 2), float2(-LookupPixelWidth, LookupPixelWidth), LookupPixelWidth, IN.uv_MainTex, c);
			}
			if (offsetFromCenter.x < 0 && offsetFromCenter.y < 0 && bottomAlpha == 0 && leftAlpha == 0)
			{
				alpha += GetCornerAlpha(center, float2(-LookupPixelWidth / 2, -LookupPixelWidth / 2), float2(-LookupPixelWidth, -LookupPixelWidth), LookupPixelWidth, IN.uv_MainTex, c);
			}
			
			
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = alpha/2;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
