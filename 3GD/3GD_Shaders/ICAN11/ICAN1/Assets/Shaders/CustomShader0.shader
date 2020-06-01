Shader "Custom/CustomShader0"
{
    Properties
    {
        _CustomColor00("Color00", COLOR) = (1, 1, 1, 1)
        _CustomColor01("Color01", COLOR) = (1, 1, 1, 1)
        _CustomColor10("Color10", COLOR) = (1, 1, 1, 1)
        _CustomColor11("Color11", COLOR) = (1, 1, 1, 1)

        _MyTex0("Texture0", 2D) = "white" {}
        _MyTex1("Texture1", 2D) = "white" {}
        _LerpFactor("Lerp factor", Range(0, 1)) = 0 
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
            #pragma fragment fragmentShader

            uniform float4 _CustomColor00;
            uniform float4 _CustomColor01;
            uniform float4 _CustomColor10;
            uniform float4 _CustomColor11;

            uniform sampler2D _MyTex0;
            uniform sampler2D _MyTex1;
            uniform float _LerpFactor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 texcoord0 : texcoord0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.uv0;
                return o;
            }
            
            float4 fragmentShader (v2f IN) : COLOR
            {
                //return _MyTex;
                float4 color0 =  _LerpFactor < 1 ? tex2D(_MyTex0, IN.texcoord0) : 0;
                float4 color1 =  _LerpFactor > 0 ?tex2D(_MyTex1, IN.texcoord0) : 0;
                return lerp(color0, color1, _LerpFactor);
            }
            ENDCG
        }
    }
}
