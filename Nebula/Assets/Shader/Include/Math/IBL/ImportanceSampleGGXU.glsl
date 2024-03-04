vec3 importanceSampleGGX(vec2 hammer, vec3 normal)
{
	float roughness4 = pow(u_roughness, 4.0);
	float phi = 2.0 * PI * hammer.x;
	
	vec3 halfVector;
	halfVector.z = sqrt((1.0 - hammer.y) / (1.0 + (roughness4 - 1.0) * hammer.y));
	float sinTheta = sqrt(1.0 - halfVector.z * halfVector.z);
	halfVector.x = cos(phi) * sinTheta;
	halfVector.y = sin(phi) * sinTheta;
	
    vec3 up;
    if (abs(normal.z) < 0.999)
    {
        up = vec3(0.0, 0.0, 1.0);
    }
    else
    {
        up = vec3(1.0, 0.0, 0.0);
    }
	vec3 tangent = normalize(cross(up, normal));
	vec3 bitangent = cross(normal, tangent);
	
	vec3 samplePosition = halfVector.x * tangent + halfVector.y * bitangent + halfVector.z * normal;
	return normalize(samplePosition);
}