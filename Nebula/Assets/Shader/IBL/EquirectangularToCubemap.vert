#version 460 core

layout (location = 0) in vec3 i_position;

out vec3 io_position;

uniform mat4 u_projection;
uniform mat4 u_view;

void main()
{
	io_position = i_position;
	gl_Position = u_projection * u_view * vec4(i_position, 1.0);
}
