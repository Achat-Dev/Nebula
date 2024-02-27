using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class Framebuffer : IDisposable
{
    private uint m_handle;
    private uint m_colourAttachement;
    private uint m_rbo;

    public Framebuffer(Vector2i size)
    {
        Recreate(size);
    }

    private unsafe void Recreate(Vector2i size)
    {
        m_handle = GL.Get().GenFramebuffer();
        Bind();

        // Create colour attachement
        m_colourAttachement = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.Texture2D, m_colourAttachement);
        GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)size.X, (uint)size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Texture.FilterMode.Linear);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Texture.FilterMode.Linear);
        GL.Get().BindTexture(TextureTarget.Texture2D, 0);

        GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, m_colourAttachement, 0);

        // Create render buffer object
        // | This includes a depth and a stencil attachment in one buffer
        // | But it is writeonly
        m_rbo = GL.Get().GenRenderbuffer();
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, m_rbo);
        GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, (uint)size.X, (uint)size.Y);
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, m_rbo);

        if (GL.Get().CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
        {
            Logger.EngineError("Failed to create framebuffer, framebuffer is incomplete");
        }

        Unbind();

        Window.Resizing += OnResize;
    }

    internal void Bind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, m_handle);
    }

    internal void Unbind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    private void OnResize(Vector2i size)
    {
        Dispose();
        Recreate(size);
    }

    public uint GetColourAttachment()
    {
        return m_colourAttachement;
    }

    public void Dispose()
    {
        GL.Get().DeleteTexture(m_colourAttachement);
        GL.Get().DeleteRenderbuffer(m_rbo);
        GL.Get().DeleteFramebuffer(m_handle);
        Window.Resizing -= OnResize;
    }
}
