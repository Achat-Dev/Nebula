vec3 fresnelSchlick(float hDotV, vec3 f0)
{
	return f0 + (1.0 - f0) * pow(clamp(1.0 - hDotV, 0.0, 1.0), 5.0);
}