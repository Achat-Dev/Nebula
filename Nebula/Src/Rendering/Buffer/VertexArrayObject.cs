using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class VertexArrayObject : IDisposable
{
    private readonly uint r_handle;
    private readonly uint r_indexCount;

    public VertexArrayObject(BufferObject<float> vertexBuffer, BufferObject<uint> indexBuffer, BufferLayout bufferLayout)
    {
        r_handle = GL.Get().GenVertexArray();
        r_indexCount = indexBuffer.GetElementCount();

        Bind();
        vertexBuffer.Bind();
        indexBuffer.Bind();

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
    }

    public void Dispose()
    {
        GL.Get().DeleteVertexArray(r_handle);
    }

    public uint GetIndexCount()
    {
        return r_indexCount;
    }
}
