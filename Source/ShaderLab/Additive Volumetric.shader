Shader "Waterfall/Additive (Volumetric)"
{
    Properties
    {
        _MainTex("MainTex (RGBA)", 2D) = "gray" {}
        _StartTint("StartTint", Color) = (1,1,1,1)
        _EndTint("EndTint", Color) = (1,1,1,1)
        _TintFalloff("Tint Falloff", Range(0,5)) = 0
        _TintFresnel("Tint Fresnel", Range(0,5)) = 0
        _Falloff("Falloff", Range(0,10)) = 0
        _FalloffStart("Falloff Start", Range(-1, 1)) = 0
        _Fresnel("Fresnel", Range(0,10)) = 0
        _FresnelFadeIn("Fresnel Fade In", Range(0,1)) = 0
        _FresnelInvert("Fresnel Inverted", Range(0,5)) = 0
        _Brightness("Brightness", Range(0,10)) = 1
        _LengthBrightness("Lengthwise Brightness", Range(0,10)) = 1
        _ClipBrightness("_ClipBrightness", Range(0,50)) = 50

        [Space]

        _Noise("Noise", Range(0,15)) = 0
        _NoiseFresnel("Noise Fresnel", Range(0,15)) = 0
        _FadeIn("Fade In", Range(0,1)) = 0
        _FadeOut("Fade Out", Range(0,1)) = 0

        [Space]

        _ExpandLinear("Linear Expansion", Range(-20, 20)) = 0
        _ExpandSquare("Quadratic Expansion", Range(-20, 20)) = 0

        [Space]

        _Seed("Time Offset", Range(-10,10)) = 1
        _SpeedX("Scroll Speed X", Float) = 0
        _SpeedY("Scroll Speed Y", Float) = 1
        _TileX("Tiling X", Float) = 1
        _TileY("Tiling Y", Float) = 1
    }

    SubShader
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }

        Blend one one

        ZWrite Off
        ZTest Off
        Cull Front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "unitycg.cginc"

            sampler2D_float _CameraDepthTexture;
            sampler2D _MainTex;
            float _Fresnel;
            float _FresnelFadeIn;
            float _Noise;
            float _NoiseFresnel;
            float _FresnelInvert;
            float4 _StartTint;
            float4 _EndTint;
            float _TintFalloff;
            float _TintFresnel;
            float _Falloff;
            float _FalloffStart;
            float _Brightness;
            float _LengthBrightness;
            float _ClipBrightness;
            float _FadeIn;
            float _FadeOut;
            float _FadeOutSharpness;

            float _ExpandLinear;
            float _ExpandSquare;

            float _Seed;
            float _SpeedX;
            float _SpeedY;
            float _TileX;
            float _TileY;

            struct vdata
            {
                float4 vertex     : POSITION;
                float4 color      : COLOR;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex                   : SV_POSITION;
                float4 clipPos                  : COLOR0;    // same as vertex, but vertex seems to be unavailable/changed in fragment shader
                float3 V                        : TEXCOORD0; // (Vx, Vy, Vz) * Sv
                float3 uvy                      : TEXCOORD1; // (u, v, vertex y coordinate, )
                float4 B                        : TEXCOORD2; // (Bm, B0, B1, Bt) * Sv
                nointerpolation float3 a        : TEXCOORD3; // (am, a0, a1)
                nointerpolation float4 C        : TEXCOORD4; // (Cm, C0, C1, Ct)
                nointerpolation float4 Cam      : TEXCOORD5; // (ObjSpace_CameraPosition.xyz, vertex type) 
                nointerpolation float3 y0       : TEXCOORD6; // y0 for (exit, raydepth tail, texture tail) ellipsoids
            };

            // integral of smoothstep(0, 1, x) between x1 (x.x) and x2 (x.y)
            float ssint(float2 x) {
                x = clamp(x * x * x, 0, 1) * (max(x, 1) - 0.5 * min(x, 1));
                return x.y-x.x;
            }

            v2f vert(vdata i) {
                v2f o;
                                
                // vertex type
                o.Cam.w = 0.1 * round(i.color.a * 10.0); 

                // Camera position in object space
                o.Cam.xyz = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1.0)).xyz;

                // mesh ellipsoid parameters
                float4 a;
                a.x = mad(_ExpandLinear, _ExpandLinear, -10 * _ExpandSquare);
                float4 b;
                b.x = mad(-2.0, _ExpandLinear, -10 * _ExpandSquare);

                // vertex position
                float3 vertex = i.vertex.xyz;
                vertex.xz *= sqrt(mad(vertex.y, mad(a.x, vertex.y, b.x), 1.0));   // vertex xz-position
                o.vertex = UnityObjectToClipPos(float4(vertex, 1));               // vertex clip position
                o.clipPos = o.vertex;
                
                // ray values
                o.V = vertex - o.Cam.xyz;
                o.uvy.z = vertex.y;
                float CV = dot(o.Cam.xz, o.V.xz);
                float CC = dot(o.Cam.xz, o.Cam.xz);

                // object space radius in y-direction of a world space sphere at y=0
                float R0sphere = length(mul((float3x3)unity_WorldToObject, length(unity_ObjectToWorld._11_21_31) * normalize(unity_ObjectToWorld._12_22_32))); 

                // bounding ellipsoid parameters
                float2 yt = float2(0.0, -1.0);                                  // tangent y-values
                float2 RRt = mad(yt, mad(a.x, yt, b.x), 1.0);                   // Rm² at yt
                float2 dRRt = 2.0 * a.x * yt + b.x;                             // derivative of Rm² at yt
                float2 y0 = float2(2 * R0sphere, -1.5);                         // zero y-values
                float2 Dy = yt - y0;
                
                // avoid hyperbolic ellipsoids
                bool2 hyp = mad(dRRt, (yt - y0), -RRt) > 0.0;
                
                // construct bounding ellipsoids
                float4 c;
                c.x = 1.0;
                a.yz = hyp ? 0.0                 : mad(dRRt, Dy, -RRt) / (Dy * Dy);
                b.yz = hyp ? dRRt                : dRRt - 2.0 * a.yz * yt;
                c.yz = hyp ? mad(-yt, dRRt, RRt) : -y0 * mad(a.yz, y0, b.yz);
                
                // texture tail ellipsoid
                a.w = 0.0;
                b.w = dRRt.y;
                c.w = 1.0 - a.x;
                
                // Quadratic intersection equation
                o.B = mad(o.V.y, mad(2.0 * a, o.Cam.y, b), -2.0 * CV);  // B * Sv
                o.C = mad(o.Cam.y, mad(o.Cam.y, a, b), c - CC);         // C
                
                // recalculate y0's
                o.y0.z = (a.x - 1.0) / dRRt.y;               // texture tail
                if (o.y0.z >= -1.0) { o.y0.z = -1000000.0; } // if tail expands, set fake y0 far away
                y0 = hyp ? yt - RRt / dRRt : y0;             // bounding ellipsoids
                
                // check if bounding ellipsoids are longer than mesh ellipsoid
                float D0 = mad(b.x, b.x, -4.0 * a.x * c.x);
                float2 y0m = (-b.x + sign(a.x) * float2(1.0, -1.0) * sqrt(D0)) / (2.0 * a.x);   // y0's for the mesh
                if (y0m.x > 0.0 && (y0m.x < y0.x || (y0.x < 0.0001 && y0.x > 0.0))) {
                    y0.x = y0m.x;
                    a.y = a.x;
                    b.y = b.x;
                    c.y = c.x;
                    o.B.y = o.B.x;
                    o.C.y = o.C.x;
                }
                if (y0m.y < -1.0 && (y0m.y > y0.y || (y0.y > -1.0001 && y0.y < -1.0))) {
                    y0.y = y0m.y;
                    a.z = a.x;
                    b.z = b.x;
                    c.z = c.x;
                    o.B.z = o.B.x;
                    o.C.z = o.C.x;
                }
                
                o.y0.xy = y0;   // y0s of the bounding ellipsoids
                o.a = a.xyz;    // final a-values
                
                // uv map
                if (o.Cam.w < 0.15) {
                    float umir = _SpeedX * _Time.x;
                    float u = 2 * abs(i.uv_MainTex.x - umir - floor(i.uv_MainTex.x - umir + 0.5)) + umir;
                    o.uvy.xy = (float2(u, i.uv_MainTex.y) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                }
                else {
                    o.uvy.xy = float2(0.0, 0.0);
                }
                
                return o;
            }


            float4 frag(v2f i) : SV_Target
            {   
                // construct camera view parameters and normalize
                float Sv = length(i.V);                                 // distance to fragment
                i.V /= Sv;                                              // normalize V ray vector
                i.B /= Sv;                                              // normalize B-parameter
                float VV = dot(i.V.xz, i.V.xz);
                float CV = dot(i.Cam.xz, i.V.xz);
                float CC = dot(i.Cam.xz, i.Cam.xz);

                // view ray y-direction is positive?
                float VSgn = mad(2.0, step(0.0, i.V.y), -1.0);

                // intersection parameters
                float b = mad(-2.0, _ExpandLinear, -10 * _ExpandSquare);
                float4 A = float4(mad(i.V.y, i.V.y * i.a, -VV), -VV);
                float4 D = mad(i.B, i.B, - 4.0 * A * i.C);                                        // Discriminant for the plume intersection

                // intersection points
                float2 Si = (-i.B.x + float2(1.0, -1.0) * sqrt(D.x)) / (2 * A.x);                 // (entry, exit) plume intersection points
                float2 St = Si;
                float2 S01 = (float2(0.0, -1.0) - i.Cam.y) / i.V.y;                               // S-values for y = 0 & -1
                float2 Sbound = (-i.B.yz - float2(1.0, -1.0) * VSgn * sqrt(D.yz)) / (2 * A.yz);   // (bound at 0, bound at -1) bounding ellipsoid intersection points
                float2 Stex = (-i.B.yw - float2(1.0, -1.0) * VSgn * sqrt(D.yw)) / (2 * A.yw);     // tail paraboloid intersection points
                float2 dtexfade = 0.8 * (float2(0.0, -1.0) - i.y0.xz);
                
                // change from (0,-1) to (entry , exit)
                if (i.V.y >= 0) {
                    Stex = Stex.yx;
                    Sbound = Sbound.yx;
                    S01 = S01.yx;
                    i.y0.xz = i.y0.zx;
                    dtexfade = dtexfade.yx;
                }

                // check for hyperbolic intersection
                if (A.x >= 0) {
                    if (i.Cam.w < 0.15) { Si.x = Sbound.x; St.x = Stex.x; }
                    else if (D.x < 0) { Si = Sbound; St = Stex; }
                    else {
                        if (Si.x < Sv) {
                            Si.y = Sbound.y;
                            St.y = Stex.y;
                            if (Si.x < S01.x) { Si.x = Sbound.x; St.x = Stex.x; }
                        }
                        else {
                            Si.x = Sbound.x;
                            St.x = Stex.x;
                            if (Si.y > S01.y) { Si.y = Sbound.y; St.y = Stex.y; }
                        }
                    }
                }
                else {
                    // bound intersection points within bounding ellipsoids
                    if (Si.x < S01.x) { Si.x = Sbound.x; St.x = Stex.x; }
                    if (Si.y > S01.y) { Si.y = Sbound.y; St.y = Stex.y; }
                }

                // texture at entry
                float2 c;                                 // texture at (entry, exit)
                Si.x = max(0.0, Si.x);                    // make sure entry is in front of camera
                St.x = max(0.0, St.x);                    // make sure entry is in front of camera
                float3 Ptex = mad(i.V, St.x, i.Cam.xyz);
                float2 ytex;
                ytex.x = Ptex.y;
                float umir = _SpeedX * _Time.x;
                float u = 0.1591549431 * atan2(Ptex.z, Ptex.x) + step(Ptex.z, 0.0);
                u = 2.0 * abs(u - umir - floor(u - umir + 0.5)) + umir;
                float2 uv = (float2(u, lerp(0.5, 1.0, 1.0 + Ptex.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                c.x = length(tex2D(_MainTex, uv).rgb);

                // texture at exit
                if (i.Cam.w < 0.15) {
                    uv = i.uvy.xy;
                    ytex.y = i.uvy.z;
                }
                else {
                    Ptex = mad(i.V, St.y, i.Cam.xyz);
                    ytex.y = Ptex.y;
                    u = 0.1591549431 * atan2(Ptex.z, Ptex.x) + step(Ptex.z, 0.0);
                    u = 2.0 * abs(u - umir - floor(u - umir + 0.5)) + umir;
                    uv = (float2(u, lerp(0.5, 1.0, 1.0 + Ptex.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                }
                c.y = length(tex2D(_MainTex, uv).rgb);

                // final Ray segment
                float2 yRange = mad(Si, i.V.y, i.Cam.y);
                if (abs(yRange.x - yRange.y) < 0.0001) { yRange.y += VSgn * 0.0001; }
                float ydelta = yRange.y - yRange.x;
                float Raydepth = Si.y - Si.x;
                
                // read depth of solid object from _CameraDepthTexture
                float4 screenUV = ComputeGrabScreenPos(i.clipPos);
                float opaqueDepth = tex2Dproj(_CameraDepthTexture, screenUV).x;

                // ray entry and exit positions
                float3 objectEntry = mad(Si.x, i.V, i.Cam);
                float3 objectExit = mad(Si.y, i.V, i.Cam);
                // transform to clip space
                float4 clipEntry = UnityObjectToClipPos(float4(objectEntry, 1));
                float4 clipExit = UnityObjectToClipPos(float4(objectExit, 1));
                // get linear depth of solid object and clip space positions
                float depthEntry = LinearEyeDepth(clipEntry.z / clipEntry.w);
                float depthExit = LinearEyeDepth(clipExit.z / clipExit.w);
                float depthSolid = LinearEyeDepth(opaqueDepth);

                // custom z-test
                if (depthEntry > depthSolid) {
                    // entry point is behind solid object -> nothing is visible
                    discard;
                } else if (depthExit > depthSolid) {
                    // exit point is behind solid object -> some of the plume is visible
                    Raydepth *= (depthSolid - depthEntry) / (depthExit - depthEntry);
                }

                // Gauss legendre quadrature sampling
                float4 Ss = lerp(Si.x, Si.y, float4(0.069431844, 0.330009478, 0.669990522, 0.930568156));
                float4 ys = mad(Ss, i.V.y, i.Cam.y);
                float4 Weights = float4(0.1739274226, 0.3260725774, 0.3260725774, 0.1739274226);
                
                // intensity
                float4 rr = rcp(mad(ys, mad(i.a.x, ys, b), 1.0));
                float4 Samples = sqrt(mad(4.0 * _LengthBrightness * _LengthBrightness * i.V.y, i.V.y, VV * rr));

                // Fadeout
                rr *= mad(Ss, mad(VV, Ss, 2.0 * CV), CC);
                Samples *= smoothstep(min(_FadeOut + i.y0.y, -1.0), _FadeOut - 1.0, ys - max(_FadeOut, -1.0 - i.y0.y) * (1.0 - sqrt(1.0 - rr)));
                
                // falloff parameter
                float4 f = min(0.0, (ys + _FalloffStart));
                f *= -f;
                
                // Falloff
                float4 N = exp2(f * 5.0 * _Falloff);
                Samples *= N;
                
                // Color Gradient
                float Gradient = dot(exp2((f * (_TintFalloff * 5.0) - rr * rr * (_TintFresnel * 5.0))) * Samples, Weights) / dot(Samples, Weights);
                float4 Color = lerp(_EndTint, _StartTint, Gradient);
                
                // Inverted Fresnel
                f = rr - 1.0;
                Samples *= exp2(-10.0 * _FresnelInvert * f * f);
                
                // Fresnel
                f = (_Fresnel + 0.01) * (1.0 - exp2(2.0 * min(0.0, mad(_FresnelFadeIn, ys, _FresnelFadeIn - 1.0))));    // fresnel amount
                f *= f;
                f *= rr / (rr - mad(0.15, f, 1.0));
                N *= exp2(0.5 * _NoiseFresnel * f);
                Samples *= exp2(f);
                
                // Noise
                dtexfade = (ytex - i.y0.xz) / dtexfade;
                c = lerp(1.0, c, min(1.0, dtexfade * dtexfade));
                N = _Noise * (1.0 - N) * rr;
                Samples *= lerp(1.0, lerp(c.x, c.y, (Ss - St.x) / (St.y - St.x)), N);
                
                //Fade
                float Fade = dot(Samples, Weights);
                
                // Fadein
                float Fadein = _FadeIn + 0.0001;
                Fadein = -ssint(-yRange / Fadein) * Fadein / ydelta;
                
                return clamp(0.5 * _Brightness * Raydepth * Fade * Fadein * Color, 0.0, _ClipBrightness);
            }

            ENDCG
        }
    }
    Fallback "KSP/Particles/Additive"

} 