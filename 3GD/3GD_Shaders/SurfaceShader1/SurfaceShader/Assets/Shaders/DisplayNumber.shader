Shader "Internal/DisplayNumber"
{
    Properties
    {
        [NoScaleOffset] _NumberAtlas ("Atlas Number ", 2D) = "black" {}
        
        _ValueToDisplay("Value", Float) = 3.141592
        _BackgrounColor("Background color", COLOR) = (0, 0, 0, 1)
        _LetterColor("Letter color", COLOR) = (1, 1, 1, 1)
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
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment frag
            
            #define CHAR_COUNT 10
            #define CHAR_PER_LINE 4
            
            #include "Assets/[Tools]/Shaders/Include/AntiAliasingUtils.cginc"
            
            uniform sampler2D _NumberAtlas;
            uniform float4 _NumberAtlas_TexelSize;
            
            uniform float _ValueToDisplay;
            uniform float4 _BackgrounColor;
            uniform float4 _LetterColor;

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
                o.uv = v.uv * float2(CHAR_COUNT, 1);
                return o;
            }
            
            float4 NumberUVMinMax(uint number)
            {
                const float ooCharPerLine = 1.0 / CHAR_PER_LINE;
                const uint col = number % CHAR_PER_LINE;
                const uint lineIndex = number / CHAR_PER_LINE;
                
                const float2 baseMinUV = float2(col * ooCharPerLine, lineIndex * ooCharPerLine);
                const float2 baseMaxUV = baseMinUV + ooCharPerLine;
                const float2 minUV = lerp(baseMinUV, baseMaxUV, float2(0.35, 0.3)); //those values are tuned to cut the useless part of the texture.
                const float2 maxUV = lerp(baseMinUV, baseMaxUV, float2(0.65, 0.7));
                
                // because of unity convention of uv
                return float4(minUV.x, 1 - maxUV.y, maxUV.x, 1 - minUV.y);
            }
            
            float4 frag (v2f IN) : COLOR
            {
                // i use an array because it is difficult to produce 10^n precisly for without doing a loop. need shader model 4 to access array from a computed value.
                const uint decimaValue[CHAR_COUNT] = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000};
                
                const float cellIndexf = floor(IN.uv.x);
                const uint cellIndex = cellIndexf;
                const float2 uvInCell = float2(IN.uv.x - cellIndexf, IN.uv.y);
                
                const uint valueToDisplay = round(_ValueToDisplay); //1000 * _Time.y;
                //const uint valueToDisplay = 10000 * _Time.y;
                
                const uint unite = decimaValue[CHAR_COUNT - cellIndex - 1];
                const uint numberToDisplay = (valueToDisplay / unite) % 10;
                
                float value = 0;
                if (valueToDisplay > unite || unite == 1)
                {
                    const float4 numberUV = NumberUVMinMax(numberToDisplay);
                    const float2 numberUVRange = numberUV.zw - numberUV.xy;
                    const float2 finalUV = lerp(numberUV.xy, numberUV.zw, uvInCell);
                    const float4 numberTex = tex2Dlod(_NumberAtlas, float4(finalUV, 0, 0));
                    
                    const float2 ddxUVInTex = numberUVRange * ddx(IN.uv) / ( _NumberAtlas_TexelSize.xy);
                    const float2 ddyUVInTex = numberUVRange * ddy(IN.uv) / ( _NumberAtlas_TexelSize.xy);
                    const float halfPixelSize = 0.5 * length(float2(length(float2(ddxUVInTex.x, ddyUVInTex.x)), length(float2(ddxUVInTex.y, ddyUVInTex.y))));
                    
                    const float pixelDistance = 10;
                    const float signedDistance =  2 * (numberTex.x - 0.5) * pixelDistance;
                    
                    value = AlphaDoor5A(signedDistance - halfPixelSize, signedDistance + halfPixelSize, 0, pixelDistance);
                }
                
                return lerp(_BackgrounColor, _LetterColor, value);
            }
            ENDCG
        }
    }
}
