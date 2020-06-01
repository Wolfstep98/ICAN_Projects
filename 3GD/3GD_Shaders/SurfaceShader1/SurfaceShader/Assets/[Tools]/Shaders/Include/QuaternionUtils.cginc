#ifndef _QUATERNION_UTILS_INCLUDED_
#define _QUATERNION_UTILS_INCLUDED_

#ifdef _OCTWRAP_UTILS_INCLUDED_

float4 DecodeQuaternion(float3 encodedQuaternion)
{
    float3 normalizeAxis = DecodeOct(encodedQuaternion.xy);
    float3 axisInQuaternion = normalizeAxis * sqrt(1 - encodedQuaternion.z * encodedQuaternion.z);
    return float4(axisInQuaternion, encodedQuaternion.z);
}

#endif

float4 CreateQuaternion(const float3 normalizedAxis, float angleInRadiant)
{
    const float halfAngle =  0.5 * angleInRadiant;
    float cosHalfAngle;
    float sinHalfAngle;
    sincos(halfAngle, sinHalfAngle, cosHalfAngle);
    return float4(normalizedAxis * sinHalfAngle, cosHalfAngle);
}

float3 MultiplyQuaternion(const float4 quaternionUnity, float3 vector3)
{
    float a = quaternionUnity.w;
    float b = quaternionUnity.x;
    float c = quaternionUnity.y;
    float d = quaternionUnity.z;
    float v1 = vector3.x;
    float v2 = vector3.y;
    float v3 = vector3.z;
    
    float t2 =   a * b;
    float t3 =   a * c;
    float t4 =   a * d;
    float t5 =  -b * b;
    float t6 =   b * c;
    float t7 =   b * d;
    float t8 =  -c * c;
    float t9 =   c * d; 
    float t10 = -d * d;
    float v1new = 2 * ((t8 + t10) * v1 + (t6 -  t4) * v2 + (t3 + t7) * v3 ) + v1;
    float v2new = 2 * ((t4 +  t6) * v1 + (t5 + t10) * v2 + (t9 - t2) * v3 ) + v2;
    float v3new = 2 * ((t7 -  t3) * v1 + (t2 +  t9) * v2 + (t5 + t8) * v3 ) + v3;
    
    return float3(v1new, v2new, v3new);
}

float3x3 ConvertToRotationMatrix(const float4 quaternionUnity)
{
    const float a = quaternionUnity.w;
    const float b = quaternionUnity.x;
    const float c = quaternionUnity.y;
    const float d = quaternionUnity.z;
    const float t2 =   a * b;
    const float t3 =   a * c;
    const float t4 =   a * d;
    const float t5 =  -b * b;
    const float t6 =   b * c;
    const float t7 =   b * d;
    const float t8 =  -c * c;
    const float t9 =   c * d; 
    const float t10 = -d * d;
    
    const float3 axisX = float3(1 + 2 * (t8 + t10),     2 * (t6 - t4) ,     2 * (t3 + t7));
    const float3 axisY = float3(    2 * (t4 + t6) , 1 + 2 * (t5 + t10),     2 * (t9 - t2));
    const float3 axisZ = float3(    2 * (t7 - t3) ,     2 * (t2 + t9),  1 + 2 * (t5 + t8));
    return float3x3(axisX, axisY, axisZ);
}

#endif
