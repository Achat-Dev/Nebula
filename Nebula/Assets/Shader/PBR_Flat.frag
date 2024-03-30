#version 460 core

#include UniformBuffer/Lights.glsl
#include UniformBuffer/Camera.glsl

out vec4 o_colour;

in vec4 io_vertexPositionLightSpace;
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

float calculateDirectionalShadowValue()
{
	vec3 uv = io_vertexPositionLightSpace.xyz / io_vertexPositionLightSpace.w;
	uv = uv * 0.5 + 0.5;

	float mappedDepth = texture(u_directionalShadowMap, uv.xy).r;

	if (mappedDepth < uv.z)
	{
		return 0.0;
	}
	else
	{
		return 1.0;
	}
}

vec3 calculateDirectionalLight(FlatLightParams params)
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
	kd *= 1.0 - u_metallic;

	// Calculate specular
	vec3 specular = distributionGGX(nDotH) * geometrySchlickGGX(nDotL) * geometrySchlickGGX(params.nDotV) * fresnel;
	float specularDenom = 4.0 * params.nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
	specular /= specularDenom;

	return (kd * u_albedo / PI + specular) * u_directionalLight.colour * nDotL * calculateDirectionalShadowValue();
}

float calculateOmnidirectionalShadowValue(int index)
{
    vec3 uv = io_vertexPosition - u_pointLights[index].position;

    float mappedDepth = texture(u_omnidirectionalShadowMap, vec4(uv, index)).r * u_pointLights[index].range;
	float currentDepth = length(uv);

	if (mappedDepth > currentDepth - c_shadowSamplingBias)
	{
		return 1.0 - smoothstep(0.0, u_pointLights[index].range, currentDepth);
	}

	return 0.0;
}

vec3 calculatePointLights(FlatLightParams params)
{
	vec3 colour = vec3(0.0);
	for (int i = 0; i < u_pointLightCount; i++)
	{
		// Calculate radiance
		vec3 lightDirection = u_pointLights[i].position - io_vertexPosition;
		float distance = length(lightDirection);
		float attenuation = (u_pointLights[i].range * u_pointLights[i].intensity) / (distance * distance);
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
		kd *= 1.0 - u_metallic;

		// Calculate specular
		vec3 specular = distributionGGX(nDotH) * geometrySchlickGGX(nDotL) * geometrySchlickGGX(params.nDotV) * fresnel;
		float specularDenom = 4.0 * params.nDotV * nDotL + 0.0001; // plus at the end to prevent dividing by 0
		specular /= specularDenom;

		colour += (kd * u_albedo / PI + specular) * radiance * nDotL * calculateOmnidirectionalShadowValue(i);
	}
	return colour;
}

vec3 calculateIBL(FlatLightParams params)
{
	vec3 fresnel = fresnelSchlickRoughness(params.nDotV, params.f0);
	vec3 kd = 1.0 - fresnel;
	kd *= 1.0 - u_metallic;

	vec3 irradiance = texture(u_irradianceMap, params.normal).rgb;
	vec3 diffuse = u_albedo * irradiance;

	vec3 prefiltered = textureLod(u_prefilteredMap, reflect(-params.viewDirection, params.normal), u_roughness * MAX_REFLECTION_LOD).rgb;
	vec2 brdf = texture(u_brdfLut, vec2(params.nDotV, u_roughness)).rg;
	vec3 specular = prefiltered * (fresnel * brdf.x + brdf.y);

	return (kd * diffuse + specular) * u_skyLightIntensity;
}

void main()
{
	FlatLightParams params;
	params.viewDirection = normalize(u_cameraPosition - io_vertexPosition);
	params.normal = normalize(io_normal);
	params.f0 = mix(vec3(0.04), u_albedo, u_metallic);
	params.nDotV = max(dot(params.normal, params.viewDirection), 0.0);

	vec3 colour = calculateDirectionalLight(params);
	colour += calculatePointLights(params);
	colour += calculateIBL(params);

	#include Math/PBR/HDRTonemapping.glsl
	#include Math/PBR/GammaCorrection.glsl

	o_colour = vec4(colour, 1.0);
}
