#version 460 core

in vec3 io_position;

out vec4 o_colour;

uniform float u_roughness;
uniform samplerCube u_environmentMap;

#include Math/Pi.glsl
#include Math/IBL/Hammersley.glsl
#include Math/IBL/ImportanceSampleGGXU.glsl
#include Math/PBR/DistributionGGXU.glsl

const float TEXEL_RESOLUTION = 6.0 * 512.0 * 512.0;
const float TEXEL = 4.0 * PI / TEXEL_RESOLUTION;

void main()
{
    vec3 normal = normalize(io_position);
    vec3 colour = vec3(0.0);
    float totalWeight = 0.0;
    
    for (uint i = 0u; i < SAMPLE_COUNT; ++i)
    {
        vec2 hammer = hammersley(i);
        vec3 halfVector = importanceSampleGGX(hammer, normal);
        vec3 lightDirection = normalize(2.0 * dot(normal, halfVector) * halfVector - normal);
        float nDotL = max(dot(normal, lightDirection), 0.0);

        if (nDotL > 0.0)
        {
            float nDotH = max(dot(normal, halfVector), 0.0);
            float hDotV = max(dot(halfVector, normal), 0.0);
            float pdf = distributionGGX(nDotH) * nDotH / (4.0 * hDotV) + 0.0001; 

            float saSample = 1.0 / (float(SAMPLE_COUNT) * pdf + 0.0001);

            float mipLevel;
            if (u_roughness == 0.0)
            {
                mipLevel = 0.0;
            }
            else
            {
                mipLevel = 0.5 * log2(saSample / TEXEL);
            }
            
            colour += textureLod(u_environmentMap, lightDirection, mipLevel).rgb * nDotL;
            totalWeight += nDotL;
        }
    }

    colour /= totalWeight;

    o_colour = vec4(colour, 1.0);
}
