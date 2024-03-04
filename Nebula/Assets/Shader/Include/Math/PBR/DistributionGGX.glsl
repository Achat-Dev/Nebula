float distributionGGX(float nDotH, float roughness)
{
	float roughness4 = pow(roughness, 4.0);
	float nDotH2 = nDotH * nDotH;

	float denom = (nDotH2 * (roughness4 - 1.0) + 1.0);
	denom = PI * denom * denom;
	
	return roughness4 / denom;
}