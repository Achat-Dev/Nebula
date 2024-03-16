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
    private readonly FramebufferAttachmentConfig r_config;

    internal unsafe FramebufferAttachment(FramebufferAttachmentConfig config, Vector2i size)
    {
        r_config = config;

        switch (r_config.ReadWriteMode)
        {
            case ReadWriteMode.Readable:
                r_handle = GL.Get().GenTexture();
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);

                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, config.TextureConfig.GetInternalFormat(), (uint)size.X, (uint)size.Y, 0, config.TextureConfig.GetPixelFormat(), config.TextureConfig.GetPixelType(), null);

                int wrapMode = (int)config.TextureConfig.WrapMode;
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapMode);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapMode);

                int filterMode = (int)config.TextureConfig.FilterMode;
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filterMode);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filterMode);

                if (config.TextureConfig.GenerateMipMaps)
                {
                    GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
                    GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, config.TextureConfig.MaxMipMapLevel);
                    GL.Get().GenerateMipmap(TextureTarget.Texture2D);
                }

                GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), TextureTarget.Texture2D, r_handle, 0);
                break;
            case ReadWriteMode.Writeonly:
                r_handle = GL.Get().GenRenderbuffer();

                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, config.TextureConfig.GetInternalFormat(), (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), RenderbufferTarget.Renderbuffer, r_handle);
                break;
        }

        Resize(size);
    }

    internal unsafe void Resize(Vector2i size)
    {
        switch (r_config.ReadWriteMode)
        {
            case ReadWriteMode.Readable:
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_config.TextureConfig.GetInternalFormat(), (uint)size.X, (uint)size.Y, 0, r_config.TextureConfig.GetPixelFormat(), r_config.TextureConfig.GetPixelType(), null);
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);
                break;
            case ReadWriteMode.Writeonly:
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_config.TextureConfig.GetInternalFormat(), (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                break;
        }
    }

    public void Bind(Texture.Unit textureUnit)
    {
        if (r_config.ReadWriteMode == ReadWriteMode.Writeonly)
        {
            Logger.EngineError("Trying to bind a writeonly framebuffer attachment");
            return;
        }
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public uint GetHandle()
    {
        if (r_config.ReadWriteMode == ReadWriteMode.Writeonly)
        {
            Logger.EngineError("Trying to access a writeonly framebuffer attachment");
            return 0;
        }
        return r_handle;
    }

    internal AttachmentType GetAttachmentType()
    {
        return r_config.AttachmentType;
    }

    void IDisposable.Dispose()
    {
        switch (r_config.ReadWriteMode)
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
