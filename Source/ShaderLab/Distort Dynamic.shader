Shader "Waterfall/Distortion (Dynamic)"
{
	Properties 
	{
		_DistortionTex("DistortionTex (RGBA)", 2D) = "gray" {}

		_Falloff("Falloff", Range(0,10)) = 0
		_Fresnel("Fresnel", Range(0,10)) = 0
		_FresnelInvert("Fresnel Invert", Range(0,10)) = 0
		_dirAdjust("Adjust for Exhaust Direction", Range(0,1)) = 1
		_PlumeDir("Exhaust Direction", Vector) = (0,1,0,0)
		
		_Strength("Strength", Range(0,5)) = 1
		_Highlight("Highlight", Range(0,1)) = 0

		_Blur("Blur", Range(0,5)) = 1
		_Swirl("Swirl", Range(0,5)) = 1


		[Space]
		_FadeIn("Fade In", Range(0,1)) = 0
		_FadeOut("Fade Out", Range(0,1)) = 0

		[Space]

		_ExpandOffset("Offset", Range(-5, 5)) = 0
		_ExpandLinear("Linear Expansion", Range(-10, 10)) = 0
		_ExpandSquare("Quadratic Expansion", Range(-10, 10)) = 0
		_ExpandBounded("Bounded Expansion", Range(-10, 10)) = 0

		[Space]
		_FalloffStart("Falloff Start", Range(0, 1)) = 0
		
		[Space]
		_SpeedX("Scroll Speed X", Float) = 0
		_SpeedY("Scroll Speed Y", Float) = 1
		[Space]

		_TileX("Tiling X", Float) = 1
		_TileY("Tiling Y", Float) = 1

	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _BackgroundTexture;
	float4 _BackgroundTexture_TexelSize;
	sampler2D _DistortionTex;

	float4 _PlumeDir;
	float _ExpandOffset;
	float _ExpandLinear;
	float _ExpandSquare;
	float _ExpandBounded;

	float _Fresnel;
	float _dirAdjust;
	float _Strength;
	float _FresnelInvert;
	float _Falloff;
	float _Highlight;

	float _Blur;
	float _Swirl;

	float _FadeIn;
	float _FadeOut;

	float _FalloffStart;

	float _SpeedX;
	float _SpeedY;
	float _TileX;
	float _TileY;

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal: NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 screenUV : TEXCOORD1;
		float plumePos : TEXCOORD2;
		float3 worldPos : TEXCOORD3;
		float3 worldNormal : TEXCOORD4;
		float3 viewDir : TEXCOORD5;
	};

	float lin(float x) { return x; }
	float linDeriv(float x) { return 1; }
	
	float sqr(float x) { return x * x; }
	float sqrDeriv(float x) { return 2 * x; }

	float bounded(float x) { return 1 - exp(-3 * x); }
	float boundedDeriv(float x) { return 3 * exp(-3 * x); }

	float dynamicTransform(inout float4 vertex, inout float3 normal)
	{
		float3 axis = normalize(_PlumeDir);
		float arg = -dot(vertex, axis);
		// vertex displacement along original normal
		float value = _ExpandOffset
		+ _ExpandLinear * lin(arg) 
		+ _ExpandSquare * sqr(arg) 
		+ _ExpandBounded * bounded(arg);
		vertex.xyz += normal * value;

		// new normal calculation
		float deriv = _ExpandLinear * linDeriv(arg) 
		+ _ExpandSquare * sqrDeriv(arg) 
		+ _ExpandBounded * boundedDeriv(arg);
		normal = normalize(normal + deriv * axis);

		return arg;
	}

	v2f vert(appdata v)
	{
		float plumePos = dynamicTransform(v.vertex, v.normal);

		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.screenUV = ComputeGrabScreenPos(o.vertex);
		o.uv = v.uv;
		o.plumePos = plumePos;
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
		o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
		o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// all of this is quite similar to the plume shader
		fixed2 scrollUV = i.uv;
		fixed xScrollValue = _SpeedX * _Time.x;
		fixed yScrollValue = _SpeedY * _Time.x;
		scrollUV += fixed2(xScrollValue, yScrollValue);
		float noise = tex2D(_DistortionTex, scrollUV * float2(_TileX,_TileY)).rgb;

		float3 plumeDir = normalize(mul(unity_ObjectToWorld, _PlumeDir));
		float3 plumeFlow = normalize(cross(cross(plumeDir, i.worldNormal), i.worldNormal));
		float3 view = normalize(cross(cross(i.viewDir, plumeFlow), plumeFlow));

		half viewdot = abs(dot(i.worldNormal, view));
		half viewdotAlt = abs(dot(i.worldNormal, i.viewDir));
		
		half rim = smoothstep(0, 1, saturate(viewdot)); // fresnel effect
		half rim2 = clamp(1-rim, 0.001, 10); // inverted fresnel effect

		float g = min(1, (1 + _FalloffStart) * i.uv.g);
		float fade = pow(g,_Falloff); // opacity gradient

		fade *= smoothstep(0, _FadeIn, i.plumePos);
		float fOut = _FadeOut + 0.0001;
		fade *= max(0, saturate(viewdot) - max(0, (fOut + i.plumePos - 1) / fOut));
				
		// combine everything (similar to plume shader)
		float strength = pow(rim, _Fresnel)* pow(rim2, clamp(_FresnelInvert,0.001,1)) * fade * _Strength;


		// calculate refraction (I didn't put much thought into this, but it looks ok)
		float angle = 6 * (_Swirl * noise + 0*i.plumePos) + 4*_Time.y;
		float2 unit = float2(sin(angle), cos(angle));
		//float2 unit = normalize(float2(noise-0.5,0.5));
		float2 offset = unit * (noise - 0.5) * strength;
		i.screenUV.xy += offset;
		
		// blur background. replace with tex2Dproj(_BackgroundTexture, i.screenUV) to disable
		half4 background = half4(0,0,0,0);
		float radius = saturate(strength);
		float sumWeights = 0.0;
		float sigma = 0.5 / (_Blur + 0.0001);
		for (int j = -2; j <= 2; j++) 
		{
			for (int k = -2; k <= 2; k++) 
			{
				float weight = exp(-(j*j + k*k)*sigma);
				sumWeights += weight;

				float4 blurOffset = _BackgroundTexture_TexelSize * radius * i.screenUV.w * float4(j, k, 0, 0);
				background += weight * tex2Dproj(_BackgroundTexture, i.screenUV + blurOffset);
			}
		}
		background /= sumWeights;

		// Blend with highlight color
		return (1.0 - _Highlight + _Highlight * (1.0 - strength)) * background 
		+ _Highlight * strength * float4(1,0,0,1);
	}

	ENDCG

	SubShader
	{
		Tags{ "Queue" = "Transparent+2" "IgnoreProjector" = "True" }

		GrabPass
		{
			"_BackgroundTexture"
		}

		Pass
		{
			ZWrite Off
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
		Pass
		{
			ZWrite Off
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			ENDCG
		}
	}
	
	
	Fallback "KSP/Particles/Additive"
		
}