#version 460 core

layout (triangles) in;
layout (triangle_strip, max_vertices = 18) out;

out vec4 io_vertexPosition;

uniform mat4 u_viewProjections[6];

void main()
{
    for (int face = 0; face < 6; face++)
    {
        gl_Layer = face;
        for (int i = 0; i < 3; i++)
        {
            io_vertexPosition = gl_in[i].gl_Position;
            gl_Position = u_viewProjections[face] * io_vertexPosition;
            EmitVertex();
        }
        EndPrimitive();
    }
}