float calculateDirectionalShadowValue()
{
	vec3 uv = io_vertexPositionLightSpace.xyz / io_vertexPositionLightSpace.w;
	uv = uv * 0.5 + 0.5;

	float mappedDepth = texture(u_directionalShadowMap, uv.xy).r;

	if (mappedDepth < uv.z)
	{
		return 0.0;
	}
	else
	{
		return 1.0;
	}
}