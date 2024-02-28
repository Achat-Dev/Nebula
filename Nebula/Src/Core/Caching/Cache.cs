using Nebula.Rendering;

namespace Nebula;

internal static class Cache
{
    internal static readonly CacheObject<string, Cubemap> CubemapCache = new CacheObject<string, Cubemap>();
    internal static readonly CacheObject<string, Model> ModelCache = new CacheObject<string, Model>();
    internal static readonly CacheObject<(string, string), Shader> ShaderCache = new CacheObject<(string, string), Shader>();
    internal static readonly CacheObject<string, Texture> TextureCache = new CacheObject<string, Texture>();
    internal static readonly CacheObject<int, UniformBuffer> UniformBufferCache = new CacheObject<int, UniformBuffer>();

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing cache");

        CubemapCache.Dispose();
        ModelCache.Dispose();
        ShaderCache.Dispose();
        UniformBufferCache.Dispose();
        TextureCache.Dispose();
    }
}
