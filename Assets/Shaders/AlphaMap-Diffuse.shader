
Shader "Custom/AlphaMap-Diffuse" {
	
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaTex ("Alpha Map", 2D) = "white" {}
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha:fade

		sampler2D _MainTex;
		sampler2D _AlphaTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 a = tex2D(_AlphaTex, IN.uv_MainTex);

			o.Albedo = c.rgb;
			o.Alpha = a.r;
		}
		ENDCG
	}
	Fallback "Legacy Shaders/Transparent/VertexLit"
}
