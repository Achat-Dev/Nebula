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

    public enum ReadWriteMode
    {
        Readable,
        Writeonly,
    }

    private readonly uint r_handle;

    private readonly AttachmentType r_attachmentType;
    private readonly ReadWriteMode r_readWriteMode;
    private readonly InternalFormat r_internalFormat;
    private readonly PixelFormat r_pixelFormat;
    private readonly PixelType r_pixelType;

    internal unsafe FramebufferAttachment(FramebufferAttachmentConfig config, Vector2i size)
    {
        r_attachmentType = config.AttachmentType;
        r_readWriteMode = config.ReadWriteMode;
        r_internalFormat = config.GetInternalFormat();
        r_pixelFormat = config.GetPixelFormat();
        r_pixelType = config.GetPixelType();

        switch (r_readWriteMode)
        {
            case ReadWriteMode.Readable:
                r_handle = GL.Get().GenTexture();
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);

                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);

                int wrapMode = (int)config.WrapMode;
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapMode);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapMode);

                int filterMode = (int)config.FilterMode;
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filterMode);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filterMode);

                if (config.GenerateMipMaps)
                {
                    GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
                    GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, config.MaxMipMapLevel);
                    GL.Get().GenerateMipmap(TextureTarget.Texture2D);
                }

                GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), TextureTarget.Texture2D, r_handle, 0);
                break;
            case ReadWriteMode.Writeonly:
                r_handle = GL.Get().GenRenderbuffer();

                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_internalFormat, (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), RenderbufferTarget.Renderbuffer, r_handle);
                break;
        }

        Resize(size);
    }

    internal unsafe void Resize(Vector2i size)
    {
        switch (r_readWriteMode)
        {
            case ReadWriteMode.Readable:
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);
                break;
            case ReadWriteMode.Writeonly:
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_internalFormat, (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                break;
        }
    }

    public void Bind(Texture.Unit textureUnit)
    {
        if (r_readWriteMode == ReadWriteMode.Writeonly)
        {
            Logger.EngineError("Trying to bind a writeonly framebuffer attachment");
            return;
        }
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public uint GetHandle()
    {
        if (r_readWriteMode == ReadWriteMode.Writeonly)
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
        switch (r_readWriteMode)
        {
            case ReadWriteMode.Readable:
                GL.Get().DeleteTexture(r_handle);
                break;
            case ReadWriteMode.Writeonly:
                GL.Get().DeleteRenderbuffer(r_handle);
                break;
        }
    }
}
