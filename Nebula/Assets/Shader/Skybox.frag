#version 460 core

out vec4 o_colour;

in vec3 io_uv;

uniform samplerCube u_cubemap;

void main()
{
	o_colour = texture(u_cubemap, io_uv);
}
