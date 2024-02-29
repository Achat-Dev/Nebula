#version 460 core

#include Shader/Include/InverseAtan.glsl

in vec3 io_position;

out vec4 o_colour;

uniform sampler2D u_equirectangularMap;

vec2 sampleEquirectangularMap(vec3 v)
{
	vec2 uv = vec2(atan(v.z, v.x), asin(v.y));
	uv *= inverseAtan;
	uv += 0.5;
	return uv;
}

void main()
{
	vec2 uv = sampleEquirectangularMap(normalize(io_position));
	vec3 colour = texture(u_equirectangularMap, uv).rgb;
	o_colour = vec4(colour, 1.0);
}
