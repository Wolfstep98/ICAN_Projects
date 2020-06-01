Shader "Custom/ArrayReader"
{
    Properties
    {
        [NoScaleOffset] _MyTex("_MainTex", 2DArray) = "white" {}
        _Index("Index", int) = 0
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
            UNITY_DECLARE_TEX2DARRAY( _MyTex);
            uniform uint _Index;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                return UNITY_SAMPLE_TEX2DARRAY(_MyTex,float3(IN.uv, _Index));
            }
            ENDCG
        }
    }
}
