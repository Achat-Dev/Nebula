#include Math/IBL/SampleCount.glsl
#include Math/IBL/RadicalInverseVdC.glsl

vec2 hammersley(uint i)
{
    return vec2(float(i)/float(SAMPLE_COUNT), radicalInverseVdC(i));
}