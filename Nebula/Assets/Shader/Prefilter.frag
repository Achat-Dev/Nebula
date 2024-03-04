#version 460 core

#include Shader/Include/Math/Pi.glsl

in vec3 io_position;

out vec4 o_colour;

uniform float u_roughness;
uniform samplerCube u_environmentMap;

const uint SAMPLE_COUNT = 1024u;
const float TEXEL_RESOLUTION = 6.0 * 512.0 * 512.0;
const float TEXEL = 4.0 * PI / TEXEL_RESOLUTION;

float distributionGGX(float nDotH)
{
	float roughness4 = pow(u_roughness, 4.0);
	float nDotH2 = nDotH * nDotH;

	float denom = (nDotH2 * (roughness4 - 1.0) + 1.0);
	denom = PI * denom * denom;
	
	return roughness4 / denom;
}

float radicalInverseVdC(uint bits) 
{
    bits = (bits << 16u) | (bits >> 16u);
    bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
    bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
    bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
    bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);
    return float(bits) * 2.3283064365386963e-10;
}

vec2 hammersley(uint i)
{
    return vec2(float(i)/float(SAMPLE_COUNT), radicalInverseVdC(i));
}

vec3 importanceSampleGGX(vec2 hammer, vec3 normal)
{
	float roughness4 = pow(u_roughness, 4.0);
	float phi = 2.0 * PI * hammer.x;
	
	vec3 halfVector;
	halfVector.z = sqrt((1.0 - hammer.y) / (1.0 + (roughness4 - 1.0) * hammer.y));
	float sinTheta = sqrt(1.0 - halfVector.z * halfVector.z);
	halfVector.x = cos(phi) * sinTheta;
	halfVector.y = sin(phi) * sinTheta;
	
    vec3 up;
    if (abs(normal.z) < 0.999)
    {
        up = vec3(0.0, 0.0, 1.0);
    }
    else
    {
        up = vec3(1.0, 0.0, 0.0);
    }
	vec3 tangent = normalize(cross(up, normal));
	vec3 bitangent = cross(normal, tangent);
	
	vec3 samplePosition = halfVector.x * tangent + halfVector.y * bitangent + halfVector.z * normal;
	return normalize(samplePosition);
}

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
