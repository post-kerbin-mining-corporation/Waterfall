﻿Shader "Waterfall/Alpha (Dynamic)"
{
  Properties
  {
    _MainTex("MainTex (RGBA)", 2D) = "gray" {}
    _StartTint("StartTint", Color) = (1,1,1,1)
    _EndTint("EndTint", Color) = (1,1,1,1)
    _TintFalloff("TintFalloff", Range(1,10)) = 0
    _Falloff("Falloff", Range(0,10)) = 0
    _Fresnel("Fresnel", Range(0,10)) = 0
    _FresnelInvert("Fresnel Invert", Range(0,10)) = 0
    _dirAdjust("Adjust for Exhaust Direction", Range(0,1)) = 1
    _PlumeDir("Exhaust Direction", Vector) = (0,1,0,0)
    _Noise("Noise", Range(0,15)) = 0
    _Intensity("Intensity", Range(0,5)) = 1

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
    _Symmetry("Symmetry", Range(0, 24)) = 0
    _SymmetryStrength("Symmetry Strength", Range(0,1)) = 1

    [Space]
    _Seed("Time Offset", Range(-10,10)) = 1
    _SpeedX("Scroll Speed X", Float) = 0
    _SpeedY("Scroll Speed Y", Float) = 1
    [Space]

    _TileX("Tiling X", Float) = 1
    _TileY("Tiling Y", Float) = 1

  }
  SubShader
  {


    ZWrite Off
    ZTest LEqual
    Blend SrcAlpha OneMinusSrcAlpha, One One
    Cull Back


    Tags {"Queue" = "Transparent" "IgnoreProjector" = "True"}

    CGPROGRAM

    #include "SquadCore/LightingKSP.cginc"


    #pragma surface surf Unlit noshadow noambient novertexlights nolightmap alpha  vertex:vert

    #pragma target 3.0


    sampler2D _MainTex;
    float _Fresnel;
    float4 _PlumeDir;
    float _Noise;
    float _FresnelInvert;
    float4 _StartTint;
    float4 _EndTint;
    float _TintFalloff;
    float _Falloff;
    float _Intensity;

    float _FadeIn;
    float _FadeOut;

    float _ExpandOffset;
    float _ExpandLinear;
    float _ExpandSquare;
    float _ExpandBounded;

    float _FalloffStart;
    float _Symmetry;
    float _SymmetryStrength;

    float _Seed;
    float _SpeedX;
    float _SpeedY;
    float _TileX;
    float _TileY;

    struct Input
    {
      float2 uv_MainTex;
      float3 viewDir;
      float3 worldNormal;
      float3 worldPos;
      float4 color : COLOR; //
      float plumePos;
    };

    float lin(float x) { return x; }
    float linDeriv(float x) { return 1; }

    float sqr(float x) { return x * x; }
    float sqrDeriv(float x) { return 2 * x; }

    float bounded(float x) { return 1 - exp(-3 * x); }
    float boundedDeriv(float x) { return 3 * exp(-3 * x); }

    void vert(inout appdata_full v, out Input o) {
      UNITY_INITIALIZE_OUTPUT(Input, o);

      float3 normal = v.normal;
      float3 axis = normalize(_PlumeDir);
      float arg = -dot(v.vertex, axis);
      // vertex displacement along original normal
      float value = _ExpandOffset
      + _ExpandLinear * lin(arg)
      + _ExpandSquare * sqr(arg)
      + _ExpandBounded * bounded(arg);
      v.vertex.xyz += normal * value;

      // new normal calculation
      float deriv = _ExpandLinear * linDeriv(arg)
      + _ExpandSquare * sqrDeriv(arg)
      + _ExpandBounded * boundedDeriv(arg);
      v.normal = normalize(normal + deriv * axis);

      o.plumePos = arg;
    }

    void surf(Input IN, inout SurfaceOutput o)
    {

      fixed2 scrollUV = IN.uv_MainTex;
      fixed xScrollValue = _SpeedX * _Time.x + _Seed;
      fixed yScrollValue = _SpeedY * _Time.x + _Seed;
      scrollUV += fixed2(xScrollValue, yScrollValue);
      half4 c = tex2D(_MainTex, scrollUV * float2(_TileX,_TileY));
      float3 normal = float3(0,0,1);


      // even more complicated fresnel stuff, but it works with fade out!!
      // (previous improved version didn't)
      float3 plumeDir = normalize(mul(unity_ObjectToWorld, _PlumeDir));
      float3 plumeFlow = normalize(cross(cross(plumeDir, IN.worldNormal), IN.worldNormal));
      float3 view = normalize(cross(cross(IN.viewDir, plumeFlow), plumeFlow));

      float3 crs = normalize(cross(IN.worldNormal, IN.viewDir));
      float dirdot = abs(dot(crs, plumeDir));

      half viewdot = abs(dot(IN.worldNormal, view));
      // original viewdot, new one doesn't work with inverted fresnel yet
      half viewdotAlt = abs(dot(IN.worldNormal, IN.viewDir));


      half rim = smoothstep(0, 1, saturate(viewdot)); // fresnel effect
      half rim2 = smoothstep(1, 0, saturate(viewdotAlt)); // inverted fresnel effect

      // falloff start. everything that's affected by falloff goes from
      // _FalloffStart to 1 instead of from 0 to 1
      float g = min(1, (1 + _FalloffStart) * IN.uv_MainTex.g);
      float fade = pow(g,_Falloff); // opacity gradient
      float v = pow(fade * (rim * 0.5 + 0.5), _TintFalloff);
      float4 gradient = lerp(_EndTint, _StartTint, min(1, 1 * v)); // tint along gradient and a bit along edges

      float4 col = lerp(0.5,c.a , _Noise); // control texture contrast
      float4 noise = lerp(col, 1, fade); // less texture near throat


      // SYMMETRY
      fade *= smoothstep(0, _FadeIn, IN.plumePos);// * smoothstep(0, _FadeOut, 1 - IN.plumePos);
      float fOut = _FadeOut + 0.0001;
      fade *= max(0, saturate(viewdot) - max(0, (fOut + IN.plumePos - 1) / fOut));

      float pi = 3.1415926535;
      fade *= 1.0 - _SymmetryStrength + _SymmetryStrength * pow(cos(_Symmetry * pi * IN.uv_MainTex.x), 2);



      //combine all the things: Color gradient, fresnel, opacity gradient, texture, and additional _Brightness boost
      //Fresnel strength is being modified by the texture as well, and we increase this as we ramp up the texture strength to avoid hard edges
      o.Alpha = clamp(pow(rim, (1 - noise + 0.5 * _Noise) * _Fresnel) * pow(rim2, (1 - noise + 0.5 * _Noise) * _FresnelInvert) * fade * noise * _Intensity * IN.color.a, 0, 1);

      o.Albedo = gradient;


    }

    ENDCG
  }
  Fallback "KSP/Particles/Additive"

}