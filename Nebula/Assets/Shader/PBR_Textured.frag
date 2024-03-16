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

vec3 calculateDirectionalLight(TexturedLightParams params)
{
	// Calculate Cook-Torrance BRDF
	vec3 lightDirection = -u_directionalLight.direction;
	vec3 halfVector = normalize(params.viewDirection + lightDirection);

	float nDotH = max(dot(params.normal, halfVector), 0.0);
	float nDotL = max(dot(params.normal, lightDirection), 0.0);
	float hDotV = clamp(dot(halfVector, params.viewDirection), 0.0, 1.0);

	// Calculate kd
	vec3 fresnel = fresnelSchlick(hDotV, params.f0);
	vec3 kd = vec3(1.0) - fresnel;
	kd *= 1.0 - params.metallic;

	// Calculate specular
	vec3 specular = distributionGGX(nDotH, params.roughness) * geometrySchlickGGX(nDotL, params.roughness) * geometrySchlickGGX(params.nDotV, params.roughness) * fresnel;
	float specularDenom = 4.0 * params.nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
	specular /= specularDenom;

	return (kd * params.albedo / PI + specular) * u_directionalLight.colour * nDotL;
}

vec3 calculatePointLights(TexturedLightParams params)
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
		vec3 halfVector = normalize(params.viewDirection + lightDirection);

		float nDotH = max(dot(params.normal, halfVector), 0.0);
		float nDotL = max(dot(params.normal, lightDirection), 0.0);
		float hDotV = clamp(dot(halfVector, params.viewDirection), 0.0, 1.0);

		// Calculate kd
		vec3 fresnel = fresnelSchlick(hDotV, params.f0);
		vec3 kd = vec3(1.0) - fresnel;
		kd *= 1.0 - params.metallic;

		// Calculate specular
		vec3 specular = distributionGGX(nDotH, params.roughness) * geometrySchlickGGX(nDotL, params.roughness) * geometrySchlickGGX(params.nDotV, params.roughness) * fresnel;
		float specularDenom = 4.0 * params.nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
		specular /= specularDenom;

		colour += (kd * params.albedo / PI + specular) * radiance * nDotL;
	}
	return colour;
}

vec3 calculateIBL(TexturedLightParams params)
{
	vec3 fresnel = fresnelSchlickRoughness(params.nDotV, params.f0, params.roughness);
	vec3 kd = 1.0 - fresnel;
	kd *= 1.0 - params.metallic;

	vec3 irradiance = texture(u_irradianceMap, params.normal).rgb;
	vec3 diffuse = params.albedo * irradiance;

	vec3 prefiltered = textureLod(u_prefilteredMap, reflect(-params.viewDirection, params.normal), params.roughness * MAX_REFLECTION_LOD).rgb;
	vec2 brdf = texture(u_brdfLut, vec2(params.nDotV, params.roughness)).rg;
	vec3 specular = prefiltered * (fresnel * brdf.x + brdf.y);

	return (kd * diffuse + specular) * u_skyLightIntensity;
}

void main()
{
	TexturedLightParams params;
	params.viewDirection = normalize(u_cameraPosition - io_vertexPosition);
	params.normal = getNormalFromMap();
	params.nDotV = max(dot(params.normal, params.viewDirection), 0.0);
	params.albedo = pow(texture(u_albedoMap, io_uv).rgb, vec3(2.2));
	params.metallic = texture(u_metallicMap, io_uv).r;
	params.f0 = mix(vec3(0.04), params.albedo, params.metallic);
	params.roughness = texture(u_roughnessMap, io_uv).r;

	vec3 colour = calculateDirectionalLight(params);
	colour += calculatePointLights(params);
	colour += calculateIBL(params);

	#include Math/PBR/HDRTonemapping.glsl
	#include Math/PBR/GammaCorrection.glsl

	o_colour = vec4(colour, 1.0);
}
