﻿#version 460 core

in vec3 io_position;

out vec4 o_colour;

uniform samplerCube u_environmentMap;

#include Math/Pi.glsl

void main()
{
	vec3 normal = normalize(io_position);
	vec3 irradiance = vec3(0.0);

	vec3 up = vec3(0.0, 1.0, 0.0);
	vec3 right = normalize(cross(up, normal));
	up = normalize(cross(normal, right));

	float sampleDelta = 0.025;
	float samples = 0.0;

	for (float phi = 0.0; phi < 2.0 * PI; phi += sampleDelta)
	{
		for (float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta)
		{
			vec3 sampleTangent = vec3(sin(theta) * cos(phi), sin(theta) * sin(phi), cos(theta));
			vec3 samplePosition = sampleTangent.x * right + sampleTangent.y * up + sampleTangent.z * normal;

			irradiance += texture(u_environmentMap, samplePosition).rgb * cos(theta) * sin(theta);

			samples++;
		}
	}
	
	irradiance = PI * irradiance * (1.0 / float(samples));

	o_colour = vec4(irradiance, 1.0);
}
