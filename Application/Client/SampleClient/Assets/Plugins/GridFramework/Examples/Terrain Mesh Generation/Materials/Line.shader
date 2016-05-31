Shader "GridFramework/TerrainExample/Line" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0)
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
		}
		Pass {
			Blend One One ZWrite Off ColorMask RGB
			Material {
				Diffuse [_Color] Ambient [_Color]
			}
			Lighting On
			SetTexture [_Dummy] {
				combine primary double, primary
			}
		}
	}
}

