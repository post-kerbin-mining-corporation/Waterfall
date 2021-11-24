Shader "Waterfall/Additive Echo (Dynamic)"
{
  Properties
  {
    _MainTex("MainTex (RGBA)", 2D) = "gray" {}
    _StartTint("StartTint", Color) = (1,1,1,1)
    _EndTint("EndTint", Color) = (1,1,1,1)
    _TintFalloff("TintFalloff", Range(1,10)) = 0
    _Falloff("Falloff", Range(0,10)) = 0
    _FalloffStart("Falloff Start", Range(0, 1)) = 0
    _Fresnel("Fresnel", Range(0,10)) = 0
    _FresnelInvert("Fresnel Invert", Range(0,10)) = 0
    _Noise("Noise", Range(0,15)) = 0
    _Brightness("Brightness", Range(0,5)) = 1
    _ClipBrightness("_ClipBrightness", Range(0,50)) = 50

    [Space]
    _FadeIn("Fade In", Range(0,1)) = 0
    _FadeOut("Fade Out", Range(0,1)) = 0

    [Space]

    _ExpandLinear("Linear Expansion", Range(-10, 10)) = 0
    _ExpandSquare("Quadratic Expansion", Range(-10, 10)) = 0
    _ExpandBounded("Bounded Expansion", Range(-10, 10)) = 0

    [Space]
    
    _Seed("Time Offset", Range(-10,10)) = 1
    _SpeedX("Scroll Speed X", Float) = 0
    _SpeedY("Scroll Speed Y", Float) = 1
    
    [Space]

    _TileX("Tiling X", Float) = 1
    _TileY("Tiling Y", Float) = 1
    _SrcMode("SrcMode", Float) = 1
    _DestMode("DestMode", Float) = 6
    
    [Space]

    _Echos("Number of Echos", Range(1, 12)) = 1
    _EchoLength("Echo Length", Range(0, 20)) = 2
    _Stretch("Stretch", Range(-1, 1)) = 0
    _EchoFalloff("Echo Falloff", Range(0, 1)) = 0
    _Dimming("Dimming", Range(-1, 1)) = 0
  }

  SubShader {
    Tags { "Queue" = "Transparent" "IgnoreProjector" = "True"  }


    Blend[_SrcMode][_DestMode]

    ZWrite Off
    ZTest LEqual
    Cull OFF

    Pass {
    
    CGPROGRAM
    #pragma vertex vert
    #pragma geometry geom
    #pragma fragment frag
    #pragma target 4.0

    sampler2D _MainTex;
    float _Fresnel;
    float4 _PlumeDir;
    float _dirAdjust;
    float _Noise;
    float _FresnelInvert;
    float4 _StartTint;
    float4 _EndTint;
    float _TintFalloff;
    float _Falloff;
    float _Brightness;

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
    float _ClipBrightness;
    
    float _Echos;
    float _EchoLength;
    float _Stretch;
    float _Dimming;
    float _EchoFalloff;
    
    struct vdata
    {
        float4 vertex     : POSITION;
        float4 normal     : NORMAL;
        float2 uv_MainTex : TEXCOORD0;
        float4 color      : COLOR;
    };
    
    struct v2g
    {
        float4 vertex       : TEXCOORD0;
        float2 uv_MainTex   : TEXCOORD1;
        float4 color        : COLOR;
        float3 worldNormal  : TEXCOORD2;
    };

    struct g2f
    {
        float4 vertex       : SV_POSITION;
        float2 uv_MainTex   : TEXCOORD0;
        float4 color        : TEXCOORD1;
        float3 data         : TEXCOORD2; // (echo number, local -y value (plumePos), viewdot)
    };

    float lin(float x) { return x; }
    float linDeriv(float x) { return 1; }

    float sqr(float x) { return x * x; }
    float sqrDeriv(float x) { return 2 * x; }

    float bounded(float x) { return 1 - exp(-3 * x); }
    float boundedDeriv(float x) { return 3 * exp(-3 * x); }
    
    v2g vert(vdata i) {
        v2g o;
        
        o.vertex = i.vertex;
        o.color = i.color;
        
        // transform uv-map
        fixed2 scrollUV = i.uv_MainTex;
        scrollUV += mad(_Time.x, fixed2(_SpeedX, _SpeedY), _Seed);
        o.uv_MainTex = scrollUV * float2(_TileX,_TileY);
        
        // vertex displacement along original normal
        float3 normal = i.normal.xyz;
        float value = + _ExpandLinear * lin(-i.vertex.y)
                      + _ExpandSquare * sqr(-i.vertex.y)
                      + _ExpandBounded * bounded(-i.vertex.y);
        o.vertex.xyz += normal * value;

        // new normal calculation
        float deriv = _ExpandLinear * linDeriv(-i.vertex.y)
                    + _ExpandSquare * sqrDeriv(-i.vertex.y)
                    + _ExpandBounded * boundedDeriv(-i.vertex.y);
        normal.y += deriv;
        normal = normalize(normal);
        o.worldNormal = normalize(mul(normal, (float3x3)unity_WorldToObject));
        
        return o;
    }
    
    [maxvertexcount(36)]
    void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream) {
        
        g2f o[3];
        
        o[0].uv_MainTex  = i[0].uv_MainTex;
        o[0].color       = i[0].color;
        
        o[1].uv_MainTex  = i[1].uv_MainTex;
        o[1].color       = i[1].color;
        
        o[2].uv_MainTex  = i[2].uv_MainTex;
        o[2].color       = i[2].color;
        
        float3 plumeDir = normalize(unity_ObjectToWorld._12_22_32);
        float ymult = dot(unity_WorldToObject._21_22_23, plumeDir);
        
        float3 plumePos = float3(-i[0].vertex.y, -i[1].vertex.y, -i[2].vertex.y);
        float EchoLength = _EchoLength;
        int Echos = clamp(_Echos, 1, 12);
        
        for (int E = 0; E < Echos; E++) {
            
            EchoLength *= exp2(_Stretch);
            
            [unroll(3)] for (int v = 0; v < 3; v++) {
                
                // vertex position
                o[v].vertex = UnityObjectToClipPos(i[v].vertex);
                
                // viewdot
                float3 plumeFlow = normalize(cross(cross(plumeDir, i[v].worldNormal), i[v].worldNormal));
                float4 worldPos = mul(unity_ObjectToWorld, i[v].vertex);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
                float3 view = normalize(cross(cross(viewDir, plumeFlow), plumeFlow));
                float viewdot = abs(dot(i[v].worldNormal, view));
                
                o[v].data = float3(E, plumePos[v], viewdot);
                triStream.Append(o[v]);
                
                i[v].vertex.y -= ymult * EchoLength;
                o[v].uv_MainTex.y += 0.5 * _TileY;
            }
            triStream.RestartStrip();
        }
    }

    float4 frag(g2f i) : SV_Target
    {
        half4 c = tex2D(_MainTex, i.uv_MainTex);
        
        half rim = smoothstep(0, 1, saturate(i.data.z)); // fresnel effect
        half rim2 = clamp(1 - rim, 0.001, 10); // inverted fresnel effect
        
        // echo dimming
        float Dim = pow(1.0 - i.data.x / _Echos, _Dimming * (1.0 + _EchoFalloff));

        // falloff start. everything that's affected by falloff goes from
        // _FalloffStart to 1 instead of from 0 to 1
        float g = min(1, (1 + _FalloffStart - i.data.x * _EchoFalloff / _Echos) * mad(-0.5, i.data.y, 1.0));
        float fade = pow(g,_Falloff); // opacity gradient
        float v = pow(fade * (rim * 0.5 + 0.5), _TintFalloff);
        float4 gradient = lerp(_EndTint, _StartTint, min(1, 1 * v)); // tint along gradient and a bit along edges

        float4 col = lerp(0.5,c , _Noise); // control texture contrast
        float4 noise = lerp(col, 1, fade); // less texture near throat

        // fade in / out
        fade *= smoothstep(0, _FadeIn, i.data.y);
        float fOut = _FadeOut + 0.0001;
        fade *= max(0, saturate(i.data.z) - max(0, (fOut + i.data.y - 1) / fOut));

        //combine all the things: Color gradient, fresnel, opacity gradient, texture, and additional _Brightness boost
        //Fresnel strength is being modified by the texture as well, and we increase this as we ramp up the texture strength to avoid hard edges
        
        return clamp(gradient * pow(rim, (1 - noise + 0.5 * _Noise) * _Fresnel) * pow(rim2, clamp((1.0 - noise + 0.5 * _Noise) * _FresnelInvert, 0.001, 10)) * fade * noise * _Brightness * Dim * float4(i.color.rgb, 1), 0, _ClipBrightness);
    
    }

    ENDCG
    
    }
  }
  Fallback "KSP/Particles/Additive"

}