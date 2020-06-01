Shader "Custom/Eval8"
{
    Properties
    {
        _NumberAtlas("Main", 2D) = "white"
        _Number("Value", Float) = 1
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
            Blend One OneMinusSrcalpha
            Cull Off
            Lighting Off
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            #include "Assets/[Tools]/Shaders/Include/AntiAliasingUtils.cginc"

            uniform sampler2D _NumberAtlas;
            uniform float4 _NumberAtlas_TexelSize;
            uniform float _Number;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float proportion : TEXCOORD1;
            };

            v2f vert (appData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                const float baseNumber = _Number + _Time.y;
                const float numberIn010 = frac(baseNumber / 10) * 10;
                const uint number0 = (uint)trunc(numberIn010) % 10;
                const uint number1 = (number0 + 1) % 10;
                const uint numberPerLine = 4;
                const float2 conventionalUVMin0 = float2(number0 % numberPerLine, number0 / numberPerLine) / (float)numberPerLine;
                const float2 conventionalUV0 = conventionalUVMin0 + float2(v.texcoord0.x, 1 - v.texcoord0.y) / (float)numberPerLine;
                const float2 conventionalUVMin1 = float2(number1 % numberPerLine, number1 / numberPerLine) / (float)numberPerLine;
                const float2 conventionalUV1 = conventionalUVMin1 + float2(v.texcoord0.x, 1 - v.texcoord0.y) / (float)numberPerLine;
                o.uv0 = float2(conventionalUV0.x, 1 - conventionalUV0.y);
                o.uv1 = float2(conventionalUV1.x, 1 - conventionalUV1.y);
                o.proportion = numberIn010 - number0;
                return o;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                const float2 ddxUVInTex0 = ddx(IN.uv0) / (_NumberAtlas_TexelSize.xy);
                const float2 ddyUVInTex0 = ddy(IN.uv0) / (_NumberAtlas_TexelSize.xy);
                const float halfPixelSize = 0.5 * length(float2(length(float2(ddxUVInTex0.x, ddyUVInTex0.x)), lenght(float2(ddxUVInTex0.y, ddyUVInTex0.y))));

                const float pixelDistance = 10;
                const float4 numberTex0 = tex2D(_NumberAtlas, IN.uv0);
                const float4 numberTex1 = tex2D(_NumberAtlas, IN.uv1);
                const float signedDistance0 = 2 * (numberTex0.x - 0.5) * pixelDistance;
                const float signedDistance1 = 2 * (numberTex1.x - 0.5) * pixelDistance;
                const float signedDistance = lerp(signedDistance0, signedDistance1, IN.proportion);
                const float value = AlphaDoor5A(signedDistance - halfPixelSize, signedDistance + halfPixelSize, 0, pixelDistance);
                
                return float4(value.xxx, 1);
            }
            ENDCG
        }
    }
}
