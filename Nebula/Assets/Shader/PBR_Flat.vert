#version 460 core

layout (location = 0) in vec3 i_position;
layout (location = 1) in vec3 i_normal;

layout (std140, binding = 0) uniform ub_matrices
{
	mat4 u_viewProjection;
};

out vec3 io_vertexPosition;
out vec3 io_normal;

uniform mat4 u_model;
uniform mat3 u_modelNormalMatrix;

void main()
{
	io_normal = u_modelNormalMatrix * i_normal;
	io_vertexPosition = vec3(u_model * vec4(i_position, 1.0));
	gl_Position = u_viewProjection * vec4(io_vertexPosition, 1.0);
}
