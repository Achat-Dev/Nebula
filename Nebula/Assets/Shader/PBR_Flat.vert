﻿#version 460 core

layout (location = 0) in vec3 i_position;
layout (location = 1) in vec3 i_normal;

#include UniformBuffer/Matrices.glsl

out vec4 io_vertexPositionLightSpace;
out vec3 io_vertexPosition;
out vec3 io_normal;

uniform mat4 u_modelMatrix;
uniform mat3 u_normalMatrix;

void main()
{
	io_vertexPosition = vec3(u_modelMatrix * vec4(i_position, 1.0));
	io_normal = u_normalMatrix * i_normal;
	io_vertexPositionLightSpace = u_lightSpaceViewProjection * vec4(io_vertexPosition, 1.0);
	gl_Position = u_viewProjection * vec4(io_vertexPosition, 1.0);
}
