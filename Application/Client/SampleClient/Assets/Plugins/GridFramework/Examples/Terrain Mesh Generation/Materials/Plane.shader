Shader "GridFramework/TerrainExample/Plane" {
	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha BindChannels {
				Bind "Color",color
			}
			ZWrite On Cull Front Fog {
				Mode Off
			}
		}
	}
}

