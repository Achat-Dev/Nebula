#version 460 core

in vec4 io_vertexPosition;

uniform vec3 u_lightPosition;
uniform float u_farPlane;

void main()
{
	float lightDistance = length(io_vertexPosition.xyz - u_lightPosition);
	gl_FragDepth = lightDistance / u_farPlane;
}
