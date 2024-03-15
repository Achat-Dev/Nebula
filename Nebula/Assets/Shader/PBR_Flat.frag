#version 460 core

#include UniformBuffer/Lights.glsl
#include UniformBuffer/Camera.glsl

out vec4 o_colour;

in vec3 io_vertexPosition;
in vec3 io_normal;

uniform vec3 u_albedo;
uniform float u_metallic;
uniform float u_roughness;

#include Math/Pi.glsl
#include Math/PBR/DistributionGGXU.glsl
#include Math/PBR/GeometrySchlickGGXU.glsl
#include Math/PBR/FresnelSchlick.glsl
#include Math/PBR/FresnelSchlickRoughnessU.glsl
#include Math/PBR/MaxReflectionLod.glsl

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

// nDotV is calculated twice, once with the normalised and once with the unnormlaised normal
// | This is to minimise artifacts in the specular lighting
vec3 calculateIBL(vec3 viewDirection, vec3 f0, vec3 normal)
{
	vec3 fresnel = fresnelSchlickRoughness(max(dot(normal, viewDirection), 0.0), f0);
	vec3 kd = 1.0 - fresnel;
	kd *= 1.0 - u_metallic;

	vec3 irradiance = texture(u_irradianceMap, normal).rgb;
	vec3 diffuse = u_albedo * irradiance;

	vec3 prefiltered = textureLod(u_prefilteredMap, reflect(-viewDirection, normal), u_roughness * MAX_REFLECTION_LOD).rgb;
	vec2 brdf = texture(u_brdfLut, vec2(max(dot(io_normal, viewDirection), 0.0), u_roughness)).rg;
	vec3 specular = prefiltered * (fresnel * brdf.x + brdf.y);

	return (kd * diffuse + specular) * u_skyLightIntensity;
}

void main()
{
	vec3 viewDirection = normalize(u_cameraPosition - io_vertexPosition);
	vec3 f0 = mix(vec3(0.04), u_albedo, u_metallic);
	vec3 normal = normalize(io_normal);

	vec3 colour = calculateDirectionalLight(viewDirection, f0, normal);
	colour += calculatePointLights(viewDirection, f0, normal);
	colour += calculateIBL(viewDirection, f0, normal);

	#include Math/PBR/HDRTonemapping.glsl
	#include Math/PBR/GammaCorrection.glsl

	o_colour = vec4(colour, 1.0);
}
