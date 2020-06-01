// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Custom/ShowSampling"
{
Properties
{
    _Range("Range", Float) = 1
    _Size("Size", Float) = 0.1
    _Width("_Width", Float) = 0.5
    _MainTex("Main Tex", 2D) = "white" {}
    [Enum(Regular, 0, WangHash, 1, Halton, 2, Hammersley, 3)]_IntegrationMethod("Integration Method", Float) = 0
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
        #pragma target 4.0
        #pragma vertex vert
        #pragma fragment frag
        #include "Assets/[Tools]/Shaders/Include/Wang_Hash.cginc"
        #include "Assets/[Tools]/Shaders/Include/Halton.cginc"
        #include "Assets/[Tools]/Shaders/Include/Hammersley.cginc"
            
        #define ID_PER_PRIMITIVE 6

        #ifndef PI
        #define PI 3.14159265358979
        #endif

        uniform float _Range;
        uniform float _Size;
        uniform float _Width;
        uniform sampler2D _MainTex;
        uniform int _IntegrationMethod;
        uniform uint _VertexCount;

        struct v2f
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
        };

        float2 GetCorner(uint index)
        {
        #if 0
            const float2 corners[ID_PER_PRIMITIVE] = { float2(-0.5, -0.5), float2(-0.5, 0.5), float2(0.5, 0.5), float2(0.5, 0.5), float2(0.5, -0.5), float2(-0.5, -0.5) };
            return corners[index % ID_PER_PRIMITIVE];
        #else
            return float2((index >= 2 && index <= 4) ? 0.5 : -0.5, (index >= 1 && index <= 3) ? 0.5 : -0.5);
        #endif
        }

        float2 GetSample01RegularGrid(uint index, uint quadCount)
        {
            const uint sqrtStepCount = ceil(sqrt(quadCount));
            uint x = index % sqrtStepCount;
            uint y = index / sqrtStepCount;
            return (float2(x + 0.5, y + 0.5) / sqrtStepCount); // *((sqrtStepCount - 1) / (float)sqrtStepCount);
        }

        float2 GetSample01Halton(uint index)
        {
            return float2(GetHaltonValue(index + 1, 2), GetHaltonValue(index + 2, 3));
        }

        float2 GetSample01WangHash(uint index)
        {
            const uint wangHash = wang_hash(index + 1);
            return float2((wangHash & 0xFFFF) / 65535.0f, ((wangHash >> 16) & 0xFFFF) / 65535.0f);
        }

        float2 GetSample01Hammersley(uint index, uint stepCount)
        {
            return Hammersley2d(index, stepCount);
        }

        float2 GetSample01(uint index, uint stepCount)
        {
            switch (_IntegrationMethod)
            {
            case 0: return GetSample01RegularGrid(index, stepCount);
            case 1: return GetSample01WangHash(index);
            case 2: return GetSample01Halton(index);
            case 3: return GetSample01Hammersley(index, stepCount);
            default: return 0;
            }
        }

        v2f vert (uint id : SV_VertexID)
        {
            v2f o;
            
            const uint quadIndex = id / ID_PER_PRIMITIVE;
            const uint vertexIndex = id % ID_PER_PRIMITIVE;

            const uint quadCount = _VertexCount / ID_PER_PRIMITIVE;
            float2 sample01 = GetSample01(quadIndex, quadCount);
            const float dsample01 = 1 / sqrt(quadCount);
            const float2 corner = GetCorner(vertexIndex);
            const float3 localPos = float3(sample01.xy, 0) * _Range;
            const float3 worldPos = mul(unity_ObjectToWorld, float4(localPos, 1)).xyz;
            const float3 posToCamera = normalize(_WorldSpaceCameraPos - worldPos);
            const float3 orthoDirection = float3(0, 1, 0);
            const float3 orthoDirection2 = float3(1, 0, 0);
            const float size = _Size / sqrt(quadCount);
            const float3 basePos3D = corner.x * orthoDirection * size +
                                     corner.y * orthoDirection2 * size;
            float4 vertex = float4(worldPos + basePos3D, 1);
            o.vertex = mul(UNITY_MATRIX_VP, vertex);
            o.uv = 0.5f + corner;
            o.uv2 = sample01;
            return o;
        }
            
        float4 frag (v2f IN) : COLOR
        {
            const float4 color = float4(64 / 255.0, 8 / 255.0, 1.0 / 255.0, 0);
            const float distanceToCenter = length(IN.uv - 0.5) * 2;
            const float ddxu = ddx(distanceToCenter);
            const float ddyu = ddy(distanceToCenter);
            const float pixelSize = length(float2(ddxu, ddyu));
            const float minU = 0;
            const float maxU = _Width;
            const float minPixelInLine = clamp(distanceToCenter - 0.5f * pixelSize, minU, maxU);
            const float maxPixelInLine = clamp(distanceToCenter + 0.5f * pixelSize, minU, maxU);
            const float extentInLine = max(maxPixelInLine - minPixelInLine, 0);
            const float proportion = extentInLine / pixelSize;
            return 1; //color * proportion;
        }
        ENDCG
    }
}
}
