Shader "Internal/NeedleClock"
{
    Properties
    {
        _Hour("Hour", float) = 0
        _Minutes("_Minutes", float) = 0
        _Seconds("_Seconds", float) = 0
        
        _HourSettings("Hour Settings w, minV, maxV, na", Vector) = (0.05, 0, 0.3, 0)
        _MinutesSettings("Minutes Settings w, minV, maxV, na", Vector) = (0.025, 0, 0.4, 0)
        _SecondsSettings("Seconds Settings w, minV, maxV, na", Vector) = (0.001, 0, 0.5, 0)
        
        _BackgrounColor("Background color", COLOR) = (0, 0, 0, 1)
        _ForegroundColor("Foreground color", COLOR) = (1, 1, 1, 1)
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
            ZTest LEqual
        
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Assets/[Tools]/Shaders/Include/AntialiasingUtils.cginc"
            #define PI 3.141592
            
            uniform float4 _BackgrounColor;
            uniform float4 _ForegroundColor;
            
            uniform float4 _HourSettings;
            uniform float4 _MinutesSettings;
            uniform float4 _SecondsSettings;
            
            // from global shader, not a property. Exported by the script ExportTimeOfTheDayToShader
            uniform float4 _DayTime; //x : seconds since start of the day, yzw hour/minutes/Seconds as int

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uvHour : TEXCOORD0;
                float2 uvMinutes : TEXCOORD1;
                float2 uvSeconds : TEXCOORD2;
            };
            
            float2 GetNeedleUV(float2 baseUV, float2 uvCenter, float valueIn01)
            {
                float2 sinCosValueIn01;
                sincos(valueIn01 * 2 * PI, sinCosValueIn01.x, sinCosValueIn01.y);
                const float2 centeredUV = baseUV - uvCenter;
                return float2(centeredUV.x * sinCosValueIn01.x + centeredUV.y * sinCosValueIn01.y, -centeredUV.x * sinCosValueIn01.y + centeredUV.y * sinCosValueIn01.x);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                const float timeInSec = _DayTime.x;
                const float hour = frac(timeInSec / (12 * 60 * 60));
                const float minutes = frac(timeInSec / (60 * 60));
                const float seconds = trunc(60 * frac(timeInSec / 60.0)) / 60.0; // trunc to the round second to have a better visual effect.
                
                o.uvHour = GetNeedleUV(v.uv, 0.5, hour);
                o.uvMinutes = GetNeedleUV(v.uv, 0.5, minutes);
                o.uvSeconds = GetNeedleUV(v.uv, 0.5, seconds);
                return o;
            }
            
            float GetLine(float2 uv, float2 ddxUv, float2 ddyUv, float4 lineSettings)
            {
                const float2 pixelSize = float2(length(float2(ddxUv.x, ddyUv.x)), length(float2(ddxUv.y, ddyUv.y)));
                
                const float lineU = AlphaDoor5A(uv.y + pixelSize.y, uv.y - pixelSize.y, -lineSettings.x, lineSettings.x);
                const float lineV = AlphaDoor5A(uv.x + pixelSize.x, uv.x - pixelSize.x, lineSettings.y, lineSettings.z);
                return lineU * lineV;
            }
            
            float4 frag (v2f IN) : COLOR
            {
                
                const float lineHour = GetLine(IN.uvHour, ddx(IN.uvHour), ddy(IN.uvHour), _HourSettings);
                const float lineMinutes = GetLine(IN.uvMinutes, ddx(IN.uvMinutes), ddy(IN.uvMinutes), _MinutesSettings);
                const float lineSeconds = GetLine(IN.uvSeconds, ddx(IN.uvSeconds), ddy(IN.uvSeconds), _SecondsSettings);
                
                const float lineIntensity = max(max(lineHour, lineMinutes), lineSeconds);
                return lerp(_BackgrounColor, _ForegroundColor, lineIntensity);
            }
            ENDCG
        }
    }
}
