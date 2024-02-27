using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal struct FramebufferAttachment : IDisposable
{
    public enum AttachmentType
    {
        Colour,
        Depth,
    }

    public enum ReadWriteMode
    {
        Writeonly,
        Readable,
    }

    private uint m_handle;
    private readonly AttachmentType r_attachmentType;
    private readonly ReadWriteMode r_readWriteMode;

    public unsafe FramebufferAttachment(Vector2i size, AttachmentType attachmentType, ReadWriteMode readWriteMode)
    {
        r_attachmentType = attachmentType;
        r_readWriteMode = readWriteMode;

        Resize(size);
    }

    public void Resize(Vector2i size)
    {
        if (m_handle != 0)
        {
            Dispose();
        }

        switch (r_attachmentType)
        {
            case AttachmentType.Colour:
                CreateTextureAttachment(size);
                break;
            case AttachmentType.Depth:
                CreateRenderbufferAttachment(size);
                break;
        }
    }

    private unsafe void CreateTextureAttachment(Vector2i size)
    {
        m_handle = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.Texture2D, m_handle);
        GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)size.X, (uint)size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Texture.FilterMode.Linear);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Texture.FilterMode.Linear);
        GL.Get().BindTexture(TextureTarget.Texture2D, 0);

        GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, m_handle, 0);
    }

    private unsafe void CreateRenderbufferAttachment(Vector2i size)
    {
        m_handle = GL.Get().GenRenderbuffer();
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, m_handle);
        GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, (uint)size.X, (uint)size.Y);
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, m_handle);
    }

    public uint GetHandle()
    {
        if (r_readWriteMode == ReadWriteMode.Writeonly)
        {
            Logger.EngineError("Trying to access a writeonly framebuffer attachment");
            return 0;
        }
        return m_handle;
    }

    public AttachmentType GetAttachmentType()
    {
        return r_attachmentType;
    }

    public void Dispose()
    {
        switch (r_attachmentType)
        {
            case AttachmentType.Colour:
                GL.Get().DeleteTexture(m_handle);
                break;
            case AttachmentType.Depth:
                GL.Get().DeleteRenderbuffer(m_handle);
                break;
        }
    }
}
