using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class RawVertexArrayObject : IDisposable
{
    private readonly uint r_handle;
    private readonly uint r_vertexCount;
    private readonly BufferObject<float> r_vbo;

    public unsafe RawVertexArrayObject(BufferObject<float> vertexBuffer, BufferLayout bufferLayout)
    {
        r_vbo = vertexBuffer;
        r_handle = GL.Get().GenVertexArray();

        Bind();

        uint vertexCount = 0;
        BufferElement[] bufferElements = bufferLayout.GetElements();
        for (uint i = 0; i < bufferElements.Length; i++)
        {
            vertexCount += (uint)bufferElements[i].GetCount();
            GL.Get().VertexAttribPointer(i, bufferElements[i].GetCount(), bufferElements[i].GetGLType(), false, bufferLayout.GetByteSize(), (void*)bufferElements[i].Offset);
            GL.Get().EnableVertexAttribArray(i);
        }
        r_vertexCount = vertexBuffer.GetElementCount() / vertexCount;
    }

    private void Bind()
    {
        GL.Get().BindVertexArray(r_handle);
        r_vbo.Bind();
    }

    public void Dispose()
    {
        r_vbo.Dispose();
        GL.Get().DeleteVertexArray(r_handle);
    }

    public unsafe void Draw()
    {
        Bind();
        GL.Get().DrawArrays(PrimitiveType.Triangles, 0, r_vertexCount);
    }
}
