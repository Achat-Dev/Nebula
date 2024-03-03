namespace Nebula.Rendering;

internal struct FramebufferAttachmentConfig
{
    public FramebufferAttachment.AttachmentType AttachmentType;
    public FramebufferAttachment.ReadWriteMode ReadWriteMode;

    public FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType attachmentType, FramebufferAttachment.ReadWriteMode readWriteMode)
    {
        AttachmentType = attachmentType;
        ReadWriteMode = readWriteMode;
    }
}
