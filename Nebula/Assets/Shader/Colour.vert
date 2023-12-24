#version 330 core

layout (location = 0) in vec3 i_position;

uniform mat4 u_model;
uniform mat4 u_viewProjection;

void main()
{
	gl_Position = u_viewProjection * u_model * vec4(i_position, 1.0);
}
