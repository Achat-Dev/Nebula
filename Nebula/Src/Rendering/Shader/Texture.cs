using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

public class Texture : ICacheable, IDisposable
{
    public enum WrapMode
    {
        ClampToBorder = 33069,
        ClampToEdge = 33071,
        MirroredRepeat = 33648,
        Repeat = 10497,
    }

    public enum FilterMode
    {
        Linear = 9729,
        Nearest = 9728,
    }

    public enum Unit
    {
        Texture0 = 33984,
        Texture1,
        Texture2,
        Texture3,
        Texture4,
        Texture5,
        Texture6,
        Texture7,
        Texture8,
        Texture9,
        Texture10,
        Texture11,
        Texture12,
        Texture13,
        Texture14,
        Texture15,
        Texture16,
        Texture17,
        Texture18,
        Texture19,
        Texture20,
        Texture21,
        Texture22,
        Texture23,
        Texture24,
        Texture25,
        Texture26,
        Texture27,
        Texture28,
        Texture29,
        Texture30,
        Texture31,
    }

    private readonly uint r_handle;
    private readonly uint r_width;
    private readonly uint r_height;

    private unsafe Texture(string path, WrapMode wrapMode, FilterMode filterMode)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Unit.Texture0);

        ImageResult result = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(path, out int dataSize), ColorComponents.RedGreenBlueAlpha);

        r_width = (uint)result.Width;
        r_height = (uint)result.Height;

        fixed (void* d = result.Data)
        {
            GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, r_width, r_height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
        }

        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

        GL.Get().GenerateMipmap(TextureTarget.Texture2D);
    }

    public static Texture Create(string path, WrapMode wrapMode, FilterMode filterMode)
    {
        if (Cache.TextureCache.GetValue(path, out Texture texture))
        {
            Logger.EngineDebug($"Texture from path \"{path}\" already exists, returning cached instance");
            return texture;
        }

        Logger.EngineDebug($"Creating texture from path \"{path}\" with wrap mode {wrapMode} and filter mode {filterMode}");
        texture = new Texture(path, wrapMode, filterMode);
        Cache.TextureCache.CacheData(path, texture);
        return texture;
    }

    public uint GetWidth()
    {
        return r_width;
    }

    public uint GetHeight()
    {
        return r_height;
    }

    internal void Bind(Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public void Delete()
    {
        string key = Cache.TextureCache.GetKey(this);

        Logger.EngineDebug($"Deleting texture loaded from path {key}");

        IDisposable disposable = this;
        disposable.Dispose();

        Cache.TextureCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
