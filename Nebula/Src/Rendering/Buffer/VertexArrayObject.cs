using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class VertexArrayObject : IDisposable
{
    private readonly uint r_handle;
    private readonly BufferObject<float> r_vbo;
    private readonly BufferObject<uint> r_ibo;

    public unsafe VertexArrayObject(BufferObject<float> vertexBuffer, BufferObject<uint> indexBuffer, BufferLayout bufferLayout)
    {
        r_vbo = vertexBuffer;
        r_ibo = indexBuffer;
        r_handle = GL.Get().GenVertexArray();

        Bind();

        BufferElement[] bufferElements = bufferLayout.GetElements();
        for (uint i = 0; i < bufferElements.Length; i++)
        {
            GL.Get().VertexAttribPointer(i, bufferElements[i].GetCount(), bufferElements[i].GetGLType(), false, bufferLayout.GetByteSize(), (void*)bufferElements[i].Offset);
            GL.Get().EnableVertexAttribArray(i);
        }
    }

    private void Bind()
    {
        GL.Get().BindVertexArray(r_handle);
        r_vbo.Bind();
        r_ibo.Bind();
    }

    public void Dispose()
    {
        r_vbo.Dispose();
        r_ibo.Dispose();
        GL.Get().DeleteVertexArray(r_handle);
    }

    public unsafe void Draw()
    {
        Bind();
        GL.Get().DrawElements(PrimitiveType.Triangles, r_ibo.GetElementCount(), DrawElementsType.UnsignedInt, null);
    }
}
