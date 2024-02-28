using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

internal class Cubemap : ICacheable, IDisposable
{
    private readonly uint r_handle;

    private unsafe Cubemap(params string[] paths)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        ImageResult imageResult;
        for (int i = 0; i < paths.Length; i++)
        {
            imageResult = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(paths[i], out int dataSize), ColorComponents.RedGreenBlue);
            fixed (void* d = imageResult.Data)
            {
                GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgb, (uint)imageResult.Width, (uint)imageResult.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, d);
            }
        }

        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    }

    public static Cubemap Create(string pathRight, string pathLeft, string pathTop, string pathBottom, string pathFront, string pathBack)
    {
        // Caching of cubemaps only uses pathRight as the key right now
        // | The assumption is that the individual images of cubemaps aren't being mixed
        // | But if they are mixed this can lead to retrieving the wrong cubemap from cache
        if (Cache.CubemapCache.GetValue(pathRight, out Cubemap cubemap))
        {
            Logger.EngineDebug($"Cubemap from path \"{pathRight}\", ... already exists, returning cached instance");
            return cubemap;
        }
        Logger.EngineDebug($"Creating cubemap from path \"{pathRight}\", ...");
        cubemap = new Cubemap(pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);
        Cache.CubemapCache.CacheData(pathRight, cubemap);
        return cubemap;
    }

    internal void Bind(Texture.Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
    }

    public void Delete()
    {
        string key = Cache.CubemapCache.GetKey(this);

        Logger.EngineDebug($"Deleting cubemap loaded from path {key}");

        IDisposable disposable = this;
        disposable.Dispose();

        Cache.TextureCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
