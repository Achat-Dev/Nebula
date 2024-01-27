namespace Nebula.Rendering;

internal class VertexArrayObject : IDisposable
{
    private readonly BufferObject<Vertex> r_vbo;
    private readonly BufferObject<uint> r_ibo;
    private readonly uint r_handle;

    public VertexArrayObject(BufferObject<Vertex> vertexBuffer, BufferObject<uint> indexBuffer, BufferLayout bufferLayout)
    {
        r_vbo = vertexBuffer;
        r_ibo = indexBuffer;
        r_handle = GL.Get().GenVertexArray();

        Bind();
        SetVertexBufferLayout(bufferLayout);
    }

    private unsafe void SetVertexBufferLayout(BufferLayout bufferLayout)
    {
        BufferElement[] bufferElements = bufferLayout.GetElements();
        for (uint i = 0; i < bufferElements.Length; i++)
        {
            GL.Get().VertexAttribPointer(i, bufferElements[i].GetCount(), bufferElements[i].GetGLType(), false, bufferLayout.GetStride(), (void*)bufferElements[i].Offset);
            GL.Get().EnableVertexAttribArray(i);
        }
    }

    public void Bind()
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

    public uint GetIndexCount()
    {
        return r_ibo.GetElementCount();
    }
}
