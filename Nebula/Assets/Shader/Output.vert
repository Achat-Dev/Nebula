﻿#version 460 core

layout (location = 0) in vec3 i_position;
layout (location = 1) in vec2 i_uv;

out vec2 io_uv;

void main()
{
	io_uv = i_uv;
	gl_Position = vec4(i_position, 1.0);
}
