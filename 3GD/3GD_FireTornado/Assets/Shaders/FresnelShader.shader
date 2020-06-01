Shader "Unlit/FresnelShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        //Debug
        _FakePos("_FakePos", Vector) = (0,0,0)

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
                float4 reflection : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

            uniform float3 _FakePos;

			//Fresnel Effect
			uniform float _FresnelBias;
			uniform float _FresnelScale;
			uniform float _FresnelPower;
			uniform float4 _FresnelColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				UNITY_TRANSFER_FOG(o,o.vertex);

				//Fresnel Effect
				float3 posWorld = mul(unity_ObjectToWorld, v.vertex);
				float3 worldSpaceNormal = normalize(mul(unity_ObjectToWorld, v.normal));

				float3 I = normalize(posWorld - _FakePos);
                float4 dotProduct = dot(I, worldSpaceNormal);
                //o.reflection = float4(dotProduct,dotProduct.r);

				float4 fresnelReflection = _FresnelBias + _FresnelScale * pow((1.0 + dotProduct), _FresnelPower);
                o.reflection = fresnelReflection;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//float4 tex = tex2D(_MainTex, i.uv);
				return lerp(float4(0,0,0,0),_FresnelColor,i.reflection);
			}
			ENDCG
		}
	}
}
