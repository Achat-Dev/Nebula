const float c_omnidirectionalShadowMinBias = 0.07;
const float c_omnidirectionalShadowMaxBias = 0.05;

float calculateOmnidirectionalShadowValue(int index, float nDotL)
{
    vec3 uv = io_vertexPosition - u_pointLights[index].position;

    float mappedDepth = texture(u_omnidirectionalShadowMap, vec4(uv, index)).r * u_pointLights[index].range;
	float currentDepth = length(uv);

	float bias = max(c_omnidirectionalShadowMaxBias * (1.0 - nDotL), c_omnidirectionalShadowMinBias);
	if (mappedDepth > currentDepth - bias)
	{
		return 1.0 - smoothstep(0.0, u_pointLights[index].range, currentDepth);
	}

	return 0.0;
}