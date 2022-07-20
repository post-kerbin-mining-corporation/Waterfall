// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Waterfall/Procedural Particles (Additive)"
{
	Properties 
	{
        _StartTint("StartTint", Color) = (1,1,1,1)
		_EndTint("EndTint", Color) = (1,1,1,1)
        _Brightness("Brightness", Range(0, 5)) = 1
        _PlumeDir("Exhaust Direction", Vector) = (0,1,0,0)
        [Space]

        _Scale("Scale", Range(1, 15)) = 1
        _Expand("Expand", Range(0, 5)) = 0
        _FadeIn("Fade In", Range(0, 1)) = 0.4
        _FadeOut("Fade Out", Range(0.0, 2.5)) = 1
        //_Edge("Edge Highlight", Range(0, 1)) = 0

		_Resolution("Resolution", Range(1,5)) = 1
        _Stretch("Stretch", Range(0, 1)) = 0
        [Space]

        _DetailScale("Detail Scale", Range(1.5, 2.5)) = 2
        _DetailBrightness("Detail Brightness", Range(0, 1)) = 0.5
        _Turbulence("Turbulence", Range(0, 10)) = 1.0
        _Speed("Speed", Range(0, 100)) = 1.0

        _Exponent("Exponent", Range(0, 2)) = 1

        _Seed("Time Offset", Range(-10,10)) = 1
	}

	CGINCLUDE

	#include "UnityCG.cginc"

    float4 _StartTint;
    float4 _EndTint;
    float _Brightness;
    float4 _PlumeDir;
    float _Scale;
    float _Expand;
    float _FadeIn;
    float _FadeOut;
    //float _Edge;
    float _Resolution;
    float _Stretch;
    float _DetailScale;
    float _DetailBrightness;
    float _Turbulence;
    float _Speed;
    float _Exponent;

    float _Seed;


    // generic shader random function
    float random(in float2 st) {
        return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453123);
    }

    // Fractal Brownian Motion based on https://thebookofshaders.com/13/ 
    // and https://www.shadertoy.com/view/4dS3Wd
    float noise(in float2 st) {
        float2 index = floor(st);
        float2 fract = frac(st);

        float a = random(index);
        float b = random(index + float2(1, 0));
        float c = random(index + float2(0, 1));
        float d = random(index + float2(1, 1));

        float2 u = smoothstep(0, 1, fract);
        return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
    }

    #define OCTAVES 3
    float fbm(in float2 pos) {
        float value = 0.0;
        float ampl = _DetailBrightness;
        float2x2 rotation = float2x2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));

        for (int i = 0; i < OCTAVES; i++) {
            value += ampl * noise(pos);
            ampl *= _DetailBrightness;
            pos = mul(rotation, pos) * _DetailScale + 10;
        }
        return value;
    }

    // Domain warping based on https://www.iquilezles.org/www/articles/warp/warp.htm
    float warp(in float2 pos) {
        float2 q = float2(  fbm(pos),
                            fbm(pos + 4));

        float turbMult = _Turbulence * (_Time.y * _Speed);
        float2 r = float2(  fbm(pos + q + 0.038 * turbMult),
                            fbm(pos + q + 0.032 * turbMult));

        return fbm(pos + r);
    }

	ENDCG

	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass
		{
			ZWrite Off
            //Blend SrcAlpha OneMinusSrcAlpha
			Blend OneMinusDstColor One
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vert2frag
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float alpha : TEXCOORD1;
                float beta : TEXCOORD2;
                float2 dirTex : TEXCOORD3;
                float viewdot : TEXCOORD4;
            };

            float3 dirToViewSpace(float3 view_zero, float3 dir) {
                float3 perpendicular = normalize(dir);
                float3 view = UnityObjectToViewPos(perpendicular);
                return view - view_zero;
            }

            float4 billboardTransform(in float4 vertex, in float beta) {
                float3 view_zero = UnityObjectToViewPos(float3(0, 0, 0));
                float3 plumeDir = normalize(_PlumeDir.xyz);

                // calculate two vectors perpendicular to plumeDir
                float3 notPlumeDir = normalize(plumeDir + float3(1, 1, 1));
                float3 perpendicular1 = cross(plumeDir, notPlumeDir);
                float3 perpendicular2 = cross(plumeDir, perpendicular1);
                // transform those to view space
                float3 perp1view = dirToViewSpace(view_zero, perpendicular1);
                float3 perp2view = dirToViewSpace(view_zero, perpendicular2);
                // combine the scaling of these vectors through the model matrix
                float transformScaleTotal = sqrt(length(cross(perp1view, perp2view)));
                //float transformScaleTotal = length(perp1view); <- simple version, only uses one dimension to check scaling

                // only transform y with MV matrix, then add xz components, then apply projection
                float scale = (_Scale + beta * _Expand) * transformScaleTotal;
                float4 xz = float4(vertex.x, vertex.z, 0, 0);
                float4 yView = float4(UnityObjectToViewPos(float4(0, vertex.y, 0, 1)), 1);

                return mul(UNITY_MATRIX_P, yView + scale * xz);
            }

            float2 plumeDirToTexSpace() {
                float4 p1 = ComputeScreenPos(UnityObjectToClipPos(float4(0,0,0,1)));
                float4 p2 = ComputeScreenPos(UnityObjectToClipPos(float4(0,-1,0,1)));
                float2 dir = (p2.xy/p2.w - p1.xy/p1.w).xy;
                // correct for the screen aspect ratio (particles appear square on screen, not in screen space)
                float aspect = _ScreenParams.x / _ScreenParams.y;
                return dir * float2(aspect, 1);
            }

            vert2frag vert(appdata v)
            {
                vert2frag o;
                o.uv = v.uv;

                // particle movement in -y direction. store y pos before and after transform
                o.alpha = -v.vertex.y;
                v.vertex.y = -frac(v.vertex.y + _Time.x * _Speed + 0.5*_Seed);
                o.beta = -v.vertex.y;

                // particle-like billboard effect
                o.vertex = billboardTransform(v.vertex, o.beta);
                // transform plume direction to screen space
                o.dirTex = normalize(plumeDirToTexSpace());

                // store how much the effect should be stretched in o.dirTex, depending on viewDir
                float3 viewDir = normalize(ObjSpaceViewDir(float4(0,0,0,1)));
                o.viewdot = abs(dot(viewDir, float3(0,1,0)));

                return o;
            }

            fixed4 frag(vert2frag i) : SV_Target
            {
                // rotate uv, so that uv.x becomes the axis of the stretch effect
                float2 dir = i.dirTex;
                float2 dirNormal = -float2(-dir.y, dir.x);
                float2 rotated = i.uv.x * dir + i.uv.y * dirNormal;

                // shift noise sample position by particle position in the plume
                float2 pos = rotated - 8 * i.alpha;
                // apply scale and stretch effect
                pos *= _Resolution;
                float stretch = 1.0 - i.viewdot;
                pos *= 1 - float2(stretch * _Stretch, 0);

                // sample noise
                float val = pow(warp(pos), 1 + _Exponent * i.beta);

                //val = saturate(val - i.beta*_FadeOut*0.4);

                // fade out towards edges for blending particles together
                float dist = length(2 * i.uv - 1);  // distance from particle center
                val = saturate(val - 0.5 * exp(1.5 * (dist - 1)));
                val *= saturate(1 - exp(dist - 1));

                // float edge = (1 - i.viewdot) * min(1, 8 * _Edge * (1 - i.beta));
                // val *= edge * 16*pow(abs(rotated.y - dot(dir, 0.5)), 2.5) + (1 - edge);

                
                float fade = min(1.0 - pow(i.beta, 1 / _FadeOut), i.beta / _FadeIn);
                //float tColor = pow(1 - val, 1.5);
                float tColor = i.beta;
                float4 color = (1.0 - tColor) * _StartTint + tColor * _EndTint;
                return val * fade * _Brightness * 2.0 * color;
            }

			ENDCG
		}
	}
	
	
	Fallback "KSP/Particles/Additive"
}