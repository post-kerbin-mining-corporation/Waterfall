Shader "Waterfall/Billboard (Additive)"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_StartTint("StartTint", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags { "Queue" = "Transparent"  "IgnoreProjector" = "True" "RenderType" = "Transparent"  }
		Blend One One
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog { Color(0,0,0,0) }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _StartTint;

			v2f vert(appdata_base v)
			{
				v2f o;

				// extract world pivot position from object to world transform matrix
				float3 worldPos = unity_ObjectToWorld._m03_m13_m23;

				// extract x and y scale from object to world transform matrix
				float2 scale = float2(
					length(unity_ObjectToWorld._m00_m10_m20),
					length(unity_ObjectToWorld._m01_m11_m21)
					);

				// transform pivot position into view space
				float4 viewPos = mul(UNITY_MATRIX_V, float4(worldPos, 1.0));

				// apply transform scale to xy vertex positions
				float2 vertex = v.vertex.xy * scale;

				// add vertex positions to view position pivot
				viewPos.xy += vertex;

				// transform into clip space
				o.pos = mul(UNITY_MATRIX_P, viewPos);

				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return clamp(col * _StartTint * 2.0f, 0, 50);
			}
			ENDCG
		}
	}
}