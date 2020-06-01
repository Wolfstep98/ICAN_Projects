
Shader "Custom/LightsShader" 
{
	Properties 
    {
        _LightColor ("Light Color", Color) = (1,1,1,1)
        _DarkColor ("Dark Color", Color) = (1,1,1,1)
        
        [NoScaleOffset] _DecalMap("Decal Map", 2D) = "white" {}
	}
	SubShader 
    {
		Tags 
		{ 
		    "RenderType" = "Transparent" 
            "Queue" = "Transparent+0"  
        }
		 
		Pass
		{
		    Blend SrcAlpha OneMinusSrcAlpha
            Cull Off 
            Lighting Off 
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
		    
            uniform float4 _LightColor;
            uniform float4 _DarkColor;
            
            uniform sampler2D _DecalMap;
    
            uniform float4x4 _WorldToLocal;
		    
            struct Data
            {
                float2 Size;
                float4x4 WorldToLocal;
            };
		    
            #if defined(SHADER_API_D3D11)
            uniform uint _BufferSize;
            StructuredBuffer<Data> _Buffer;
            #endif
            
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                float4 colorFilter = _DarkColor;
                for (uint i = 0; i < _BufferSize; ++i)
                {
                    if(all(_Buffer[i].Size))
                    {
                        const float3 uv2 = mul(_Buffer[i].WorldToLocal, float4(IN.worldPos, 1)).xyz;
                        float2 resized = uv2.xy * (1/_Buffer[i].Size);
                        float2 texCoord = resized + 0.5;
                        if (all(saturate(texCoord) == texCoord))
                        {
                            float4 alphaColor = tex2D(_DecalMap, texCoord);
                            colorFilter = lerp(colorFilter, _LightColor, alphaColor.a);
                        }   
                    }    
                }
                return colorFilter;
            }
            ENDCG
		}
	}
}
