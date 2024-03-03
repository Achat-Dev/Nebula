using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class Framebuffer : IDisposable
{
    private readonly uint r_handle;
    private readonly FramebufferAttachment[] r_attachments;

    public Framebuffer(params FramebufferAttachmentConfig[] attachmentConfigs)
        : this(Game.GetWindowSize(), attachmentConfigs) { }

    public Framebuffer(Vector2i size, params FramebufferAttachmentConfig[] attachmentConfigs)
    {
        r_handle = GL.Get().GenFramebuffer();
        Bind();

        r_attachments = new FramebufferAttachment[attachmentConfigs.Length];

        for (int i = 0; i < attachmentConfigs.Length; i++)
        {
            FramebufferAttachment attachment = new FramebufferAttachment(size, attachmentConfigs[i].AttachmentType, attachmentConfigs[i].ReadWriteMode);
            r_attachments[i] = attachment;
        }
    }

    internal unsafe void Resize(Vector2i size)
    {
        if (size.X == 0 || size.Y == 0)
        {
            Logger.EngineWarn($"Trying to resize a framebuffer to {size}, a size of 0 is not allowed");
            return;
        }

        Bind();

        for (int i = 0; i < r_attachments.Length; i++)
        {
            r_attachments[i].Resize(size);
        }

        if (GL.Get().CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
        {
            Logger.EngineError("Failed to create framebuffer, framebuffer is incomplete");
        }
    }

    internal void Bind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, r_handle);
    }

    internal void Unbind()
    {
        GL.Get().BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public FramebufferAttachment GetAttachment(FramebufferAttachment.AttachmentType attachmentType)
    {
        for (int i = 0; i < r_attachments.Length; i++)
        {
            if (r_attachments[i].GetAttachmentType() == attachmentType)
            {
                return r_attachments[i];
            }
        }
        Logger.EngineError($"Framebuffer doesn't have an attachment of type {attachmentType}");
        return null;
    }

    void IDisposable.Dispose()
    {
        IDisposable disposable;
        for (int i = 0; i < r_attachments.Length; i++)
        {
            disposable = r_attachments[i];
            disposable.Dispose();
        }
        GL.Get().DeleteFramebuffer(r_handle);
    }
}
