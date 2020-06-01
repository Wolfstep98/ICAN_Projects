Shader "Custom/MinimalGeometryShader"
{
    Properties
    {
    }
    
SubShader
{
    Tags 
    { 
        "RenderType" = "Transparent" 
        "Queue"="Transparent+0" 
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

    #define ID_PER_PRIMITIVE 6
            
    struct v2f
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    float2 GetCorner(uint index)
    {
#if 1
        const float2 corners[ID_PER_PRIMITIVE] = { float2(-0.5, -0.5), float2(-0.5, 0.5), float2(0.5, 0.5), float2(0.5, 0.5), float2(0.5, -0.5), float2(-0.5, -0.5) };
        return corners[index % ID_PER_PRIMITIVE];
#else
        return float2((index >= 2 && index <= 4) ? 0.5 : -0.5, (index >= 1 && index <= 3) ? 0.5 : -0.5);
#endif
    }

    v2f vert (uint id : SV_VertexID)
    {
        v2f o;
        const uint quadIndex = id / ID_PER_PRIMITIVE;
        const uint vertexIndex = id % ID_PER_PRIMITIVE;
        float vertexY = quadIndex;
        float2 corner = GetCorner(vertexIndex);
        float4 vertex = float4(corner.x, vertexY, corner.y, 1);
        o.vertex = UnityObjectToClipPos(vertex);
        o.uv = 0.5f + 0.5f * corner;
        return o;
    }
            
    float4 frag (v2f IN) : COLOR
    {
        return float4(IN.uv.xy, 0, 1);
    }
    ENDCG
}
}
}
