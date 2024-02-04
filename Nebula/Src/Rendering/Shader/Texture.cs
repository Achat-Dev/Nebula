using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

public class Texture : IDisposable
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

    private readonly uint r_width;
    private readonly uint r_height;
    private readonly uint r_handle;

    public unsafe Texture(string path, WrapMode wrapMode, FilterMode filterMode)
    {
        r_handle = GL.Get().GenTexture();
        Bind();

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

    public uint GetWidth()
    {
        return r_width;
    }

    public uint GetHeight()
    {
        return r_height;
    }

    internal void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
    {
        GL.Get().ActiveTexture(textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public void Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
