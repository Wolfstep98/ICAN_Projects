Shader "Custom/Eval1"
{
    Properties
    {
        _MainTex("Main", 2D) = "white"
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

            uniform sampler2D _MainTex;

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
                o.vertex = UnityObjectToClipPos(v.vertex)
                o.uv = v.texcoord0;
                return o;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                const float4 texColor = tex2D(_MainTex, IN.uv);
                return texColor
            }
            ENDCG
        }
    }
}
