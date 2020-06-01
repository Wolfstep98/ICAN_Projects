Shader "Custom/Eval9"
{
    Properties
    {
        _Speed("Speed", Float) = 1
        _Width("Width", Float) = 0.1
        _Curvature("Curvatur", Float) = 10
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
            #pragma exclude_renderers d3d11 

            #include "Assets/[Tools]/Shaders/Include/AntiAliasingUtils.cginc"

            uniform float _Width;
            uniform float _Speed;
            uniform float _Curvature;

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
                o.uv = v.texcoord0 - 0.5f;
                return o;
            }

            #define PI 3.141592653
            
            float4 frag (v2f IN) 
            {
                const float distanceUV = _Curvature * length(IN.uv);
                const float halfPixelSize = 0.5 * length(float2(ddx(distanceUV), ddy(distanceUV)));
                float lineCenter = frac(_Time.y * _Speed  +  atan2(IN.uv.y, IN.uv.x) / (2 * PI));
                const float halfLineWidth = _width;
                const float testUV = frac(distanceUV);

                lineCenter += ((testUV + halfPixelSize) < (lineCenter - halfLineWidth)) ? -1 : 0
                lineCenter += ((testUV - halfPixelSize) > (lineCenter + halfLineWidth)) ? 1 : 0;

                const float value = AlphaDoor5A(testUV - halfPixelSize, testUV + halfPixelSize, lineCenter - halfLineWidth, lineCenter + halfLineWidth);
                                 
                return float4(value.xxx, 1);
            }
            ENDCG
        }
    }
}
