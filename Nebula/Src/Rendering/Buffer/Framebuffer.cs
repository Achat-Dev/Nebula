using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class Framebuffer : IDisposable
{
    private uint m_handle;
    private readonly FramebufferAttachment[] r_attachments;

    public Framebuffer(Vector2i size, params FramebufferAttachment[] attachments)
    {
        r_attachments = attachments;
        Resize(size);
    }

    internal unsafe void Resize(Vector2i size)
    {
        if (size.X == 0 || size.Y == 0)
        {
            Logger.EngineWarn($"Trying to resize a framebuffer to {size}, a size of 0 is not allowed");
            return;
        }

        if (m_handle != 0)
        {
            Dispose();
        }

        m_handle = GL.Get().GenFramebuffer();
        Bind();

        for (int i = 0; i < r_attachments.Length; i++)
        {
            r_attachments[i].Resize(size);
        }

        if (GL.Get().CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
        {
            Logger.EngineError("Failed to create framebuffer, framebuffer is incomplete");
        }

        Unbind();
    }

    internal void Bind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, m_handle);
    }

    internal void Unbind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public uint GetAttachment(FramebufferAttachment.AttachmentType attachmentType)
    {
        for (int i = 0; i < r_attachments.Length; i++)
        {
            if (r_attachments[i].GetAttachmentType() == attachmentType)
            {
                return r_attachments[i].GetHandle();
            }
        }
        Logger.EngineError($"Framebuffer doesn't have an attachment of type {attachmentType}");
        return 0;
    }

    public void Dispose()
    {
        for (int i = 0; i < r_attachments.Length; i++)
        {
            r_attachments[i].Dispose();
        }
        GL.Get().DeleteFramebuffer(m_handle);
    }
}
