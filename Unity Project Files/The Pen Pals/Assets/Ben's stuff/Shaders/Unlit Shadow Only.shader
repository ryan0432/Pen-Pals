// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit With Shadows" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
	}
	SubShader {
		 Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Transparent"}
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				
				struct v2f
				{
					float4	pos			: SV_POSITION;
					float2	uv			: TEXCOORD0;
					LIGHTING_COORDS(1,2)
				};

				float4 _MainTex_ST;
				float4 _Color;

				v2f vert (appdata_tan v)
				{
					v2f o;
					
					o.pos = UnityObjectToClipPos( v.vertex);
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f i) : COLOR
				{
					fixed atten = LIGHT_ATTENUATION(i);	// Light attenuation + shadows.
					//fixed atten = SHADOW_ATTENUATION(i); // Shadows ONLY.

					fixed4 c = _Color;
					c = c * atten;
					c.a = 1.0f - c.a;
							
				/*	if(atten >= 0.95)
						c.a = 0.0f;*/

					return float4(c.r * 0.25f, c.g * 0.25f, c.b * 0.25f, c.a);
				}
			ENDCG
		}

		Pass {
			Tags {"LightMode" = "ForwardAdd"}
			Blend One One
			ZWrite Off
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdadd_fullshadows
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				
				struct v2f
				{
					float4	pos			: SV_POSITION;
					float2	uv			: TEXCOORD0;
					LIGHTING_COORDS(1,2)
				};

				float4 _MainTex_ST;
				float4 _Color;

				v2f vert (appdata_tan v)
				{
					v2f o;
					
					o.pos = UnityObjectToClipPos( v.vertex);
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				sampler2D _MainTex;

				fixed4 frag(v2f i) : COLOR
				{
					//fixed atten = LIGHT_ATTENUATION(i);	// Light attenuation + shadows.
					fixed atten = SHADOW_ATTENUATION(i); // Shadows ONLY.

					fixed4 c = _Color;
					c = c * atten;
					c.a = 0.2;
							
					if(atten >= 0.75)
						c.a = 0.0f;

					return float4(c.r * 0.25f, c.g * 0.25f, c.b * 0.25f, c.a);
				}
			ENDCG
		}
	}
	FallBack "VertexLit"
}