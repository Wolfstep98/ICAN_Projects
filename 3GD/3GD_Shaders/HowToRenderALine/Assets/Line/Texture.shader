Shader "Line/Texture"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        _StepTexture("StepTexture", 2D ) = "white"
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
            uniform float4 _Color;
            uniform sampler2D _StepTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
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
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f IN) : COLOR
            {
                const float proportion = tex2D(_StepTexture, float2(IN.uv.x, 0.5));
                return _Color * proportion;
            }
            ENDCG
        }
    }
}
