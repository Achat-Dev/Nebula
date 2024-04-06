uniform samplerCubeArray u_pointShadowMap;

const float c_pointShadowMinBias = 0.07;
const float c_pointShadowMaxBias = 0.05;

float calculatePointShadowValue(int index, float nDotL)
{
    vec3 uv = io_vertexPosition - u_pointLights[index].position;

    float mappedDepth = texture(u_pointShadowMap, vec4(uv, index)).r * u_pointLights[index].range;
	float currentDepth = length(uv);

	float bias = max(c_pointShadowMaxBias * (1.0 - nDotL), c_pointShadowMinBias);
	if (mappedDepth > currentDepth - bias)
	{
		return 1.0 - smoothstep(0.0, u_pointLights[index].range, currentDepth);
	}

	return 0.0;
}