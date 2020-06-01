// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Eval12"
{
    Properties
    {
        _TesselationFactor("_TesselationFactor", Float) = 2
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
            Cull Front
            Lighting Off
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma target 5.0

            #include "Assets/[Tools]/Shaders/Include/OctWrapUtils.cginc"
            #include "Assets/[Tools]/Shaders/Include/snoise.cginc"

    		#pragma vertex VS
    		#pragma hull HS
    		#pragma domain DS
            #pragma fragment frag

            uniform float _TesselationFactor;

            struct VS_Input
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct HS_Input
            {
                float3 pos  : POS;
                float2 uv   : TEXCOORD0;
            };

            struct HS_ConstantOutput
            {
                float TessFactor[3]    : SV_TessFactor;
                float InsideTessFactor : SV_InsideTessFactor;
            };

            struct HS_ControlPointOutput
            {
                float3 pos  : POS;
                float2 uv   : TEXCOORD0;
            };

            struct DS_Output
            {
                float4 pos : SV_Position;
                float2 uv : TEXCOORD0;
            };


            HS_Input VS(VS_Input Input)
            {
                HS_Input Output;
                Output.pos = Input.vertex;
                Output.uv = Input.texcoord0;
                return Output;
            }

            HS_ConstantOutput HSConstant(InputPatch<HS_Input, 3> Input)
            {
                HS_ConstantOutput Output = (HS_ConstantOutput)0;
                Output.TessFactor[0] = Output.TessFactor[1] = Output.TessFactor[2] = _TesselationFactor;
                Output.InsideTessFactor = _TesselationFactor;

                return Output;
            }

            [domain("tri")]
            [partitioning("integer")]
            [outputtopology("triangle_cw")]
            [patchconstantfunc("HSConstant")]
            [outputcontrolpoints(3)]
            HS_ControlPointOutput HS(InputPatch<HS_Input, 3> Input, uint uCPID : SV_OutputControlPointID)
            {
                HS_ControlPointOutput Output = (HS_ControlPointOutput)0;
                Output.pos = Input[uCPID].pos;
                Output.uv = Input[uCPID].uv;
                return Output;
            }

            [domain("tri")]
            DS_Output DS(HS_ConstantOutput HSConstantData,
                const OutputPatch<HS_ControlPointOutput, 3> Input,
                float3 BarycentricCoords : SV_DomainLocation)
            {
                DS_Output Output = (DS_Output)0;

                const float fU = BarycentricCoords.x;
                const float fV = BarycentricCoords.y;
                const float fW = BarycentricCoords.z;

                const float2 uv = Input[0].uv * fU + Input[1].uv * fV + Input[3].uv * fW;

                const float3 basePos = (0.5 + 0.1 * snoise(float3(abs(0.5 - uv), _Time.y))) * DecodeOct(uv);
                Output.pos = UnityObjectToClipPos(float4(basePos, 1.0));
                Output.uv = uv;

                return Output;
            }

            float4 frag (DS_Output IN) : COLOR
            {
                return float4(IN.uv, 0, 1);
            }
            ENDCG
        }
    }
}
