Shader "Line/Naive"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        _Width("Width", Float) = 0.1
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
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            uniform float _Width;
            uniform float4 _Color;

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

            float4 frag(v2f IN) : COLOR
            {
                bool inLine = (IN.uv.x >= (0.5 - _Width)) && (IN.uv.x <= (0.5 + _Width));
                return _Color * inLine;
            }
            ENDCG
        }
    }
}
