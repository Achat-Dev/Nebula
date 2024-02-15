#version 460 core

#include Shader/Include/UB_Lights.glsl
#include Shader/Include/UB_Camera.glsl
#include Shader/Include/Pi.glsl

out vec4 o_colour;

in vec3 io_vertexPosition;
in vec3 io_normal;

uniform vec3 u_albedo;
uniform float u_metallic;
uniform float u_roughness;

float distributionGGX(float nDotH)
{
	float roughness4 = pow(u_roughness, 4.0);
	float nDotH2 = nDotH * nDotH;

	float denom = (nDotH2 * (roughness4 - 1.0) + 1.0);
	denom = PI * denom * denom;
	
	return roughness4 / denom;
}

float geometrySchlickGGX(float nDotV)
{
	float r = u_roughness + 1.0;
	float k = (r * r) / 8.0;
	float denom = nDotV * (1.0 - k) + k;

	return nDotV / denom;
}

vec3 fresnelSchlick(float hDotV, vec3 f0)
{
	return f0 + (1.0 - f0) * pow(clamp(1.0 - hDotV, 0.0, 1.0), 5.0);
}

vec3 calculateDirectionalLight(vec3 viewDirection, vec3 f0, vec3 normal)
{
	// Calculate Cook-Torrance BRDF
	vec3 lightDirection = -u_directionalLight.direction;
	vec3 halfVector = normalize(viewDirection + lightDirection);

	float nDotH = max(dot(normal, halfVector), 0.0);
	float nDotL = max(dot(normal, lightDirection), 0.0);
	float nDotV = max(dot(normal, viewDirection), 0.0);
	float hDotV = clamp(dot(halfVector, viewDirection), 0.0, 1.0);

	// Calculate kd
	vec3 fresnel = fresnelSchlick(hDotV, f0);
	vec3 kd = vec3(1.0) - fresnel;
	kd *= 1.0 - u_metallic;

	// Calculate specular
	vec3 specular = distributionGGX(nDotH) * geometrySchlickGGX(nDotL) * geometrySchlickGGX(nDotV) * fresnel;
	float specularDenom = 4.0 * nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
	specular /= specularDenom;

	return (kd * u_albedo / PI + specular) * u_directionalLight.colour * nDotL;
}

vec3 calculatePointLights(vec3 viewDirection, vec3 f0, vec3 normal)
{
	vec3 colour = vec3(0.0);
	for (int i = 0; i < u_pointLightCount; i++)
	{
		// Calculate radiance
		vec3 lightDirection = u_pointLights[i].position - io_vertexPosition;
		float distance = length(lightDirection) / u_pointLights[i].range;
		float attenuation = 1.0 / (distance * distance);
		vec3 radiance = u_pointLights[i].colour * attenuation;

		// Calculate Cook-Torrance BRDF
		lightDirection = normalize(lightDirection);
		vec3 halfVector = normalize(viewDirection + lightDirection);

		float nDotH = max(dot(normal, halfVector), 0.0);
		float nDotL = max(dot(normal, lightDirection), 0.0);
		float nDotV = max(dot(normal, viewDirection), 0.0);
		float hDotV = clamp(dot(halfVector, viewDirection), 0.0, 1.0);

		// Calculate kd
		vec3 fresnel = fresnelSchlick(hDotV, f0);
		vec3 kd = vec3(1.0) - fresnel;
		kd *= 1.0 - u_metallic;

		// Calculate specular
		vec3 specular = distributionGGX(nDotH) * geometrySchlickGGX(nDotL) * geometrySchlickGGX(nDotV) * fresnel;
		float specularDenom = 4.0 * nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
		specular /= specularDenom;

		colour += (kd * u_albedo / PI + specular) * radiance * nDotL;
	}
	return colour;
}

void main()
{
	vec3 viewDirection = normalize(u_cameraPosition - io_vertexPosition);
	vec3 f0 = mix(vec3(0.04), u_albedo, u_metallic);
	vec3 normal = normalize(io_normal);

	vec3 colour = calculateDirectionalLight(viewDirection, f0, normal);
	colour += calculatePointLights(viewDirection, f0, normal);

	// HDR tonemapping
	colour = colour / (colour + vec3(1.0));

	// Gamma correction
	colour = pow(colour, vec3(1.0 / 2.2));

	o_colour = vec4(colour, 1.0);
}
