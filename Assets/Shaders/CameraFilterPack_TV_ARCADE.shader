﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

Shader "CameraFilterPack/TV_ARCADE" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
		_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
		_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
		_EffectOpacity ("Effect Opacity", Range(0.0, 1.0)) = 1.0 // Add this line
	}
	SubShader 
	{
		Pass
		{
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float _TimeX;
			uniform float _Distortion;
			uniform float4 _ScreenResolution;
			uniform float _EffectOpacity; // Add this line
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				half2 texcoord  : TEXCOORD0;
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
			};   
			 
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				
				return OUT;
			}

			inline float2 curve(float2 uv)
			{
				uv 		= (uv - 0.5) * 2.0;
				uv     *= 1.1;	
				uv.x   *= 1.0 + pow((abs(uv.y) * 0.2), 2.0);
				uv.y   *= 1.0 + pow((abs(uv.x) * 0.25), 2.0);
				uv 	 	= (uv / 2.0) + 0.5;
				uv 		=  uv *0.92 + 0.04;
				return uv;
			}

			fixed4 frag (v2f i) : COLOR
			{
				float2 q = i.texcoord.xy;
				float2 uv = q;
				uv = curve(uv);
				
				float3 col;
				float x =  sin(0.3*_TimeX+uv.y*21.0)*sin(0.7*_TimeX+uv.y*29.0)*sin(0.3+0.33*_TimeX+uv.y*31.0)*0.0017;

				float4 text = tex2D(_MainTex,float2(x+uv.x+0.0005,uv.y+0.0005)); // Reduce offset from 0.001 to 0.0005
				col.rgb = text.xyz + 0.05;

				text = tex2D(_MainTex,0.75*float2(x+0.0125, -0.01)+float2(uv.x+0.0005,uv.y+0.0005)); // Reduce offset from 0.025 to 0.0125
				col.r += 0.08*text.x;
				col.g += 0.05*text.y;
				col.b += 0.08*text.z;   

				col = clamp(col*0.6+0.4*col*col,0.0,1.0);

				float vig = (0.0 + 16.0*uv.x*uv.y*(1.0-uv.x)*(1.0-uv.y));
				col *= pow(vig,0.3);

				col *= float3(2.66,2.94,2.66);
				
				float scans = clamp(0.35+0.35*sin(3.5*_TimeX+uv.y*_ScreenResolution.y*1.5), 0.0, 1.0);
				
				float s = pow(scans,1.7);
				col = col*(0.4+0.7*s);

				col *= 1.0+0.01*sin(110.0*_TimeX);
				if (uv.x < 0.0 || uv.x > 1.0)
					col = 0.0;
				if (uv.y < 0.0 || uv.y > 1.0)
					col = 0.0;
				
				col *= 1.0-0.65*clamp((fmod(i.texcoord.x * _ScreenResolution.x, 1.0)-1.0), 0.0, 1.0);

				// Blend original color with effect based on _EffectOpacity
				float4 originalColor = tex2D(_MainTex, i.texcoord.xy);
				col = lerp(originalColor.rgb, col, _EffectOpacity); // Use _EffectOpacity to blend
				
				return fixed4(col, 1.0);
			}
			
			ENDCG
		}
		
	}
}


