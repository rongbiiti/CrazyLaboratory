Shader "Hidden/RadialBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SampleCount("Sample Count", Range(4, 16)) = 8
		_Strength("Strength", Range(0.0, 1.0)) = 0.5
	}
		SubShader
		{
			Cull Off
			ZWrite Off
			ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex   : POSITION;
					float2 uv       : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv       : TEXCOORD0;
					float4 vertex   : SV_POSITION;
				};

				v2f vert (appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				half _SampleCount;
				half _Strength;

				fixed4 frag (v2f i) : SV_Target
				{
					half4 col = 0;
					// UVを-0.5～0.5に変換
					half2 symmetryUv = i.uv - 0.5;
					// 外側に行くほどこの値が大きくなる(0～0.707)
					half distance = length(symmetryUv);
					for (int j = 0; j < _SampleCount; j++) {
						// jが大きいほど、画面の外側ほど小さくなる値
						float uvOffset = 1 - _Strength * j / _SampleCount * distance;
						// jが大きくなるにつれてより内側のピクセルをサンプリングしていく
						// また画面の外側ほどより内側のピクセルをサンプリングする
						col += tex2D(_MainTex, symmetryUv * uvOffset + 0.5);
					}
					col /= _SampleCount;
					return col;
				}

				ENDCG
			}
		}
}