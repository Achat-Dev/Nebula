using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class FramebufferAttachment : IDisposable, ITextureBindable
{
    public enum AttachmentType
    {
        Colour,
        Depth,
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
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, config.InternalFormat, (uint)size.X, (uint)size.Y, 0, config.PixelFormat, config.PixelType, null);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Texture.FilterMode.Linear);
                GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Texture.FilterMode.Linear);
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);

                GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, config.Attachment, TextureTarget.Texture2D, r_handle, 0);
                break;
            case ReadWriteMode.Writeonly:
                r_handle = GL.Get().GenRenderbuffer();

                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, config.InternalFormat, (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, config.Attachment, RenderbufferTarget.Renderbuffer, r_handle);
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
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_config.InternalFormat, (uint)size.X, (uint)size.Y, 0, r_config.PixelFormat, r_config.PixelType, null);
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);
                break;
            case ReadWriteMode.Writeonly:
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_config.InternalFormat, (uint)size.X, (uint)size.Y);
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
