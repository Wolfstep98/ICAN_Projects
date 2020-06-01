Shader "Custom/Eval4"
{
    Properties
    {
        _Frequency("Frequency", Vector) = (1, 1, 1, 1)
        _Intensity("Intensity", Float) = 1
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

            #include "Assets/[Tools]/Shaders/Include/snoise.cginc"

            uniform float4 _Frequency;
            uniform float _intensity;
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
                const float noise = abs(snoise(float3(IN.uv, _Time.y) * _Frequency.xyz)) * _Intensity;
                return float4(noise.xxx, 1);
            }
            ENDCG
        }
    }
}
