Shader "Custom/FullScreen"
{
    Properties
    {
    }
    
    SubShader
    {
        Tags
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+0" 
        }
        
        GrabPass
        {
            "_BackgroundTexture"
        }

        Pass
        {
            Blend One OneMinusSrcAlpha
            Cull Off 
            Lighting Off 
            ZWrite Off
            ZTest LEqual
        
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Include/Snoise.cginc"
            
            uniform sampler2D _BackgroundTexture;
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
                // to transform a basic unity quad (-0.5,-0.5) -> (0.5, 0.5) to a full screen quad : 
                o.vertex = float4(v.vertex.xy , 0, 0.5); // a full constructed already projected vertex 
                o.uv = v.uv;
                return o;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                return tex2D(_BackgroundTexture, float2(IN.uv.x, 1 - IN.uv.y) + 0.1 * float2(snoise(10*IN.uv.xy), snoise(10*IN.uv.yx)));
            }
            ENDCG
        }
    }
}
