#define MAX_POINT_LIGHTS 128

struct DirectionalLight
{
	vec3 direction;
	vec3 colour;
};

struct PointLight
{
	float range;
	vec3 position;
	vec3 colour;
};

layout (std140, binding = 2) uniform ub_lights
{
	uniform int u_pointLightCount;
	uniform DirectionalLight u_directionalLight;
	uniform PointLight u_pointLights[MAX_POINT_LIGHTS];
};

uniform samplerCube u_irradianceMap;
uniform samplerCube u_prefilteredMap;
uniform sampler2D u_brdfLut;