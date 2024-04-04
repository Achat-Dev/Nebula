const float c_directionalShadowMinBias = 0.0005;
const float c_directionalShadowMaxBias = 0.005;

float calculateDirectionalShadowValue(float nDotL)
{
	vec3 uv = io_vertexPositionLightSpace.xyz / io_vertexPositionLightSpace.w;
	uv = uv * 0.5 + 0.5;

	float mappedDepth = texture(u_directionalShadowMap, uv.xy).r;

	if (uv.z > 1.0)
	{
		return 1.0;
	}

	float bias = max(c_directionalShadowMaxBias * (1.0 - nDotL), c_directionalShadowMinBias);
	if (mappedDepth < uv.z - bias)
	{
		if (max(uv.x, uv.y) > 1.0 || min(uv.x, uv.y) < 0.0)
		{
			return 1.0;
		}
		return 0.0;
	}

	return 1.0;
}