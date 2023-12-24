#version 330 core

out vec4 o_colour;

uniform vec3 u_colour;

void main()
{
	o_colour = vec4(u_colour, 1.0);
}
