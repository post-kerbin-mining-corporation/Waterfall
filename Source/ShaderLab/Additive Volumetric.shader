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
        ZTest LEqual
        Cull Front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0


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
                float3 V                        : TEXCOORD0; // (Vx, Vy, Vz) * Sv
                float3 uvy                      : TEXCOORD1; // (u, v, vertex y coordinate)
                float3 B                        : TEXCOORD2; // (Bm, B0, B1) * Sv
                nointerpolation float3 a        : TEXCOORD3; // (am, a0, a1)
                nointerpolation float3 C        : TEXCOORD4; // (Cm, C0, C1)
                nointerpolation float4 Cam      : TEXCOORD5; // (ObjSpace_CameraPosition.xyz, vertex type) 
                nointerpolation float4 data     : TEXCOORD6; // (y0.x, y0.y, y00, y01) 
            };
            
            // integral of smoothstep(0, 1, x) between x1 (x.x) and x2 (x.y)
            float ssint(float2 x) {
                float2 X = clamp(x * x * x, 0, 1) * (max(x, 1) - 0.5 * min(x, 1));
                return X.y-X.x;
            }
            
            // numerically integrated average Fade
            float fade(float2 SRange, float yFadeout, float3 Raypar, float Vy, float Cy, float a, float b, out float Fadeout) {
                float4 Ss = lerp(SRange.x, SRange.y, float4(0.069431844, 0.330009478, 0.669990522, 0.930568156));
                float4 Weights = float4(0.3478548451, 0.6521451549, 0.6521451549, 0.3478548451);
                float4 ys = mad(Ss, Vy, Cy);
                
                // falloff
                float4 Samples = pow(min(1.0, (1.0 + _FalloffStart) * mad(0.5, ys, 1.0)), _Falloff);
                
                // intensity
                float4 rr = rcp(mad(ys, mad(a, ys, b), 1.0));  
                Samples *= sqrt(mad(4.0 * _LengthBrightness * _LengthBrightness * Vy, Vy, Raypar.x * rr));
                
                // Fadeout
                Fadeout = 0;
                if (ys.y < yFadeout) {
                    rr *= mad(Ss, mad(Raypar.x, Ss, 2.0 * Raypar.y), Raypar.z);
                    float4 fout = (_FadeOut + 0.001) * (1.0 - sqrt(1.0 - rr));
                    fout = smoothstep(-1.001, -1.0 + _FadeOut, ys - fout);
                    Fadeout = 0.5 * dot(fout, Weights);
                    Samples *= fout;
                }
                
                return 0.5 * dot(Samples, Weights);
            }
            
            v2f vert(vdata i) {
                v2f o;
                
                // vertex type
                o.Cam.w = 0.1 * round(i.color.a * 10.0); 
                
                // Camera position in object space
                o.Cam.xyz = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1.0)).xyz;
                
                // mesh ellipsoid parameters
                o.a.x = mad(_ExpandLinear, _ExpandLinear, -10 * _ExpandSquare);
                float3 b;
                b.x = mad(-2.0, _ExpandLinear, -10 * _ExpandSquare);
                                    
                // vertex position
                float3 vertex = i.vertex.xyz;
                vertex.xz *= sqrt(mad(vertex.y, mad(o.a.x, vertex.y, b.x), 1.0));   // vertex xz-position
                o.vertex = UnityObjectToClipPos(float4(vertex, 1));                 // vertex clip position
                
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
                o.data.xy = (-b.x + sign(o.a.x) * float2(-1.0, 1.0) * sqrt(D0)) / (2.0 * o.a.x);
                if (o.data.y > 0.0 && o.data.y < y0.x) {
                    o.a.y = o.a.x;
                    b.y = b.x;
                    c.y = c.x;
                    o.B.y = o.B.x;
                    o.C.y = o.C.x;
                }
                if (o.data.x < -1.0 && o.data.x > y0.y) {
                    o.a.z = o.a.x;
                    b.z = b.x;
                    c.z = c.x;
                    o.B.z = o.B.x;
                    o.C.z = o.C.x;
                }
                float2 D01 = mad(b.yz, b.yz, -4.0 * o.a.yz * c.yz);
                o.data.zw = (-b.yz + sign(o.a.yz) * float2(1.0, -1.0) * sqrt(D01)) / (2.0 * o.a.yz); // y00, y01
                
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
                // world space view component in plume direction
                float Vyw = dot(normalize(mul((float3x3)unity_ObjectToWorld, i.V)), normalize(unity_ObjectToWorld._12_22_32)); 
                
                // construct camera view parameters and normalize
                float Sv = length(i.V);                                 // distance to fragment
                i.V /= Sv;                                              // normalize V ray vector
                i.B /= Sv;                                              // normalize B-parameter
                float VV = dot(i.V.xz, i.V.xz);
                float CV = dot(i.Cam.xz, i.V.xz);
                float CC = dot(i.Cam.xz, i.Cam.xz);
                
                // view ray y-direction is positive?
                float VSgn = mad(2.0, step(0.0, i.V.y), -1.0);
                bool Vdir = i.V.y >= 0;
                
                // intersection parameters
                float3 A = mad(i.V.y, i.V.y * i.a, -VV);
                float3 D = mad(i.B, i.B, - 4.0 * A * i.C); // Discriminant for the plume intersection
                
                // intersection points
                float2 Si = (-i.B.x + float2(1.0, -1.0) * sqrt(D.x)) / (2 * A.x);               // (entry, exit) plume intersection points
                float2 S01 = (float2(0.0, -1.0) - i.Cam.y) / i.V.y;                             // S-values for y = 0 & -1
                float2 Stex = (-i.B.yz - float2(1.0, -1.0) * VSgn * sqrt(D.yz)) / (2 * A.yz);   // (bound at 0, bound at -1) bounding ellipsoid intersection points
                
                // change from (0,-1) to (entry , exit)
                float2 boundDist = float2(i.data.z, 1.0 + i.data.w);
                if (Vdir) {
                    Stex = Stex.yx;
                    S01 = S01.yx;
                    i.data.zw = i.data.wz;
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
                float2 c;                                               // texture at (entry, exit)
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
                
                // deepest ray penetration radius: rrmin
                float b = mad(-2.0, _ExpandLinear, -10 * _ExpandSquare);
                float3 coef = float3(2.0 * i.a.x * i.V.y * i.V.y, i.V.y * mad(2 * i.a.x, i.Cam.y, b), -2.0 * mad(i.Cam.y, mad(i.a.x, i.Cam.y, b), 1.0));
                float3 m = float3(dot(float2(CV, -VV), coef.xy), dot(float2(CC, VV), coef.xz), dot(float2(CC, CV), coef.yz));
                float Dm = mad(m.y, m.y, -4.0 * m.x * m.z);
                
                float2 Sopt = (-m.y + float2(-1.0, 1.0) * sqrt(Dm)) / (2.0 * m.x);
                float2 yopt = mad(Sopt, i.V.y, i.Cam.y);
                
                float rrmin;
                if (abs(m.x) < 0.00001) {
                    float Smin = clamp(-CV / VV, Si.x, Si.y);
                    float ymin = mad(Smin, i.V.y, i.Cam.y);
                    rrmin = mad(Smin, mad(VV, Smin, 2.0 * CV), CC) / mad(ymin, mad(i.a.x, ymin, b), 1.0);
                }
                else if (i.a.x < 0.0) {
                    float Smin = (yopt.x > i.data.x && yopt.x < i.data.y) ? Sopt.x : Sopt.y;
                    Smin = clamp(Smin, Si.x, Si.y);
                    float ymin = mad(Smin, i.V.y, i.Cam.y);
                    rrmin = mad(Smin, mad(VV, Smin, 2.0 * CV), CC) / mad(ymin, mad(i.a.x, ymin, b), 1.0);
                }
                else {
                    float2 rrS = mad(Si, mad(VV, Si, 2.0 * CV), CC) / mad(yRange, mad(i.a.x, yRange, b), 1.0);
                    int IdxMin = step(rrS.y, rrS.x);
                    rrmin = rrS[IdxMin];
                    Sopt = clamp(Sopt, Si.x, Si.y);
                    yopt = mad(Sopt, i.V.y, i.Cam.y);
                    rrS = mad(Sopt, mad(VV, Sopt, 2.0 * CV), CC) / mad(yopt, mad(i.a.x, yopt, b), 1.0);
                    IdxMin = step(rrS.y, rrS.x);
                    if (rrS[IdxMin] < rrmin) {
                        rrmin = rrS[IdxMin];
                    }
                }
                rrmin = max(0.0001, rrmin);
                
                // Gauss legendre quadrature sampling
                float yFout = mad(2.0, _FadeOut, -1.0);
                float SFout = (yFout - i.Cam.y) / i.V.y;
                float Fade;
                float Fadeout;

                if (SFout < Si.y && SFout > Si.x) {
                    float F;
                    // first segment
                    Fade = fade(float2(Si.x, SFout), yFout, float3(VV, CV, CC), i.V.y, i.Cam.y, i.a.x, b, F) * (SFout - Si.x);      
                    Fadeout = F * (SFout - Si.x);
                    
                    // second segment
                    Fade += fade(float2(SFout, Si.y), yFout, float3(VV, CV, CC), i.V.y, i.Cam.y, i.a.x, b, F) * (Si.y - SFout);     
                    Fadeout += F * (Si.y - SFout);
                    
                    // weighted average
                    Fade /= Raydepth;
                    Fadeout /= Raydepth;
                }
                else {
                    Fade = fade(float2(Si.x, Si.y), yFout, float3(VV, CV, CC), i.V.y, i.Cam.y, i.a.x, b, Fadeout);
                }
                
                // Fadein
                float Fadein = _FadeIn + 0.0001;
                Fadein = -ssint(-yRange / Fadein) * Fadein / ydelta;
                
                // Fresnel
                float Fresnel = 1.0 - pow(rrmin, rcp(mad(_Fresnel, mad(-0.002, Fade, 0.1), 0.01)));
                
                // Inverted Fresnel
                float FresnelInvert = rrmin * (2.0 - rrmin);
                float FrInvStrength = _FresnelInvert * mad(1.5, mad(-Vyw, Vyw, 1.0), 1.0);
                FresnelInvert = pow(FresnelInvert, mad(0.8, FrInvStrength, 0.5));
                FresnelInvert = lerp(FresnelInvert, 1.0, exp2(-FrInvStrength));
                
                // Noise
                float Smid = dot(Stex, 0.5);
                float2 ynoise = mad(i.V.y, 0.5 * (clamp(Smid, Si.x, Si.y) + Si), i.Cam.y);
                float2 g = min(1.0, (1.0 + _FalloffStart) * mad(0.5, ynoise, 1.0));
                float2 NoiseStrength = pow(g, _Falloff);
                float2 NoiseFade = (i.data.zw - ytex) / boundDist;
                NoiseFade = min(1.0, NoiseFade * NoiseFade);
                float2 NoiseAmount = NoiseFade * mad(0.5, rrmin, 0.5) * (_Noise * mad(-2.0, Fadeout, 3.0) + _NoiseFresnel * (1.0 - Fresnel));
                float Noise = dot(max(0.0, lerp(lerp(1.0, c, NoiseAmount), 1.0, NoiseStrength)), float2(Smid - Stex.x, Stex.y - Smid) / (Stex.y - Stex.x));
                
                // Pixel color
                float Gradient = pow(clamp(mad(0.5, mad(Smid, i.V.y, i.Cam.y), 1.0), 0.0, 1.0), _TintFalloff);
                float4 Color = lerp(_EndTint, _StartTint, Gradient * pow(1.001 - rrmin, _TintFresnel));
                
                // final pixel value
                return clamp(0.35 * _Brightness * Raydepth * Fade * Fresnel * FresnelInvert * Noise * Fadein * Color, 0.0, _ClipBrightness);
            }

            ENDCG
        }
    }
    Fallback "KSP/Particles/Additive"

}