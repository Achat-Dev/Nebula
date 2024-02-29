using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

public class HDRTexture : IDisposable
{
    private readonly uint r_handle;

    private unsafe HDRTexture(string path, Texture.WrapMode wrapMode, Texture.FilterMode filterMode)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        ImageResultFloat imageResultFloat = ImageResultFloat.FromMemory(AssetLoader.LoadAsByteArray(path, out _), ColorComponents.RedGreenBlue);

        fixed(void* d = imageResultFloat.Data)
        {
            GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb16f, (uint)imageResultFloat.Width, (uint)imageResultFloat.Height, 0, PixelFormat.Rgb, PixelType.Float, d);
        }

        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filterMode);
    }

    public static HDRTexture Create(string path, Texture.WrapMode wrapMode, Texture.FilterMode filterMode)
    {
        return new HDRTexture(path, wrapMode, filterMode);
    }

    internal void Bind(Texture.Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public void Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
