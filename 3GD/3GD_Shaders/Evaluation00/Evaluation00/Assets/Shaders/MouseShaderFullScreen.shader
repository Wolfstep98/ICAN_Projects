Shader "Custom/MouseShaderFullScreen"
{
    Properties
    {
    }
    
    SubShader
    {
        Tags
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+100" 
        }
        
        // to "grab" what is behind just before this pass in rendered.
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
            ZTest Always
        
            CGPROGRAM
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            
            uniform sampler2D _BackgroundTexture;
            uniform float4 _BackgroundTexture_TexelSize;
            
            uniform float4 _MousePosition;
            
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
                const float2 basePixelPosition = float2(IN.uv.x, IN.uv.y) / abs(_BackgroundTexture_TexelSize.xy);
                const float2 pixelPosition = _ProjectionParams.x > 0 ? basePixelPosition : float2(basePixelPosition.x, _ScreenParams.y - basePixelPosition.y);
                const float distanceToMouseX = length(_MousePosition.x - pixelPosition.x);
                const float distanceToMouseY = length(_MousePosition.y - pixelPosition.y);
                const float inMouseSpot = (distanceToMouseX < 10) || (distanceToMouseY < 10);
            
                const float4 previous = tex2D(_BackgroundTexture, float2(IN.uv.x, 1 - IN.uv.y));
                
                return float4(inMouseSpot ? previous.xyz * 2 : previous.xyz, 1);
            }
            ENDCG
        }
    }
}
