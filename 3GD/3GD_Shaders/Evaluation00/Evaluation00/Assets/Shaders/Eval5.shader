Shader "Custom/Eval5"
{
    Properties
    {
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
            Blend One OneMinusSrcAlpha
            Cull Off 
            Lighting Off 
            ZWrite On
            ZTest LEqual
        
            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #include "Assets/[Tools]/Shaders/Include/HSVUtils.cgin"
            
            #define PI 3.141592
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord0;
                return o;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                const float2 centeredUV = 2.0f * (IN.uv - 0.5f);
                const float anglein01 = atan2(centeredUV.y, centeredUV.x) / ( 2.0f * PI);
                const float3 color = hsv2rgb(float3(anglein01, length(centeredUV), 0.5));
                return float4(color, 1);
            }
            ENDCG
        }
    }
}
