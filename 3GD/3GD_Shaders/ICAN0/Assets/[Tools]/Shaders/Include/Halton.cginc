#ifndef _HALTON_CGINC_
#define _HALTON_CGINC_

//https://en.wikipedia.org/wiki/Halton_sequence
float GetHaltonValue(uint index, uint radix)
{
    float result = 0.0f;
    float fraction = 1.0f / (float)radix;

    while (index > 0)
    {
        result += (float)(index % radix) * fraction;

        index /= radix;
        fraction /= (float)radix;
    }

    return result;
}

#endif
