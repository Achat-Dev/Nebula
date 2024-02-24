#version 460 core

layout (location = 0) in vec3 i_position;

#include Shader/Include/UB_Matrices.glsl

uniform mat4 u_model;

void main()
{
	gl_Position = u_viewProjection * u_model * vec4(i_position, 1.0);
}
