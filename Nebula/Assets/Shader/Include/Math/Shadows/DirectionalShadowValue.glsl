const float c_directionalShadowMinBias = 0.0005;
const float c_directionalShadowMaxBias = 0.005;

#include UniformBuffer/Matrices.glsl

float calculateDirectionalShadowValue(float nDotL)
{
	vec4 vertexPositionViewSpace = u_viewMatrix * vec4(io_vertexPosition, 1.0);
	float depthValue = abs(vertexPositionViewSpace.z);

	int cascadeLayer = -1;
	for (int i = 0; i < CASCADE_COUNT; ++i)
    {
        if (depthValue < u_cascadeDistances[i])
        {
            cascadeLayer = i;
            break;
        }
    }
    if (cascadeLayer == -1)
    {
        cascadeLayer = CASCADE_COUNT;
    }

	vec4 vertexPositionLightSpace = u_directionalLightViewProjections[cascadeLayer] * vec4(io_vertexPosition, 1.0);

	vec3 uv = vertexPositionLightSpace.xyz / vertexPositionLightSpace.w;
	uv = uv * 0.5 + 0.5;

	float mappedDepth = texture(u_directionalShadowMap, vec3(uv.xy, cascadeLayer)).r;

	if (uv.z > 1.0)
	{
		return 1.0;
	}

	float bias = max(c_directionalShadowMaxBias * (1.0 - nDotL), c_directionalShadowMinBias);
	if (mappedDepth < uv.z - bias)
	{
		return 0.0;
	}

	return 1.0;
}