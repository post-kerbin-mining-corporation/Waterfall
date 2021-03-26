Shader "Waterfall/Additive"
{	
	Properties
	{
		_MainTex("MainTex (RGBA)", 2D) = "gray" {}
		_StartTint("StartTint", Color) = (1,1,1,1)
		_EndTint("EndTint", Color) = (1,1,1,1)
		_TintFalloff("TintFalloff", Range(0,2)) = 0
		_Falloff("Falloff", Range(1,5)) = 0
		_Fresnel("Fresnel", Range(0,10)) = 0
		_FresnelInvert("Inverted Fresnel", Range(0,10)) = 0
		_Noise("Noise", Range(0,1)) = 0
		_Brightness("Brightness", Range(0,5)) = 1


		[Space]
		_Seed("Time offset",Range(-10,10)) = 1
		_SpeedX("Scroll Speed X", Float) = 0
		_SpeedY("Scroll Speed Y", Float) = 1
		[Space]

		_TileX("Tiling X", Float) = 1
		_TileY("Tiling Y", Float) = 1

	}



	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }
		//LOD 100

		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha One
		Cull OFF


		CGPROGRAM

		#include "SquadCore/LightingKSP.cginc"
		#pragma surface surf NoLighting noshadow noambient novertexlights nolightmap
		#pragma target 3.0

		sampler2D _MainTex;
		float _Fresnel;
		float4 _PlumeDir;
		float _DirAdjust;
		float _Noise;
		float _FresnelInvert;
		float4 _StartTint;
		float4 _EndTint;
		float _TintFalloff;
		float _Falloff;
		float _Brightness;

		float _Seed;
		float _SpeedX;
		float _SpeedY;
		float _TileX;
		float _TileY;

		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
			float4 color : COLOR; 
			float3 worldNormal;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed2 scrollUV = IN.uv_MainTex;
			fixed xScrollValue = _SpeedX * _Time.x+ _Seed;
			fixed yScrollValue = _SpeedY * _Time.x + _Seed;
			scrollUV += fixed2(xScrollValue, yScrollValue);

			half4 c = tex2D(_MainTex, scrollUV * float2(_TileX, _TileY));
				

			// Calc dirdot: ~ 1 when looking at the plume from the side,
			// ~ 0 when looking at it from front/back
			float dirdot = abs(dot(IN.viewDir, normalize( mul(unity_ObjectToWorld, _PlumeDir)))); // abs(dot(crs, plumeDir));

			// Use original viewdot (viewdot2) for inverted fresnel calculation because
			// I haven't figured out how to make that look good with the new viewdot...
			half viewdot2 = dot(IN.worldNormal, IN.viewDir);
			half cross = abs(dot(normalize(mul(unity_ObjectToWorld, _PlumeDir)), IN.viewDir));
			if (viewdot2 < 0)
			viewdot2 = -viewdot2;
			half viewdot =  cross ;


			half rim = smoothstep(0, 1, saturate(viewdot2));
			half rim2 = smoothstep(1, 0, saturate(viewdot2)); // inverted fresnel effect
			float fade = pow(IN.uv_MainTex.g, _Falloff); // opacity gradient
			float4 gradient = lerp(_EndTint, _StartTint, pow(fade*(rim*0.5 + 0.5), _TintFalloff)); // tint along gradient and a bit along edges

			float4 col = lerp(0.5, c, _Noise); // control texture contrast
			float4 noise = lerp(col, 1, fade); // less texture near throat

			//combine all the things: Color gradient, fresnel, opacity gradient, texture, and additional _Brightness boost
			//Fresnel strength is being modified by the texture as well, and we increase this as we ramp up the texture strength to avoid hard edges
			o.Emission = saturate( gradient * pow(rim, (1 - noise + 0.5*_Noise)*_Fresnel)* pow(rim2, (1 - noise + 0.5*_Noise)*_FresnelInvert) * fade * noise *_Brightness * IN.color.rgb);

			o.Albedo = 0;

		}

		ENDCG
	}
	Fallback "Unlit/Color"

}