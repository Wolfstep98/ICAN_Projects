Shader "Line/MultiSamplingWangHash"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        _Width("Width", Float) = 0.25
        _StepCount("StepCount", Float) = 3
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
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #include "Assets/[Tools]/Shaders/Include/Wang_Hash.cginc"
            uniform float _Width;
            uniform float4 _Color;
            uniform float _StepCount;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord0;
                return o;
            }

            float2 GetSampleDelta(uint index)
            {
                const uint wangHash = wang_hash(index);
                return float2((wangHash & 0xFFFF) / 65535.0f, ((wangHash >> 16) & 0xFFFF) / 65535.0f) - 0.5f;
            }

            float SampleValue(float2 texCoords)
            {
                const bool inLine = (texCoords.x >= (0.5 - _Width)) && (texCoords.x <= (0.5 + _Width));
                return inLine ? 1 : 0;
            }

            float4 frag(v2f IN) : COLOR
            {
                const float2 ddxUV = ddx(IN.uv);
                const float2 ddyUV = ddy(IN.uv);
                const uint stepCount = clamp(_StepCount, 1, 10000);
                float hits = 0;
                for (uint i = 0; i < stepCount; ++i)
                {
                    const float2 deltaXY = GetSampleDelta(i); 
                    const float2 localDeltaUVxUVy = ddxUV * deltaXY.x + ddyUV * deltaXY.x;
                    hits += SampleValue(IN.uv + localDeltaUVxUVy);
                }
                
                const float totalHits = stepCount;
                const float proportion = float(hits) / totalHits;
                return _Color * proportion;
            }
            ENDCG
        }
    }
}
