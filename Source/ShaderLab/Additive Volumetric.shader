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

        _ExpandLinear("Linear Expansion", Range(-10, 10)) = 0
        _ExpandSquare("Quadratic Expansion", Range(-10, 10)) = 0

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
                float3 depth                    : COLOR0;    // (relative screen uv, exit depth)
                float3 V                        : TEXCOORD0; // (Vx, Vy, Vz) * Sv
                float3 uvy                      : TEXCOORD1; // (u, v, vertex y coordinate, )
                float3 B                        : TEXCOORD2; // (Bm, B0, B1) * Sv
                nointerpolation float3 a        : TEXCOORD3; // (am, a0, a1)
                nointerpolation float3 C        : TEXCOORD4; // (Cm, C0, C1)
                nointerpolation float4 Cam      : TEXCOORD5; // (ObjSpace_CameraPosition.xyz, vertex type) 
                nointerpolation float4 y0       : TEXCOORD6; // (y0.x, y0.y, y00, y01)
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
                o.a.x = mad(_ExpandLinear, _ExpandLinear, -10 * _ExpandSquare);
                float4 b;
                b.x = mad(-2.0, _ExpandLinear, -10 * _ExpandSquare);

                // vertex position
                float3 vertex = i.vertex.xyz;
                vertex.xz *= sqrt(mad(vertex.y, mad(o.a.x, vertex.y, b.x), 1.0));   // vertex xz-position
                o.vertex = UnityObjectToClipPos(float4(vertex, 1));                 // vertex clip position
                
                // depth values
                float3 screenUV = ComputeGrabScreenPos(o.vertex).xyw;
                o.depth.xy = screenUV.xy/screenUV.z;
                o.depth.z = LinearEyeDepth(o.vertex.z / o.vertex.w);
                
                // ray values
                o.V = vertex - o.Cam.xyz;
                o.uvy.z = vertex.y;
                float CV = dot(o.Cam.xz, o.V.xz);
                float CC = dot(o.Cam.xz, o.Cam.xz);

                // object space radius of a world space sphere at y=0
                float R0sphere = length(mul((float3x3)unity_WorldToObject, length(unity_ObjectToWorld._11_21_31) * normalize(unity_ObjectToWorld._12_22_32))); 

                // bounding ellipsoid y-ranges
                float2 yt = float2(0.0, -1.0);              // tangent y-values
                float2 dRRdy = 2.0 * o.a.x * yt + b.x;      // derivative of the mesh R² at yt
                float2 y0 = float2(2 * R0sphere, -4.0);     // zero y-values
                                
                if (dRRdy.x < 0) { y0.x = min(y0.x, -0.9 / dRRdy.x); }
                if (dRRdy.y < 0) { y0.y = min(y0.y, 0.9 * (o.a.x - 1.0) / dRRdy.y); }
                                
                float2 Dy = yt - y0;
                
                // bounding ellipsoid parameters
                o.a.yz = (o.a.x * yt * yt - 1.0 - y0 * dRRdy) / (Dy * Dy);
                b.yz = dRRdy - 2.0 * o.a.yz * yt;
                float3 c = float3(1.0, -y0 * (o.a.yz * y0 + b.yz));

                // Quadratic intersection equation
                o.B = mad(o.V.y, mad(2.0 * o.a, o.Cam.y, b), -2.0 * CV);  // B * Sv
                o.C = mad(o.Cam.y, mad(o.Cam.y, o.a, b), c - CC);         // C

                // check if bounding ellipsoids are longer than mesh ellipsoid
                float D0 = mad(b.x, b.x, -4.0 * o.a.x * c.x);
                o.y0.xy = (-b.x + sign(o.a.x) * float2(-1.0, 1.0) * sqrt(D0)) / (2.0 * o.a.x);
                if ((o.y0.y > 0.0 && o.y0.y < y0.x) || y0.x < 0.0001) {
                    y0.x = o.y0.y;
                    o.a.y = o.a.x;
                    b.y = b.x;
                    c.y = c.x;
                    o.B.y = o.B.x;
                    o.C.y = o.C.x;
                }
                if (o.y0.x < -1.0 && o.y0.x > y0.y) {
                    y0.y = o.y0.x;
                    o.a.z = o.a.x;
                    b.z = b.x;
                    c.z = c.x;
                    o.B.z = o.B.x;
                    o.C.z = o.C.x;
                }
                o.y0.zw = y0;    // y00, y01
                o.y0.z *= 0.9;  // less noise in nozzle

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
                float3 A = mad(i.V.y, i.V.y * i.a, -VV);
                float3 D = mad(i.B, i.B, - 4.0 * A * i.C); // Discriminant for the plume intersection

                // intersection points
                float2 Si = (-i.B.x + float2(1.0, -1.0) * sqrt(D.x)) / (2 * A.x);               // (entry, exit) plume intersection points
                float2 S01 = (float2(0.0, -1.0) - i.Cam.y) / i.V.y;                             // S-values for y = 0 & -1
                float2 Stex = (-i.B.yz - float2(1.0, -1.0) * VSgn * sqrt(D.yz)) / (2 * A.yz);   // (bound at 0, bound at -1) bounding ellipsoid intersection points
                
                // change from (0,-1) to (entry , exit)
                float2 boundDist = float2(i.y0.z, 1.0 + i.y0.w);
                if (i.V.y >= 0) {
                    Stex = Stex.yx;
                    S01 = S01.yx;
                    i.y0.zw = i.y0.wz;
                    boundDist = boundDist.yx;
                }

                // check for hyperbolic intersection
                if (A.x >= 0) {
                    if (i.Cam.w < 0.15) { Si.x = Stex.x; }
                    else if (D.x < 0) { Si = Stex; }
                    else {
                        if (Si.x < Sv) {
                            Si.y = Stex.y;
                            if (Si.x < S01.x) { Si.x = Stex.x; }
                        }
                        else {
                            Si.x = Stex.x;
                            if (Si.y > S01.y) { Si.y = Stex.y; }
                        }
                    }
                }
                else {
                    // bound intersection points within bounding ellipsoids
                    if (Si.x < S01.x) { Si.x = Stex.x; }
                    if (Si.y > S01.y) { Si.y = Stex.y; }
                }

                // texture at entry
                float2 c;                             // texture at (entry, exit)
                float Sentry = max(Si.x, 0.0);        // make sure entry is in front of camera
                Si = max(0.0, Si);
                Stex = Si;
                float3 Ptex = mad(i.V, Sentry, i.Cam.xyz);
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
                    Ptex = mad(i.V, Stex.y, i.Cam.xyz);
                    ytex.y = Ptex.y;
                    u = 0.1591549431 * atan2(Ptex.z, Ptex.x) + step(Ptex.z, 0.0);
                    u = 2.0 * abs(u - umir - floor(u - umir + 0.5)) + umir;
                    uv = (float2(u, lerp(0.5, 1.0, 1.0 + Ptex.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                }
                c.y = length(tex2D(_MainTex, uv.xy).rgb);

                // final Ray segment
                Si = clamp(Si, S01.x, S01.y);
                float2 yRange = mad(Si, i.V.y, i.Cam.y);
                if (abs(yRange.x - yRange.y) < 0.0001) { yRange.y += VSgn * 0.0001; }
                float ydelta = yRange.y - yRange.x;
                float Raydepth = Si.y - Si.x;
                
                // read depth of solid object from _CameraDepthTexture
                float opaqueDepth = tex2Dlod(_CameraDepthTexture, float4(i.depth.xy, 0.0, 0.0)).x;
                // ray entry positions
                float3 objectEntry = mad(Si.x, i.V, i.Cam);
                // transform to clip space
                float4 clipEntry = UnityObjectToClipPos(float4(objectEntry, 1));
                // get linear depth of solid object and clip space position
                float depthEntry = LinearEyeDepth(clipEntry.z / clipEntry.w);
                float depthSolid = LinearEyeDepth(opaqueDepth);

                // custom z-test
                if (depthEntry > depthSolid) {
                    // entry point is behind solid object -> nothing is visible
                    discard;
                } else if (i.depth.z > depthSolid) {
                    // exit point is behind solid object -> some of the plume is visible
                    Raydepth *= (depthSolid - depthEntry) / (i.depth.z - depthEntry);
                }

                // Gauss legendre quadrature sampling
                float4 Ss = lerp(Si.x, Si.y, float4(0.069431844, 0.330009478, 0.669990522, 0.930568156));
                float4 Weights = 0.5 * float4(0.3478548451, 0.6521451549, 0.6521451549, 0.3478548451);
                float4 ys = mad(Ss, i.V.y, i.Cam.y);
                
                // intensity
                float4 rr = rcp(mad(ys, mad(i.a.x, ys, b), 1.0));
                float4 Samples = sqrt(mad(4.0 * _LengthBrightness * _LengthBrightness * i.V.y, i.V.y, VV * rr));

                // Fadeout
                float Fadeout = _FadeOut + 0.001;
                rr *= mad(Ss, mad(VV, Ss, 2.0 * CV), CC);
                float4 f = Fadeout * (1.0 - sqrt(1.0 - rr)) - 1.0;
                float3 yav = 0.5 * (ys.xyz + ys.yzw);
                f = float4(ssint((float2(yRange.x, yav.x) - f.x) / Fadeout), 
                           ssint((yav.xy - f.y) / Fadeout), 
                           ssint((yav.yz - f.z) / Fadeout),
                           ssint((float2(yav.z, yRange.y) - f.w) / Fadeout));
                Samples *= f * Fadeout / (float4(yav, yRange.y) - float4(yRange.x, yav));
                
                // falloff
                ys = min(0.0, (ys + _FalloffStart));
                ys *= -ys;
                
                // Inverted Fresnel
                f = rr - 1.0;
                
                // Fresnel
                float Fresnel = _Fresnel + 0.01;
                Fresnel *= Fresnel;
                
                // combine Falloff, Fresnel and Inverted Fresnel
                Samples *= exp2(ys * 5.0 * _Falloff - (10 * _FresnelInvert) * f * f + Fresnel * rr / (rr - mad(0.15, Fresnel, 1.0)));
                
                //Fade
                float Fade = dot(Samples, Weights);
                
                // Color Gradient
                f = exp2(ys * (_TintFalloff * 5.0) - rr * rr * (_TintFresnel * 5.0)) * Samples;
                float Gradient = dot(f, Weights) / dot(Samples, Weights);
                float4 Color = lerp(_EndTint, _StartTint, Gradient);
                
                // Fadein
                float Fadein = _FadeIn + 0.0001;
                Fadein = -ssint(-yRange / Fadein) * Fadein / ydelta;

                // Noise reference points on view ray
                float Smid = dot(Stex, 0.5);
                float ymid = mad(i.V.y, Smid, i.Cam.y);
                float rrmid = mad(Smid, mad(VV, Smid, 2.0 * CV), CC) / mad(ymid, mad(i.a.x, ymid, b), 1.0);
                ymid = clamp(ymid, -1.0, 0.0);
                float2 ynoise = min(0.0, 0.5 * (ymid + yRange) + _FalloffStart);
                
                // Strength of the noise pattern depends on local Falloff & Fresnel
                float2 NoiseStrength = exp2(-ynoise * ynoise * _Falloff + 0.1 * _NoiseFresnel * Fresnel * rrmid / (rrmid - mad(0.15, Fresnel, 1.0)));
                
                // fade out the noise at start and end of plume
                float2 NoiseAmount = max(0.0, (i.y0.zw - ytex) / boundDist); 
                NoiseAmount = min(1.0, NoiseAmount * NoiseAmount);
                
                // multiply noise amount factors together (less noise in the center of the plume)
                NoiseAmount *= mad(0.7, rrmid, 0.3) * _Noise;
                
                // final noise texture contrast, weighted by entry/exit raydepth
                float Noise = dot(max(0.0, lerp(lerp(1.0, c, NoiseAmount), 1.0, NoiseStrength)), float2(Smid - Stex.x, Stex.y - Smid) / (Stex.y - Stex.x));

                // final pixel value
                return clamp(0.5 * _Brightness * Raydepth * Fade * Noise * Fadein * Color, 0.0, _ClipBrightness);
            }

            ENDCG
        }
    }
    Fallback "KSP/Particles/Additive"

} 