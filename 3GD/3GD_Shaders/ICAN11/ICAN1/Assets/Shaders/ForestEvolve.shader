Shader "Custom/ForestEvolve"
{
    Properties
    {
        _MainTex("_MainTex", 2D) = "white"
        _MouseRadius("_MouseRadius", Float) = 3
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
            ZTest Always

            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag

            uniform Texture2D<float4> _MainTex;
            uniform float4 _MainTex_TexelSize;
            static const uint textureSize = 512;

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
                float4 status = Status(texcoord, 0);
                
                float4 nextStatus = status;

                const float lengthToMouse = length(_MousePos.xy - IN.uv) * textureSize;
                if (lengthToMouse < _MouseRadius)
                {
                    nextStatus = 1;
                }

                return float4(nextStatus);
            }
            ENDCG
        }
    }
}
