float geometrySchlickGGX(float nDotV)
{
	float r = u_roughness + 1.0;
	float k = (r * r) / 8.0;
	float denom = nDotV * (1.0 - k) + k;

	return nDotV / denom;
}