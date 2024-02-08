#version 460 core

#include Shader/Include/LightData.glsl

out vec4 o_colour;

in vec3 io_vertexPosition;
in vec3 io_normal;

uniform int u_pointLightCount;
uniform vec3 u_cameraPosition;

uniform Material u_material;
uniform DirectionalLight u_directionalLight;
uniform PointLight u_pointLights[MAX_POINT_LIGHTS];

#include Shader/Include/LightMethods.glsl
#stop include

void main()
{
	vec3 viewDirection = normalize(u_cameraPosition - io_vertexPosition);

	vec3 result = CalculateDirectionalLight(u_directionalLight, io_normal, viewDirection);
	for (int i = 0; i < u_pointLightCount; i++)
	{
		result += CalculatePointLight(u_pointLights[i], io_normal, io_vertexPosition, viewDirection);
	}

	o_colour = vec4(result, 1.0);
}
