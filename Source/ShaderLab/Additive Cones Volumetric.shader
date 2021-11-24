Shader "Waterfall/Additive Cones (Volumetric)"
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
        
        _ConeLength("ConeLength", Range(0, 2)) = 0.5
        _Stretch("Stretch", Range(-1, 1)) = 0
        _ExitLength("Exit Length", Range(0, 5)) = 1
        _ExitStart("Exit Start", Range(0, 1)) = 0.5
        
        [Space]

        _ConeExpansion("Cone Expansion", Range(0, 1)) = 0
        _ConeFade("Cone Fade", Range(0, 10)) = 0
        _ConeFadeStart("Cone Fade Start", Range(0, 1)) = 0
        _Smoothness("Smoothness", Range(0, 0.5)) = 0.1
        _Asymmetry("Asymmetry", Range(-0.3, 0.3)) = 0

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
        ZWrite Off
        ZTest LEqual
        Blend One One
        Cull OFF

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
            
            float _ConeLength;
            float _ExitLength;
            float _ConeExpansion;
            float _ExitStart;
            float _Smoothness;
            float _Stretch;
            float _ConeFade;
            float _ConeFadeStart;
            float _Asymmetry;

            float _Seed;
            float _SpeedX;
            float _SpeedY;
            float _TileX;
            float _TileY;

            struct vdata
            {
                float4 vertex     : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float4 color      : COLOR; // {r = wavelength number / nConesMax, g = relative pos in wavelength (0 to 1), b = angle to x-axis, a = vertex type}
            };
            
            struct v2f
            {
                float4 vertex                   : SV_POSITION;
                float3 vert                     : TEXCOORD0; // vertex coordinates in object space (x,y,z)
                float4 uvB                      : TEXCOORD1; // (u, v, B0 * Sv, B1 * Sv)
                nointerpolation float4 data     : TEXCOORD2; // (1 + r0, vertex type, ymax, ymin)
                nointerpolation float4 C        : TEXCOORD4; // (Cx, Cy, Cz, R1²)
                nointerpolation float4 par      : TEXCOORD5; // (a0, a1, C0, C1)
            };
            
            // amplitude of the wave pattern
            float expansion(float y)               {return _ConeExpansion * smoothstep(-rcp(_ConeFade + 0.001), 0.0, y + _ConeFadeStart);}
            
            // ylocal-parameter in function of t
            float ylocal(float t, float smooth)    {return -t * mad(1.0 / mad(-2.0, smooth, 1.0), abs(t), smooth / (smooth - 0.5));}
            
            // intermediate t-parameter
            float tlocal(float yloc, float smooth) {return -sign(yloc) * (smooth - sqrt(mad(mad(-2.0, smooth, 1.0), abs(yloc), smooth * smooth)));}
            
            // wave pattern expansion profile for one wavelength (yloc from -1 to 1)
            float rWave(float yloc) {
                int ASgn = sign(_Asymmetry - yloc);
                yloc = (yloc - _Asymmetry) / mad(_Asymmetry, ASgn, 1.0);
                int Sstep = step(abs(yloc), 1.0 - _Smoothness);
                yloc = mad(ASgn, 1 - Sstep, yloc);
                return mad(yloc, yloc / (_Smoothness - Sstep), Sstep);
            }
            
            // radius of the plume
            float radius(float y, float yloc, float r0) { return expansion(y) * (rWave(yloc) - 1.0) + r0; }
            
            // intensity of the pixel
            float intensity(float yavg, float ytex, float rr, float rrMax, float c, out float Falloff) {
                
                Falloff = min(1, exp2(_Falloff * (ytex + _FalloffStart)));
                float Fresnel = 1.0 - pow(rr, 0.6 + rcp(mad(_Fresnel, (1.5 - Falloff), 0.0001)));
                float fadeout = max(0.0, mad(2.0, smoothstep(-0.0001 - _FadeOut, _FadeOut, yavg + _FadeOut * (sqrt(1.0 - rr) - 1.0) + 1.0), -1.0));
                float noise = max(0.0, lerp(lerp(1.0, c, mad(0.6, rr, 0.4) * (_Noise * mad(-0.3, fadeout, 1.3) + _NoiseFresnel * (1.0 - Fresnel))), 1.0, Falloff));
                
                return Falloff * Fresnel * noise * fadeout
                        * smoothstep(-0.0001, _FadeIn, -yavg)                                                                                                                // fade in
                        * (1.0 + _FresnelInvert / (_FresnelInvert + 0.3) * (pow((rr + mad(0.2, rrMax, -0.2)) / mad(0.2, rrMax, 0.8), mad(0.3, _FresnelInvert, 1.0)) - 1.0)); // inverted fresnel
            }
            
            v2f vert(vdata i) {
                v2f o;
                
                int nConesMax = 12;                                 // Max wavelength amount (mesh dependent)
                int nVertInterp = 10;                               // Wavelength vertex interpolation amount (mesh dependent)
                
                float waveStart = _ExitStart - floor(_ExitStart);   // bound wave start
                o.data.y = 0.1 * round(i.color.a * 10.0);           // vertex type
                float wlidx = round(i.color.r * nConesMax);         // get integer value of wavelength number
                float vidx = round(i.color.g * nVertInterp);        // vertex index in the wavelength (0 to nVertInterp)
                
                // bound base wavelength and calculate final wavelength number iEnd
                float stretch = abs(_Stretch) < 0.001 ? 0 : _Stretch;
                int iEnd;
                float wavelength;
                if (stretch == 0) {
                    wavelength = max(_ConeLength, 1 / (nConesMax - 1 + _ExitLength * (1.0 - waveStart)));
                    iEnd = max(0, ceil(1.0 + 1.0 / wavelength - _ExitLength * (1.0 - waveStart)) - 1.0);
                }
                else {
                    float wlMin = 1.0 / ((1.0 - exp2(stretch * (nConesMax - 1.0))) / (1.0 - exp2(stretch)) + _ExitLength * (1.0 - waveStart));
                    wavelength = max(_ConeLength, wlMin);
                    iEnd = _ConeLength <= wlMin ? nConesMax - 1 : max(0, ceil(log2(1.0 + (1.0 - exp2(stretch)) * (-1.0 / wavelength + _ExitLength * (1.0 - waveStart))) / stretch + 1.0) - 1);
                }
                
                // discard vertices from unused wavelength sections
                if ((wlidx > iEnd && (o.data.y < 0.15)) || (wlidx > iEnd + 1 && o.data.y > 0.15)) {
                    o.vertex = float4(0.0 / 0.0 * float3(1, 1, 1), 1);
                    o.vert = float3(0, 0, 0);
                    o.uvB = float4(0, 0, 0, 0);
                    o.data = float4(0, 0, 0, 0);
                    o.C = float4(0, 0, 0, 0);
                    o.par = float4(0, 0, 0, 0);
                }
                
                // continue with only the useful vertices
                else{
                    // reassign the out-of-bounds vertices to the final wavelength section
                    if (wlidx > iEnd) {
                        wlidx = iEnd;
                        vidx = nVertInterp;
                        o.data.y = 0.4;
                    }
                    
                    float vertDistSmooth = 1.2;                                                     // smoothness of the distribution of vertexes (1 is minimum)
                    float y3loc = - 0.9 + _Smoothness;
                    float wl0 = _ExitLength * wavelength;                                           // exit wavelength
                    o.data.r = 1.0 + expansion(0.0) * (1.0 - rWave(1.0 - 2.0 * waveStart));         // radius offset + 1 
                    o.C.xyz = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1.0)).xyz;  // Camera position in object space
                                        
                    // calculate the wavelength section values
                    float wl;
                    float yc;
                    if (wlidx == 0) {
                        wl = wl0;
                        yc = wl * waveStart;
                    }
                    else {
                        wl = wavelength * exp2(stretch * (wlidx - 1.0));
                        if (stretch == 0) {
                            yc = -wl0 * (1.0 - waveStart) - wavelength * (wlidx - 1.0);
                        }
                        else {
                            yc = -wl0 * (1.0 - waveStart) - (wavelength - wl) / (1.0 - exp2(stretch));
                        }
                    }
                    
                    float wlPrev = wlidx < 1.5 ? wl0 : wl * exp2(-stretch);
                    float ycPrev = yc + 0.5 * wlPrev;
                    float3 ycone = float3(yc + wlPrev, yc, yc - wl);
                    yc -= 0.5 * wl;      
                    float tStart = wlidx == 0 ? tlocal(-2.0 * yc / wl, vertDistSmooth) : -1.0;
                    float tEnd = wlidx == iEnd ? tlocal(2.0 * (-1.0 - yc) / wl, vertDistSmooth) : 1.0;
                    float tpos = lerp(tStart, tEnd, vidx / nVertInterp);
                    
                    // vertex position
                    float yloc = ylocal(tpos, vertDistSmooth); // vertex y-local position
                    o.vert.y = yc + 0.5 * yloc * wl;
                    float Radius = radius(o.vert.y, yloc, o.data.r);
                    o.vert.xz = i.vertex.xz * Radius;                                           // vertex xz-position
                    o.vertex = UnityObjectToClipPos(float4(o.vert, 1));                         // vertex clip position
                    
                    // ray values
                    float3 V = o.vert - o.C.xyz;
                    float CV = dot(o.C.xz, V.xz);
                    
                    // Radii² at the 3 sample locations for the ellipsoid
                    float3 RR = float3(o.data.r - float2(expansion(ycone.y), expansion(ycone.z)), radius(yc + 0.5 * wl * y3loc, y3loc, o.data.r));
                    RR *= RR;
                    o.C.w = RR.x;
                    
                    // parameters of the ellipsoids (R² = a*yloc² + b*yloc + c)
                    float b = 0.5 * (RR.x - RR.y);
                    o.par.y = (RR.z - b * y3loc - dot(RR.xy, 0.5)) / mad(y3loc, y3loc, -1.0); // a1
                    float c = dot(RR.xy, 0.5) - o.par.y;
                    
                    // Quadratic intersection equation
                    o.uvB.w = mad(o.par.y, 8 * V.y * (o.C.y - yc), 2 * wl * mad(b, V.y, -wl * CV)); // B1
                    o.par.w = mad(o.C.y - yc, mad(o.C.y - yc, 4 * o.par.y, 2 * b * wl), wl * wl * (c - dot(o.C.xz, o.C.xz))); // C1
                    
                    // add values for the previous wavelength section if necessary:
                    if (o.data.y > 0.15 && o.data.y < 0.25) {
                        
                        // Radii² at the 3 sample locations for the ellipsoid
                        RR =  float3(o.data.r - expansion(ycone.x), RR.x, radius(ycPrev + 0.5 * wlPrev * y3loc, y3loc, o.data.r));
                        RR.xz *= RR.xz;
                        
                        // parameters of the ellipsoids (R² = a*yloc² + b*yloc + c)
                        float b = 0.5 * (RR.x - RR.y);
                        o.par.x = (RR.z - b * y3loc - dot(RR.xy, 0.5)) / mad(y3loc, y3loc, -1.0); // a0
                        float c = dot(RR.xy, 0.5) - o.par.x;
                        
                        // Quadratic intersection equation
                        o.uvB.z = mad(o.par.x, 8 * V.y * (o.C.y - ycPrev), 2 * wlPrev * mad(b, V.y, -wlPrev * CV)); // B0
                        o.par.z = mad(o.C.y - ycPrev, mad(o.C.y - ycPrev, 4 * o.par.x, 2 * b * wlPrev), wlPrev * wlPrev * (c - dot(o.C.xz, o.C.xz))); // C0
                        
                        // bounding y-values for the 2 adjacent cones
                        o.data.zw = ycone.xz;
                    }
                    else {
                        o.data.zw = ycone.yz;
                        o.uvB.z = 0.0;
                        o.par.x = 0.0;
                        o.par.z = 0.0;
                    }
                    
                    // uv map
                    if (o.data.y < 0.15) {
                        float umir = _SpeedX * _Time.x;
                        float u = 2 * abs(i.uv_MainTex.x - umir - floor(i.uv_MainTex.x - umir + 0.5)) + umir;
                        o.uvB.xy = (float2(u, lerp(0.5, 1.0, 1.0 + o.vert.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                    }
                    else {
                        o.uvB.xy = float2(0.0, 0.0);
                    }
                }
                return o;
            }


            float4 frag(v2f i, float facing : VFACE) : SV_Target
            {
                // construct camera view parameters
                float3 V = i.vert - i.C.xyz;                    // view ray direction
                float Sv = length(V);                           // distance to fragment
                V /= Sv;                                        // normalize view ray direction
                i.uvB.zw /= Sv;                                 // normalize B-parameters
                float VV = dot(V.xz, V.xz);
                
                bool Vdir = V.y >= 0;                                               // view ray y-direction is positive?
                bool face = i.data.y > 0.35 ? facing < 0 : facing >= 0;             // is the triangle viewed from outside?
                bool Vy0 = abs(V.y) < 0.005;                                        // viewing plume approximately from the side?
                float2 S01 = (-float2((float)Vdir, (float)!Vdir) - i.C.y) / V.y;    // S-values for y = 0 & -1 (min max)
                
                
                float4 Pixel;
                if (i.data.y < 0.15 || i.data.y > 0.25) {
                    
                    float wl = i.data.z - i.data.w;
                    float yc = dot(i.data.zw, 0.5);
                    
                    // intersection
                    float A = mad(V.y, 4.0 * V.y * i.par.y, -wl * wl * VV);
                    float D = mad(i.uvB.w, i.uvB.w, - 4.0 * A * i.par.w);
                    float2 Si = (-i.uvB.w + float2(1.0, -1.0) * sqrt(max(D, 0.000001))) / (2 * A);
                    
                    float Smin;
                    float Smid;
                    if (Vy0) {
                        // approximate by cylinder intersection
                        Smin = max(0.0, clamp(-dot(i.C.xz, V.xz) / VV, S01.x, S01.y));
                        Smid = Smin;
                    }
                    else {
                        // deepest penetration point
                        Smin = max(0.0, clamp(dot(Si, 0.5), S01.x, S01.y));
                    
                        // S-range of the front ray section
                        float2 Scone = (clamp((Vdir ? i.data.wz : i.data.zw), -1.0, 0.0) - i.C.y) / V.y;
                        float2 Sextr = lerp(clamp(Si.yx, Scone.x, Scone.y), Scone, 0.5);
                        Smid = max(0.0, clamp(Smin, Sextr.x, Sextr.y));
                    }
                    float2 Srange = face ? float2(Sv, max(Sv, Smid)) : float2(min(Sv, Smid), Sv);
                    
                    // texture
                    float c;
                    if (i.data.y < 0.15) {
                        c = length(tex2D(_MainTex, i.uvB.xy).rgb);
                    }
                    else {
                        float3 Ptex = mad(V, Si[(int)!face], i.C.xyz);
                        float umir = _SpeedX * _Time.x;
                        float u = 0.1591549431 * atan2(Ptex.z, Ptex.x) + step(Ptex.z, 0.0);
                        u = 2 * abs(u - umir - floor(u - umir + 0.5)) + umir;
                        i.uvB.xy = (float2(u, lerp(0.5, 1.0, 1.0 + Ptex.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                        c = length(tex2D(_MainTex, i.uvB.xy).rgb);
                        i.vert.y = Ptex.y;
                    }
                    
                    // relative penetration depth
                    float3 P = mad(Smin, V, i.C.xyz);
                    float RRw = radius(P.y, clamp(2.0 * (P.y - yc) / wl, -1.0, 1.0), i.data.r);
                    RRw *= RRw;
                    float rr = min(dot(P.xz, P.xz) / RRw, 1.0);
                    float rrMax = i.data.r * i.data.r / RRw;
                    
                    // Pixel color
                    float yavg = mad(dot(Srange, 0.5), V.y, i.C.y);
                    float Falloff;
                    float Intensity = intensity(yavg, i.vert.y, rr, rrMax, c, Falloff);
                    float4 Color = lerp(_EndTint, _StartTint, pow(Falloff, _TintFalloff) * pow(1.001 - rr, _TintFresnel));
                    float Raydepth = (Srange.y - Srange.x);
                    Pixel = Intensity * Color * Raydepth;  
                }
                else if (Vy0) {Pixel = 0;}
                else {
                    // flip so that first wavelength is in front of second
                    if (Vdir) {
                        i.uvB.zw = i.uvB.wz;
                        i.par = i.par.yxwz;
                        i.data.zw = i.data.wz;
                    }
                    
                    // cylinder intersections
                    float detCV = dot(V.xz, float2(i.C.z, -i.C.x));
                    float2 Scyl = (-dot(i.C.xz, V.xz) + float2(-1.0, 1.0) * sqrt(max(0.0, mad(i.C.w, VV, -detCV * detCV)))) / VV;
                    
                    // Front ray section
                    float wl = abs(i.data.z - i.vert.y);
                    float yc = 0.5 * (i.data.z + i.vert.y);
                    float2 Si;
                    
                    // intersection
                    float A = mad(V.y, 4.0 * V.y * i.par.x, -wl * wl * VV);
                    float Smin = -i.uvB.z / (2 * A);
                    float D = mad(i.uvB.z, i.uvB.z, - 4.0 * A * i.par.z);
                    Si.x = Smin + sqrt(max(D, 0.000001)) / (2 * A);
                    
                    // deepest penetration point
                    Smin = max(0.0, clamp(Smin, S01.x, S01.y));
                    
                    // S-range of the front ray section
                    float2 Scone = (clamp(float2(i.data.z, i.vert.y), -1.0, 0.0) - i.C.y) / V.y;
                    float Smid = 0.5 * (Sv + clamp(Si.x, Scone.x, Scone.y));
                    float2 Srange = max(0.0, float2(Smid, Sv));
                    
                    // relative penetration depth
                    float3 P = mad(Smin, V, i.C.xyz);
                    float RRw = radius(P.y, clamp(2.0 * (P.y - yc) / wl, -1.0, 1.0), i.data.r);
                    RRw *= RRw;
                    float rr = min(dot(P.xz, P.xz) / RRw, 1.0);
                    float rrMax = i.data.r * i.data.r / RRw;
                    
                    // texture at back cylinder intersection
                    P = mad(V, Scyl.y, i.C.xyz);
                    float umir = _SpeedX * _Time.x;
                    float u = 0.1591549431 * atan2(P.z, P.x) + step(P.z, 0.0);
                    u = 2 * abs(u - umir - floor(u - umir + 0.5)) + umir;
                    i.uvB.xy = (float2(u, lerp(0.5, 1.0, 1.0 + P.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                    float c = length(tex2D(_MainTex, i.uvB.xy).rgb);
                    
                    // Pixel color of front section
                    float2 yavg = mad(dot(Srange, 0.5), V.y, i.C.y);
                    float Falloff;
                    float Intensity = intensity(yavg, P.y, rr, rrMax, c, Falloff);
                    float4 Color = lerp(_EndTint, _StartTint, pow(Falloff, _TintFalloff) * pow(1.001 - rr, _TintFresnel));
                    float Raydepth = (Srange.y - Srange.x);
                    Pixel = Intensity * Color * Raydepth;
                    
                    // Back ray section
                    wl = abs(i.data.w - i.vert.y);
                    yc = 0.5 * (i.data.w + i.vert.y);
                    
                    // intersection
                    A = mad(V.y, 4.0 * V.y * i.par.y, -wl * wl * VV);
                    Smin = -i.uvB.w / (2 * A);
                    D = mad(i.uvB.w, i.uvB.w, - 4.0 * A * i.par.w);
                    Si.y = Smin - sqrt(max(D, 0.000001)) / (2 * A);
                    
                    // deepest penetration point
                    Smin = max(0.0, clamp(Smin, S01.x, S01.y));
                    
                    // S-range of the back ray section
                    Scone = (clamp(float2(i.vert.y, i.data.w), -1.0, 0.0) - i.C.y) / V.y;
                    Smid = 0.5 * (Sv + clamp(Si.y, Scone.x, Scone.y));
                    Srange = max(0.0, float2(Sv, Smid));
                    
                    // relative penetration depth
                    P = mad(Smin, V, i.C.xyz);
                    RRw = radius(P.y, clamp(2.0 * (P.y - yc) / wl, -1.0, 1.0), i.data.r);
                    RRw *= RRw;
                    rr = min(dot(P.xz, P.xz) / RRw, 1.0);
                    rrMax = i.data.r * i.data.r / RRw;
                    
                    // texture at front cylinder intersection
                    P = mad(V, max(0.0, Scyl.x), i.C.xyz);
                    u = 0.1591549431 * atan2(P.z, P.x) + step(P.z, 0.0);
                    u = 2 * abs(u - umir - floor(u - umir + 0.5)) + umir;
                    i.uvB.xy = (float2(u, lerp(0.5, 1.0, 1.0 + P.y)) + _Seed) * float2(_TileX, _TileY) + float2(-_SpeedX, _SpeedY * _TileY) * _Time.x;
                    c = length(tex2D(_MainTex, i.uvB.xy).rgb);
                    
                    // Pixel color of back section
                    yavg = mad(dot(Srange, 0.5), V.y, i.C.y);
                    Intensity = intensity(yavg, P.y, rr, rrMax, c, Falloff);
                    Color = lerp(_EndTint, _StartTint, pow(Falloff, _TintFalloff) * pow(1.001 - rr, _TintFresnel));
                    Raydepth = (Srange.y - Srange.x);
                    Pixel += Intensity * Color * Raydepth;
                }
                return clamp(0.5 * _Brightness * length(float3(4.0 * _LengthBrightness * V.y, V.xz)) * Pixel, 0.0, _ClipBrightness);
            }

            ENDCG
        }
    }
    Fallback "KSP/Particles/Additive"

}