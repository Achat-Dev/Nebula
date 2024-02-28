#version 460 core

layout (location = 0) in vec3 i_position;

out vec3 io_uv;

// Can't use the default uniform buffer for this
// | The translation is being removed from the matrix here before uploading it to the shader
uniform mat4 u_viewProjection;

void main()
{
	io_uv = i_position;
	vec4 position = u_viewProjection * vec4(i_position, 1.0);
	gl_Position = position.xyww;
}
