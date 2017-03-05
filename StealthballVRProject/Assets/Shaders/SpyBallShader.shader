Shader "Custom/SpyBallShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags{ "Queue" = "Overlay+1"
			"RenderType" = "Transparent" }

			Pass
		{
			ZWrite Off
			ZTest Greater
			Lighting Off
			Color[_OutlineColor]
		}

			Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Less
			SetTexture[_MainTex]{ combine texture }
		}
		}
	FallBack "Diffuse"
}
