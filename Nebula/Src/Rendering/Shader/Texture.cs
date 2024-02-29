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

    public enum Format
    {
        Rgb,
        Rgba,
        Hdr,
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

    private Texture(string path, WrapMode wrapMode, FilterMode filterMode, Format format)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Unit.Texture0);

        switch (format)
        {
            case Format.Rgb:
                CreateRgbTexture(path, wrapMode, filterMode);
                break;
            case Format.Rgba:
                CreateRgbaTexture(path, wrapMode, filterMode);
                break;
            case Format.Hdr:
                CreateHdrTexture(path, wrapMode, filterMode);
                break;
        }
    }

    public static Texture Create(string path, WrapMode wrapMode, FilterMode filterMode, Format format)
    {
        if (Cache.TextureCache.GetValue(path, out Texture texture))
        {
            Logger.EngineDebug($"Texture from path \"{path}\" already exists, returning cached instance");
            return texture;
        }

        Logger.EngineDebug($"Creating texture from path \"{path}\" with wrap mode {wrapMode}, filter mode {filterMode} and format {format}");
        texture = new Texture(path, wrapMode, filterMode, format);
        Cache.TextureCache.CacheData(path, texture);
        return texture;
    }

    private unsafe void CreateRgbTexture(string path, WrapMode wrapMode, FilterMode filterMode)
    {
        ImageResult imageResult = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(path, out _), ColorComponents.RedGreenBlue);

        fixed (void* d = imageResult.Data)
        {
            GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)imageResult.Width, (uint)imageResult.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, d);
        }

        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

        GL.Get().GenerateMipmap(TextureTarget.Texture2D);
    }

    private unsafe void CreateRgbaTexture(string path, WrapMode wrapMode, FilterMode filterMode)
    {
        ImageResult imageResult = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(path, out _), ColorComponents.RedGreenBlueAlpha);

        fixed (void* d = imageResult.Data)
        {
            GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imageResult.Width, (uint)imageResult.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
        }

        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);

        GL.Get().GenerateMipmap(TextureTarget.Texture2D);
    }

    private unsafe void CreateHdrTexture(string path, WrapMode wrapMode, FilterMode filterMode)
    {
        ImageResultFloat imageResultFloat = ImageResultFloat.FromMemory(AssetLoader.LoadAsByteArray(path, out _), ColorComponents.RedGreenBlue);

        fixed (void* d = imageResultFloat.Data)
        {
            GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb16f, (uint)imageResultFloat.Width, (uint)imageResultFloat.Height, 0, PixelFormat.Rgb, PixelType.Float, d);
        }

        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filterMode);
    }

    internal void Bind(Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public void Delete()
    {
        string key = Cache.TextureCache.GetKey(this);

        Logger.EngineDebug($"Deleting texture loaded from path \"{key}\"");

        IDisposable disposable = this;
        disposable.Dispose();

        Cache.TextureCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
