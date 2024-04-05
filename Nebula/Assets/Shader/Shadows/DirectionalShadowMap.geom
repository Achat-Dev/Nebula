#version 460 core

#replace CASCADE_COUNT

layout (triangles, invocations = CASCADE_COUNT) in;
layout (triangle_strip, max_vertices = 3) out;

#include UniformBuffer/Csm.glsl

void main()
{
	for (int i = 0; i < 3; i++)
    {
        gl_Position = u_directionalLightViewProjections[gl_InvocationID] * gl_in[i].gl_Position;
        gl_Layer = gl_InvocationID;
        EmitVertex();
    }
    EndPrimitive();
}
