#version 460 core

#include UniformBuffer/Lights.glsl
#include UniformBuffer/Camera.glsl

out vec4 o_colour;

in vec3 io_vertexPosition;
in vec3 io_normal;
in vec3 io_tangent;
in vec2 io_uv;

uniform sampler2D u_albedoMap;
uniform sampler2D u_normalMap;
uniform sampler2D u_metallicMap;
uniform sampler2D u_roughnessMap;

#include Math/Pi.glsl
#include Math/PBR/DistributionGGX.glsl
#include Math/PBR/GeometrySchlickGGX.glsl
#include Math/PBR/FresnelSchlick.glsl
#include Math/PBR/FresnelSchlickRoughness.glsl
#include Math/PBR/MaxReflectionLod.glsl

// Possible optimisation:
// | Calculate tbn matrix in vertex shader
// | Yields slighty different results (only visible if you really try to see them), but should be faster
vec3 getNormalFromMap()
{
    vec3 tangentNormal = texture(u_normalMap, io_uv).xyz * 2.0 - 1.0;

	vec3 normal = normalize(io_normal);
	vec3 tangent = normalize(io_tangent);
	vec3 bitangent = -normalize(cross(normal, tangent));
	mat3 tbn = mat3(tangent, bitangent, normal);

	return normalize(tbn * tangentNormal);
}

vec3 calculateDirectionalLight(vec3 viewDirection, vec3 f0, vec3 albedo, vec3 normal, float metallic, float roughness)
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
	kd *= 1.0 - metallic;

	// Calculate specular
	vec3 specular = distributionGGX(nDotH, roughness) * geometrySchlickGGX(nDotL, roughness) * geometrySchlickGGX(nDotV, roughness) * fresnel;
	float specularDenom = 4.0 * nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
	specular /= specularDenom;

	return (kd * albedo / PI + specular) * u_directionalLight.colour * nDotL;
}

vec3 calculatePointLights(vec3 viewDirection, vec3 f0, vec3 albedo, vec3 normal, float metallic, float roughness)
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
		kd *= 1.0 - metallic;

		// Calculate specular
		vec3 specular = distributionGGX(nDotH, roughness) * geometrySchlickGGX(nDotL, roughness) * geometrySchlickGGX(nDotV, roughness) * fresnel;
		float specularDenom = 4.0 * nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
		specular /= specularDenom;

		colour += (kd * albedo / PI + specular) * radiance * nDotL;
	}
	return colour;
}

// nDotV is calculated twice, once with the normalised and once with the unnormlaised normal
// | This is to minimise artifacts in the specular lighting
vec3 calculateIBL(vec3 viewDirection, vec3 f0, vec3 albedo, vec3 normal, float metallic, float roughness)
{
	vec3 fresnel = fresnelSchlickRoughness(max(dot(normal, viewDirection), 0.0), f0, roughness);
	vec3 kd = 1.0 - fresnel;
	kd *= 1.0 - metallic;

	vec3 irradiance = texture(u_irradianceMap, normal).rgb;
	vec3 diffuse = albedo * irradiance;

	vec3 prefiltered = textureLod(u_prefilteredMap, reflect(-viewDirection, normal), roughness * MAX_REFLECTION_LOD).rgb;
	vec2 brdf = texture(u_brdfLut, vec2(max(dot(io_normal, viewDirection), 0.0), roughness)).rg;
	vec3 specular = prefiltered * (fresnel * brdf.x + brdf.y);

	return (kd * diffuse + specular) * u_skyLightIntensity;
}

void main()
{
	vec3 albedo = pow(texture(u_albedoMap, io_uv).rgb, vec3(2.2));
	vec3 normal = getNormalFromMap();
	float metallic = texture(u_metallicMap, io_uv).r;
	float roughness = texture(u_roughnessMap, io_uv).r;


	vec3 viewDirection = normalize(u_cameraPosition - io_vertexPosition);
	vec3 f0 = mix(vec3(0.04), albedo, metallic);

	vec3 colour = calculateDirectionalLight(viewDirection, f0, albedo, normal, metallic, roughness);
	colour += calculatePointLights(viewDirection, f0, albedo, normal, metallic, roughness);
	colour += calculateIBL(viewDirection, f0, albedo, normal, metallic, roughness);

	#include Math/PBR/HDRTonemapping.glsl
	#include Math/PBR/GammaCorrection.glsl

	o_colour = vec4(colour, 1.0);
}
