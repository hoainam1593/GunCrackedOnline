
Shader "Custom/Hidden" {

	Properties {
		_HiddenColor ("Hidden Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags { "Queue"="Geometry+500" }

		Pass {
			Name "BASE"

			ZWrite Off
			ZTest Greater
			Lighting Off
			Color [_HiddenColor]
		}
	}

}
