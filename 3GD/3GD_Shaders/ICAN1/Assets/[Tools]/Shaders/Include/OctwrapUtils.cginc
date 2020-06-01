#ifndef _OCTWRAP_UTILS_INCLUDED_
#define _OCTWRAP_UTILS_INCLUDED_
float2 OctWrap(float2 v)
{
    return (1.0 - abs(v.yx)) * (v.xy >= 0.0 ? 1.0 : -1.0);
}

float2 EncodeOct(float3 n)
{
    n /=  abs(n.x) + abs(n.y) + abs(n.z);
    n.xy = n.z >= 0.0 ? n.xy : OctWrap(n.xy);
    n.xy = n.xy * 0.5 + 0.5;
    return n.xy;
}

float3 DecodeOct(float2 encN)
{
    encN = encN * 2.0 - 1.0;
    float3 n;
    n.z = 1.0 - abs(encN.x) - abs(encN.y);
    n.xy = n.z >= 0.0 ? encN.xy : OctWrap(encN.xy);
    n = normalize(n);
    return n;
}
#endif //_OCTWRAP_UTILS_INCLUDED_
