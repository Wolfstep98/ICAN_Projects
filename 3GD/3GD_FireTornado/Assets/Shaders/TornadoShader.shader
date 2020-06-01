Shader "Unlit/TornadoShader"
{
	Properties
	{
  		_TextureSpeed("_TextureSpeed", Vector) = (1,1,1,1)
		_Tiling("_Tiling", Vector) = (1,1,1,1)
		_Color ("_Color", Color) = (1,1,1,1)


		_Alpha("_Alpha", Range(0,1)) = 1

  		_WaveSpeed("_WaveSpeed", Float) = 2
  		_NumberOfWaves("_NumberOfWaves", Float) = 4

		_WaveMultiplicator("_WaveMultiplicator", Vector) = (1,1,1)

		_Noise ("_Noise", 2D) = "white" {}
		_MainTex ("Texture", 2D) = "white" {}

		//Fresnel Effect
		_FresnelBias("_FresnelBias", Float) = 1
		_FresnelScale("_FresnelScale", Float) = 1
		_FresnelPower("_FresnelPower", Float) = 1
		_FresnelColor ("_FresnelColor", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Transparent" 
			"Queue"="Transparent"
		}
		LOD 100

		Pass
		{            
			//Render State
            Blend SrcAlpha OneMinusSrcAlpha //Pre multipliedAlpha : Color + (1 - color.a)* Previous
            Cull Off //Off, cw, ccw
            Lighting Off 
            ZWrite On //On, Off
            ZTest LEqual

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Assets\[Tools]\Shaders\Include\snoise.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			uniform float _Alpha;

			uniform float2 _TextureSpeed;
			uniform float2 _Tiling;
			uniform float4 _Color;
			uniform sampler2D _Noise;

			uniform float _WaveSpeed;
			uniform float _NumberOfWaves;
			uniform float3 _WaveMultiplicator;

			//Fresnel Effect
			uniform float _FresnelBias;
			uniform float _FresnelScale;
			uniform float _FresnelPower;
			uniform float4 _FresnelColor;
			
			v2f vert (appdata v)
			{
				//float4 vertex = v.vertex;
				//float result = snoise(float3(v.uv.xy, _Time.y));
				//float result = clamp(cos(v.uv.y * 2 * _Time.y),0,1);
				//vertex += v.normal * result;

				float res = _Time.y * _WaveSpeed;
				float tempPosY = v.vertex.y + res;
				float res2 = 6.283185 * tempPosY;
				float res3 = _NumberOfWaves * res2;
				float res4 = sin(res3);
				float res5 = v.uv.y * res4;
				float3 res6 = v.normal * res5;
				res6 *= _WaveMultiplicator;
				float3 result = v.vertex + res6;

				v2f o;
				o.vertex = UnityObjectToClipPos(result);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//Change uvs
				float2 offset = _Time.y * _TextureSpeed.xy;
				float2 res2 = i.uv + offset; 

				//MainTexture for tornado
				float4 tex = tex2D(_Noise, res2);

				//AlphaClip
				float4 alphaTex = 1 - tex;
				float oneMinusAlpha = 1 - _Alpha;
				alphaTex *= oneMinusAlpha;

				//alphaTex = clamp(alphaTex,(0,0,0,0),(1,1,1,1));
				
				//tex.a = (all(alphaTex.a < alphaClipThreshold)) ? 0 : 1;
				//tex.a = alphaClipThreshold - alphaTex;
				clip(_Alpha - alphaTex);
				tex.rgb *= _Color.rgb;

				//Fresnel Effect
				//float3 worldSpaceNormal = normalize(mul(i.normal, unity_ObjectToWorld));
				//float3 worldSpaceViewDirection = _WorldSpaceCameraPos.xyz - mul(float4(i.vertex.xyz, 1.0), unity_ObjectToWorld).xyz;

				//float4 fresnelReflection = _FresnelBias + _FresnelScale * pow(1.0 - saturate(dot(normalize(worldSpaceViewDirection), normalize(i.normal))), _FresnelPower);

				//tex = fresnelReflection; //lerp(tex,_FresnelColor,fresnelReflection);

				return tex;
			}
			ENDCG
		}
	}
}
