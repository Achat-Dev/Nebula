#version 460 core

in vec2 io_uv;

out vec2 o_colour;

#include Math/Pi.glsl
#include Math/IBL/Hammersley.glsl
#include Math/IBL/ImportanceSampleGGX.glsl

// Can't use include version of this because k is different here
float geometrySchlickGGX(float nDotV, float roughness)
{
    float k = (roughness * roughness) / 2.0;
    float denom = nDotV * (1.0 - k) + k;

    return nDotV / denom;
}

float geometrySmith(vec3 normal, vec3 V, vec3 lightDirection, float roughness)
{
    float nDotV = max(dot(normal, V), 0.0);
    float nDotL = max(dot(normal, lightDirection), 0.0);
    return geometrySchlickGGX(nDotV, roughness) * geometrySchlickGGX(nDotL, roughness);
}

void main() 
{
    float nDotV = io_uv.x;
    float roughness = io_uv.y;

    vec3 V;
    V.x = sqrt(1.0 - nDotV * nDotV);
    V.y = 0.0;
    V.z = nDotV;

    float a = 0.0;
    float b = 0.0; 

    vec3 normal = vec3(0.0, 0.0, 1.0);
    
    for (uint i = 0u; i < SAMPLE_COUNT; ++i)
    {
        vec2 hammer = hammersley(i);
        vec3 halfVector = importanceSampleGGX(hammer, normal, roughness);
        vec3 lightDirection = normalize(2.0 * dot(V, halfVector) * halfVector - V);

        if (max(lightDirection.z, 0.0) > 0.0)
        {
            float vDotH = max(dot(V, halfVector), 0.0);
            float gVis = (geometrySmith(normal, V, lightDirection, roughness) * vDotH) / (max(halfVector.z, 0.0) * nDotV);
            float fc = pow(1.0 - vDotH, 5.0);

            a += (1.0 - fc) * gVis;
            b += fc * gVis;
        }
    }

    a /= float(SAMPLE_COUNT);
    b /= float(SAMPLE_COUNT);

    o_colour = vec2(a, b);
}