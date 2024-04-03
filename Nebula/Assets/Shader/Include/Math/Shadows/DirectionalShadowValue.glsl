float calculateDirectionalShadowValue()
{
	vec3 uv = io_vertexPositionLightSpace.xyz / io_vertexPositionLightSpace.w;
	uv = uv * 0.5 + 0.5;

	float mappedDepth = texture(u_directionalShadowMap, uv.xy).r;

	if (uv.z > 1.0)
	{
		return 1.0;
	}

	if (mappedDepth < uv.z - c_directionalShadowSamplingBias)
	{
		if (max(uv.x, uv.y) > 1.0 || min(uv.x, uv.y) < 0.0)
		{
			return 1.0;
		}
		return 0.0;
	}

	return 1.0;
}