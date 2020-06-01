// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Eval10"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "grey"
        _Count("Count", Float) = 128
    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent-1"
        }

        Pass
        {
            Blend One OneMinusSrcAlpha
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Assets/[Tools]/Shaders/Include/wang_hash.cginc"
            sampler2D _MainTex;
            static const uint textureSize = 512;
            uniform float _Count;

            uniform float _MouseRadius;
            uniform float4 _MousePos;

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
                
                

                const uint2 texcoord = IN.UV * textureSize;
                const uint pixelIndex = texcoord.x + texcoord.y * textureSize + _Time.x;
                const uint randomInt = wang_hash(pixelindex);
                bool alive = length(IN.UV - 0.5) < 0.1 && (randomInt & 0xFF) > (uint)_Count;

                const float lengthToMouse = length(_MousePos.xy - IN.uv) * textureSize;
                if (lengthToMouse < _MouseRadius)
                {
                    alive = float2(0, 0.9);
                }

                return float4(alive.xxx, 1);
            }
            ENDCG
        }
    }
}
