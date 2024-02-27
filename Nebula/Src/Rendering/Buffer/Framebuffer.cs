using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class Framebuffer : IDisposable
{
    private readonly uint r_handle;
    private readonly uint r_colourAttachement;
    private readonly uint r_rbo;

    public unsafe Framebuffer(Vector2i size)
    {
        r_handle = GL.Get().GenFramebuffer();
        Bind();

        // Create colour attachement
        r_colourAttachement = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.Texture2D, r_colourAttachement);
        GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)size.X, (uint)size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Texture.FilterMode.Linear);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Texture.FilterMode.Linear);
        GL.Get().BindTexture(TextureTarget.Texture2D, 0);

        GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, r_colourAttachement, 0);

        // Create render buffer object
        // | This includes a depth and a stencil attachment in one buffer
        // | But it is writeonly
        r_rbo = GL.Get().GenRenderbuffer();
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_rbo);
        GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, (uint)size.X, (uint)size.Y);
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, r_rbo);

        if (GL.Get().CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
        {
            Logger.EngineError("Failed to create framebuffer, framebuffer is incomplete");
        }

        Unbind();
    }

    internal void Bind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, r_handle);
    }

    internal void Unbind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public uint GetColourAttachment()
    {
        return r_colourAttachement;
    }

    public void Dispose()
    {
        GL.Get().DeleteFramebuffer(r_handle);
    }
}
