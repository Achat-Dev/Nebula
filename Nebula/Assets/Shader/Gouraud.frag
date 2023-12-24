#version 330 core

out vec4 o_colour;

in vec4 io_colour;

void main()
{
	o_colour = io_colour;
}
