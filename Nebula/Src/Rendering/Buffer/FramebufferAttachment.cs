using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class FramebufferAttachment : IDisposable, ITextureBindable
{
    public enum AttachmentType
    {
        Colour,
        Depth,
        DepthStencil,
    }

    public enum TextureType
    {
        Texture = 0,
        Cubemap = 1,
        Renderbuffer = 2,

        Readable = 0,
        Writeonly = 2,
    }

    private readonly uint r_handle;

    private readonly AttachmentType r_attachmentType;
    private readonly TextureType r_textureType;
    private readonly InternalFormat r_internalFormat;
    private readonly PixelFormat r_pixelFormat;
    private readonly PixelType r_pixelType;

    internal unsafe FramebufferAttachment(FramebufferAttachmentConfig config, Vector2i size)
    {
        r_attachmentType = config.AttachmentType;
        r_textureType = config.TextureType;
        r_internalFormat = config.GetInternalFormat();
        r_pixelFormat = config.GetPixelFormat();
        r_pixelType = config.GetPixelType();

        int wrapMode = (int)config.WrapMode;
        switch (r_textureType)
        {
            case TextureType.Texture:
                r_handle = GL.Get().GenTexture();
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);

                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);

                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapMode);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapMode);

                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)config.MinFilterMode);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)config.MaxFilterMode);

                if (config.GenerateMipMaps)
                {
                    GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
                    GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, config.MaxMipMapLevel);
                    GL.Get().GenerateMipmap(TextureTarget.Texture2D);
                }
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);

                GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), TextureTarget.Texture2D, r_handle, 0);
                break;
            case TextureType.Cubemap:
                r_handle = GL.Get().GenTexture();
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);

                for (int i = 0; i < 6; i++)
                {
                    GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
                }

                GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, wrapMode);
                GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, wrapMode);
                GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, wrapMode);

                GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)config.MinFilterMode);
                GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)config.MaxFilterMode);
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, 0);

                GL.Get().FramebufferTexture(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), r_handle, 0);
                break;
            case TextureType.Renderbuffer:
                r_handle = GL.Get().GenRenderbuffer();

                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_internalFormat, (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), RenderbufferTarget.Renderbuffer, r_handle);
                break;
        }
    }

    internal unsafe void Resize(Vector2i size)
    {
        switch (r_textureType)
        {
            case TextureType.Texture:
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);
                break;
            case TextureType.Cubemap:
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
                for (int i = 0; i < 6; i++)
                {
                    GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
                }
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, 0);
                break;
            case TextureType.Renderbuffer:
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_internalFormat, (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                break;
        }
    }

    public void Bind(Texture.Unit textureUnit)
    {
        if (r_textureType == TextureType.Renderbuffer)
        {
            Logger.EngineError("Trying to bind a writeonly framebuffer attachment");
            return;
        }
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public uint GetHandle()
    {
        if (r_textureType == TextureType.Renderbuffer)
        {
            Logger.EngineError("Trying to access a writeonly framebuffer attachment");
            return 0;
        }
        return r_handle;
    }

    internal AttachmentType GetAttachmentType()
    {
        return r_attachmentType;
    }

    void IDisposable.Dispose()
    {
        switch (r_textureType)
        {
            case TextureType.Texture:
                GL.Get().DeleteTexture(r_handle);
                break;
            case TextureType.Cubemap:
                GL.Get().DeleteTexture(r_handle);
                break;
            case TextureType.Renderbuffer:
                GL.Get().DeleteRenderbuffer(r_handle);
                break;
        }
    }
}
