﻿#version 460 core

layout (location = 0) in vec3 i_position;

#include UniformBuffer/Matrices.glsl

uniform mat4 u_modelMatrix;

void main()
{
	gl_Position = u_viewProjection * u_modelMatrix * vec4(i_position, 1.0);
}
