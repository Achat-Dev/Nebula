using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class BufferObject<T> : IDisposable
{
    private readonly uint r_handle;
    private readonly uint r_elementCount;
    private readonly BufferTargetARB r_bufferTarget;

    public unsafe BufferObject(Span<T> data, BufferTargetARB bufferTarget)
    {
        r_handle = GL.Get().GenBuffer();
        r_elementCount = (uint)data.Length;
        r_bufferTarget = bufferTarget;
        Bind();

        fixed (void* d = data)
        {
            GL.Get().BufferData(bufferTarget, (nuint)(data.Length * sizeof(T)), d, BufferUsageARB.StaticDraw);
        }
    }

    public void Bind()
    {
        GL.Get().BindBuffer(r_bufferTarget, r_handle);
    }

    public void Dispose()
    {
        GL.Get().DeleteBuffer(r_handle);
    }

    public uint GetElementCount()
    {
        return r_elementCount;
    }
}
