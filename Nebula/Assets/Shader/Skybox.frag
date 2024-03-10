#version 460 core

out vec4 o_colour;

in vec3 io_uv;

uniform samplerCube u_cubemap;

void main()
{
	vec3 colour = texture(u_cubemap, io_uv).rgb;

	#include Math/PBR/HDRTonemapping.glsl
	#include Math/PBR/GammaCorrection.glsl

	o_colour = vec4(colour, 1.0);
}
