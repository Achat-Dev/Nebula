﻿using Nebula.Rendering;

namespace Nebula;

internal static class Cache
{
    internal static readonly CacheObject<int, Cubemap> CubemapCache = new CacheObject<int, Cubemap>();
    internal static readonly CacheObject<string, Model> ModelCache = new CacheObject<string, Model>();
    internal static readonly CacheObject<int, Shader> ShaderCache = new CacheObject<int, Shader>();
    internal static readonly CacheObject<string, Texture> TextureCache = new CacheObject<string, Texture>();
    internal static readonly CacheObject<int, UniformBuffer> UniformBufferCache = new CacheObject<int, UniformBuffer>();

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing cache");

        CubemapCache.Dispose();
        ModelCache.Dispose();
        ShaderCache.Dispose();
        TextureCache.Dispose();
        UniformBufferCache.Dispose();
    }
}
