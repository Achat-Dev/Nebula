﻿#version 460 core

layout (location = 0) in vec3 i_position;
layout (location = 1) in vec3 i_normal;
layout (location = 2) in vec3 i_tangent;
layout (location = 3) in vec2 i_uv;

#include Shader/Include/UB_Matrices.glsl

out vec3 io_vertexPosition;
out vec3 io_normal;
out vec3 io_tangent;
out vec2 io_uv;

uniform mat4 u_model;
uniform mat3 u_modelNormalMatrix;

void main()
{
	io_vertexPosition = vec3(u_model * vec4(i_position, 1.0));
	io_normal = u_modelNormalMatrix * i_normal;
	io_tangent = u_modelNormalMatrix * i_tangent;
	io_uv = i_uv;
	gl_Position = u_viewProjection * vec4(io_vertexPosition, 1.0);
}