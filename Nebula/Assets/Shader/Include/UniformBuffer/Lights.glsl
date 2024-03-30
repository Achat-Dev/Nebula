#define MAX_POINT_LIGHTS 128

struct DirectionalLight
{
	vec3 direction;
	vec3 colour;
};

struct PointLight
{
	vec3 position;
	float range;
	vec3 colour;
	float intensity;
};

struct FlatLightParams
{
	vec3 viewDirection;
	vec3 normal;
	vec3 f0;
	float nDotV;
};

struct TexturedLightParams
{
	vec3 viewDirection;
	vec3 normal;
	vec3 f0;
	vec3 albedo;
	float nDotV;
	float metallic;
	float roughness;
};

layout (std140, binding = 2) uniform ub_lights
{
	uniform float u_skyLightIntensity;
	uniform int u_pointLightCount;
	uniform DirectionalLight u_directionalLight;
	uniform PointLight u_pointLights[MAX_POINT_LIGHTS];
};

uniform samplerCube u_irradianceMap;
uniform samplerCube u_prefilteredMap;
uniform sampler2D u_brdfLut;
uniform sampler2D u_directionalShadowMap;
uniform samplerCubeArray u_omnidirectionalShadowMap;

const float c_shadowSamplingBias = 0.07;