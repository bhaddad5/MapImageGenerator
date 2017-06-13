Shader "Custom/GroundShader" {
	Properties {
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

		float scale = 40;

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

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_LookupTex, IN.uv_LookupTex);
			if(c.r == 1 && c.g == 1 && c.b == 0)
				c = tex2D(_FertileTex, IN.uv_LookupTex * 10);
			if (c.r == 0 && c.g == 0 && c.b == 0)
				c = tex2D(_GrassTex, IN.uv_LookupTex * 10);
			if (c.r == 0 && c.g == 0 && c.b == 1)
				c = tex2D(_SandTex, IN.uv_LookupTex * 10);
			if (c.r == 0 && Equals(c.g, 0.5098) && c.b == 0)
				c = tex2D(_ForestTex, IN.uv_LookupTex * 10);
			if (Equals(c.r, .564) && Equals(c.g, .360) && c.b == 0)
				c = tex2D(_MountainsTex, IN.uv_LookupTex * 10);
			if (c.r == 0 && Equals(c.g, 0.737) && Equals(c.b, .415))
				c = tex2D(_SwampTex, IN.uv_LookupTex * 10);

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = 1;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
