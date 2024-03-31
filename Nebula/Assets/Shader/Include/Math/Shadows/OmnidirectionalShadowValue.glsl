float calculateOmnidirectionalShadowValue(int index)
{
    vec3 uv = io_vertexPosition - u_pointLights[index].position;

    float mappedDepth = texture(u_omnidirectionalShadowMap, vec4(uv, index)).r * u_pointLights[index].range;
	float currentDepth = length(uv);

	if (mappedDepth > currentDepth - c_shadowSamplingBias)
	{
		return 1.0 - smoothstep(0.0, u_pointLights[index].range, currentDepth);
	}

	return 0.0;
}