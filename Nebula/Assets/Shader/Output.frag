#version 460 core

out vec4 o_colour;

in vec2 io_uv;

uniform sampler2D u_framebuffer;

void main()
{
	vec3 colour = texture(u_framebuffer, io_uv).xyz;
	o_colour = vec4(colour, 1.0);
}
