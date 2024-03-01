#version 460 core

#include Shader/Include/Math/Pi.glsl

in vec3 io_position;

out vec4 o_colour;

uniform samplerCube u_environmentMap;

void main()
{
	vec3 position = normalize(io_position);
	vec3 irradiance = vec3(0.0);

	vec3 up = vec3(0.0, 1.0, 0.0);
	vec3 right = normalize(cross(up, position));
	up = normalize(cross(position, right));

	float sampleDelta = 0.025;
	float samples = 0.0;

	for (float phi = 0.0; phi < 2.0 * PI; phi += sampleDelta)
	{
		for (float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta)
		{
			vec3 sampleTangent = vec3(sin(theta) * cos(phi), sin(theta) * sin(phi), cos(theta));
			vec3 uv = sampleTangent.x * right + sampleTangent.y * up + sampleTangent.z * position;

			irradiance += texture(u_environmentMap, uv).rgb * cos(theta) * sin(theta);

			samples++;
		}
	}
	
	irradiance = PI * irradiance * (1.0 / float(samples));

	o_colour = vec4(irradiance, 1.0);
}
