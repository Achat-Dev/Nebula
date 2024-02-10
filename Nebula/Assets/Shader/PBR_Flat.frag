#version 460 core

#define MAX_POINT_LIGHTS 128

struct PointLight
{
	vec3 position;
	vec3 colour;
	float range;
};

out vec4 o_colour;

in vec3 io_vertexPosition;
in vec3 io_normal;

uniform vec3 u_cameraPosition;

uniform vec3 u_albedo;
uniform float u_metallic;
uniform float u_roughness;
uniform float u_ambientOcclusion;

uniform int u_pointLightCount;
uniform PointLight u_pointLights[MAX_POINT_LIGHTS];

const float PI = 3.14159265359;

float distributionGGX(float nDotH, float roughness)
{
	float roughness4 = pow(roughness, 4.0);
	float nDotH2 = nDotH * nDotH;

	float denom = (nDotH2 * (roughness4 - 1.0) + 1.0);
	denom = PI * denom * denom;
	
	return roughness4 / denom;
}

float geometrySchlickGGX(float nDotV, float roughness)
{
	float r = roughness + 1.0;
	float k = (r * r) / 8.0;
	float denom = nDotV * (1.0 - k) + k;

	return nDotV / denom;
}

float geometrySmith(float nDotL, float nDotV, float roughness)
{
	float ggx1 = geometrySchlickGGX(nDotL, roughness);
	float ggx2 = geometrySchlickGGX(nDotV, roughness);

	return ggx1 * ggx2;
}

vec3 fresnel(float hDotV, vec3 f0)
{
	return f0 + (1.0 - f0) * pow(clamp(1.0 - hDotV, 0.0, 1.0), 5.0);
}

void main()
{
	vec3 v = normalize(u_cameraPosition - io_vertexPosition);

	vec3 f0 = vec3(0.04);
	f0 = mix(f0, u_albedo, u_metallic);

	vec3 colour = vec3(0.0);
	for (int i = 0; i < u_pointLightCount; i++)
	{
		// Calculate per-light radiance
		vec3 l = normalize(u_pointLights[i].position - io_vertexPosition);
		vec3 h = normalize(v + l);
		float distance = length(u_pointLights[i].position - io_vertexPosition) / u_pointLights[i].range;
		float attenuation = 1.0 / (distance * distance);
		vec3 radiance = u_pointLights[i].colour * attenuation;

		// Cook-Torrance BRDF
		float nDotH = max(dot(io_normal, h), 0.0);
		float nDotL = max(dot(io_normal, l), 0.0);
		float nDotV = max(dot(io_normal, v), 0.0);
		float hDotV = clamp(dot(h, v), 0.0, 1.0);

		float ndf = distributionGGX(nDotH, u_roughness);
		float g = geometrySmith(nDotL, nDotV, u_roughness);
		vec3 f = fresnel(hDotV, f0);

		vec3 nom = ndf * g * f;
		// plus at the end to prevent dividing by 0
		float denom = 4.0 * nDotV * nDotL + 0.0001;
		vec3 specular = nom / denom;

		vec3 ks = f;
		vec3 kd = vec3(1.0) - ks;
		kd *= 1.0 - u_metallic;

		colour += (kd * u_albedo / PI + specular) * radiance * nDotL;
	}

	vec3 ambient = vec3(0.03) * u_albedo * u_ambientOcclusion;
	colour += ambient;

	// HDR tonemapping
	colour = colour / (colour + vec3(1.0));

	// Gamma correction
	colour = pow(colour, vec3(1.0 / 2.2));

	o_colour = vec4(colour, 1.0);
}
