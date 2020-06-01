Shader "Custom/Eval7"
{
    Properties
    {
        _GradiantTex("Grandiant", 2D) = "white"
        _GradiantU("_GradiantU", Float) = 0.5
        _Center("Center", Vector) = (0, 0, 0, 0)
        _Scale("Scale", Float) = 1
        _IterationCount("IterationCount", Int) = 256
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

            uniform sampler2D _GradiantTex;
            uniform float _GradiantU;
            uniform float4 _Center;
            uniform Float _Scale;
            uniform uint _IterationCount;

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

            uint MandelBrot(const float2 c, uint iterationCount)
            {
                float2 z = c;
                for (uint i = 0; i < iterationCount; i++)
                {
                    const float2 xy = c + z * z.x + float2(-z.y, z.x) * z.y;
                    if (dot(xy, xy) > 4)
                        return i;
                    z = xy;
                }

                return iterationCount;
            }

            float MandelBrotIntensity(const float2 c)
            {
                const uint mandel = MandelBrot(c, _IterationCount;
                return (mandel == _IterationCount ? 0.0 : (float)mandel) / (float)_IterationCount;
            }

            float4 frag(v2f IN) : COLOR
            {
                const float2 c = (IN.uv - 0.5) * _Scale - _Center.xy;
                const float intensity = MandelBrotIntensity(c);

                const float4 texColor = tex2Dlod(_GradiantTex, float4(intensity, _GradiantU, 0, 0));
                return float4(intensity > 0 ? texColor.xyz : float3(0, 0, 0), 1);
            }
            
            ENDCG
        }
    }
}
