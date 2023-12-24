#version 330 core

#include Shader/Include/LightData.glsl

layout (location = 0) in vec3 i_position;
layout (location = 1) in vec3 i_normal;

out vec4 io_colour;

uniform mat4 u_model;
uniform mat4 u_viewProjection;
uniform mat4 u_modelNormalMatrix;

uniform int u_pointLightCount;
uniform vec3 u_cameraPosition;

uniform Material u_material;
uniform DirectionalLight u_directionalLight;
uniform PointLight u_pointLights[MAX_POINT_LIGHTS];

#include Shader/Include/LightMethods.glsl
#stop include

void main()
{
	vec3 normal = normalize(mat3(u_modelNormalMatrix) * i_normal);

	vec3 vertexPosition = vec3(u_model * vec4(i_position, 1.0));
	vec3 viewDirection = normalize(u_cameraPosition - vertexPosition);

	vec3 result = CalculateDirectionalLight(u_directionalLight, normal, viewDirection);
	for (int i = 0; i < u_pointLightCount; i++)
	{
		result += CalculatePointLight(u_pointLights[i], normal, vertexPosition, viewDirection);
	}

	io_colour = vec4(result, 1.0);

	gl_Position = u_viewProjection * vec4(vertexPosition, 1.0);
}
