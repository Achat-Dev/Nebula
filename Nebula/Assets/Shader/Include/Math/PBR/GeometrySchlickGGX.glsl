float geometrySchlickGGX(float nDotV, float roughness)
{
	float r = roughness + 1.0;
	float k = (r * r) / 8.0;
	float denom = nDotV * (1.0 - k) + k;

	return nDotV / denom;
}