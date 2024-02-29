#version 460 core

out vec4 o_colour;

in vec3 io_uv;

uniform samplerCube u_cubemap;

void main()
{
	vec3 colour = texture(u_cubemap, io_uv).rgb;

	// HDR tonemapping
	colour = colour / (colour + vec3(1.0));

	// Gamma correction
	colour = pow(colour, vec3(1.0/2.2));

	o_colour = vec4(colour, 1.0);
}
