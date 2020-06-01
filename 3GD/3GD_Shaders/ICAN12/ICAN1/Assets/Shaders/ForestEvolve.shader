Shader "Custom/ForestEvolve"
{
    Properties
    {
        _MainTex("_MainTex", 2D) = "white"
        _MouseRadius("_MouseRadius", Float) = 3
        _MinimalReproducationAge("MinimalReproducationAge", Float) = 0.5

        _SpatialFrequency("SpatialFrequency", Float) = 20
        _TimeFrequency("TimeFrequency", Float) = 1
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
            Blend Off
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #include "Assets/[Tools]/Shaders/Include/snoise.cginc"

            uniform Texture2D<float4> _MainTex;
            uniform float4 _MainTex_TexelSize;
            static const uint textureSize = 512;

            uniform float _MouseRadius;
            uniform float4 _MousePos;

            uniform float _MinimalReproducationAge;
            uniform float _SpatialFrequency;
            uniform float _TimeFrequency;

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

            float4 Status(uint2 baseTexcoord, int2 offset)
            {
                int2 ibaseTexcoord = baseTexcoord;
                int2 iOffsetedTexcoord = ibaseTexcoord + offset + textureSize;
                uint2 offsetedTexcoord = iOffsetedTexcoord;
                uint2 texcoord = offsetedTexcoord % textureSize;
                return _MainTex[texcoord];
            }
            
            float4 frag (v2f IN) : COLOR
            {
                const uint2 texcoord = IN.uv * textureSize;
                float4 status = Status(texcoord, int2(0, 0));

                float age = status.g;
                float ageEpsilon = 1.0f / 255.0;
                float nextAge = age > 0 ? age + ageEpsilon : 0;
                
                float randomNoise = snoise(float3(IN.uv * _SpatialFrequency, _Time.y * _TimeFrequency));

                if (nextAge == 0)
                {
                    float4 north = Status(texcoord, int2(0, 1));
                    float4 south = Status(texcoord, int2(0, -1));
                    float4 east = Status(texcoord, int2(1, 0));
                    float4 west = Status(texcoord, int2(-1, 0));
                    float4 neighbourAge = float4(north.g, south.g, east.g, west.g);
                    bool4 neighbourTest = neighbourAge > max(_MinimalReproducationAge, randomNoise);
                    if (any(neighbourTest))
                    {
                        nextAge = ageEpsilon;
                    }
                }

                float4 nextStatus = float4(0, nextAge > 1 ? 0 : nextAge, randomNoise, 1);

                const float lengthToMouse = length(_MousePos.xy - IN.uv) * textureSize;
                if (lengthToMouse < _MouseRadius)
                {
                    nextStatus = float4(0, 0.2, 0, 1);
                }

                
                return float4(nextStatus);
            }
            ENDCG
        }
    }
}
