
Shader "Custom/Hidden-Diffuse" {

	Properties {
		// Base hidden.
		_HiddenColor ("Hidden Color", Color) = (1,1,1,1)

		// Diffuse.
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags { "Queue"="Geometry+500" }

		UsePass "Custom/Hidden/BASE"

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Legacy Shaders/Self-Illumin/VertexLit"
}
