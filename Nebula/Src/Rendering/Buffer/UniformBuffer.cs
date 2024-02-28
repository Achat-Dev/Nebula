﻿using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public class UniformBuffer : ICacheable, IDisposable
{
    internal enum DefaultType
    {
        Matrices,
        Camera,
        Lights,
    }

    private readonly uint r_handle;

    private unsafe UniformBuffer(uint size, int location)
    {
        r_handle = GL.Get().GenBuffer();
        Bind();
        GL.Get().BufferData(BufferTargetARB.UniformBuffer, size, null, BufferUsageARB.StaticDraw);
        Unbind();
        GL.Get().BindBufferBase(BufferTargetARB.UniformBuffer, (uint)location, r_handle);
    }

    public static UniformBuffer Create(int location, params UniformBufferLayout[] bufferLayouts)
    {
        if (Cache.UniformBufferCache.GetValue(location, out _))
        {
            Logger.EngineError($"Couldn't create uniform buffer at location {location} because it already exists.\nEngine reserved uniform locations are 0, 1 and 2.\nReturning null");
            return null;
        }

        uint bufferSize = 0;
        for (int i = 0; i < bufferLayouts.Length; i++)
        {
            bufferSize += bufferLayouts[i].ByteSize;
        }

        Logger.EngineDebug($"Creating uniform buffer at location {location} with a size of {bufferSize} bytes");
        UniformBuffer uniformBuffer = new UniformBuffer(bufferSize, location);
        Cache.UniformBufferCache.CacheData(location, uniformBuffer);
        return uniformBuffer;
    }

    internal static void CreateDefaults()
    {
        Logger.EngineInfo("Creating default uniform buffer objects");
        Create((int)DefaultType.Matrices, new UniformBufferLayout(UniformBufferElement.Mat4, UniformBufferElement.Mat3));
        Create((int)DefaultType.Camera, new UniformBufferLayout(UniformBufferElement.Vec3));
        Create((int)DefaultType.Lights,
            new UniformBufferLayout(UniformBufferElement.Int),
            new UniformBufferLayout(UniformBufferElement.Vec3, UniformBufferElement.Vec3),
            new UniformBufferLayout(128, UniformBufferElement.Float, UniformBufferElement.Vec3, UniformBufferElement.Vec3));
    }

    public static UniformBuffer GetAtLocation(int location)
    {
        if (Cache.UniformBufferCache.GetValue(location, out UniformBuffer uniformBuffer))
        {
            return uniformBuffer;
        }
        Logger.EngineError($"No uniform buffer at location {location} exists");
        return null;
    }

    internal static UniformBuffer GetAtLocation(DefaultType defaultType)
    {
        return GetAtLocation((int)defaultType);
    }

    private void Bind()
    {
        GL.Get().BindBuffer(BufferTargetARB.UniformBuffer, r_handle);
    }

    private void Unbind()
    {
        GL.Get().BindBuffer(BufferTargetARB.UniformBuffer, 0);
    }

    public unsafe void BufferData<T>(int offset, params T[] data)
    {
        Bind();
        fixed (T* d = data)
        {
            GL.Get().BufferSubData(BufferTargetARB.UniformBuffer, offset, (nuint)(sizeof(T) * data.Length), d);
        }
        Unbind();
    }

    public unsafe T[] ReadData<T>(int offset, int length)
    {
        Bind();

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        T[] data = new T[length];
        fixed (void* d = &data[0])
        {
            GL.Get().GetBufferSubData(BufferTargetARB.UniformBuffer, offset, (nuint)length, d);
        }

        return data;
    }

    public void Delete()
    {
        int key = Cache.UniformBufferCache.GetKey(this);

        Logger.EngineDebug($"Deleting uniform buffer at location {key}");
        IDisposable disposable = this;
        disposable.Dispose();

        Cache.UniformBufferCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteBuffer(r_handle);
    }
}
