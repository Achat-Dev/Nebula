using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal struct FramebufferAttachmentConfig
{
    public FramebufferAttachment.AttachmentType AttachmentType;
    public FramebufferAttachment.ReadWriteMode ReadWriteMode;
    internal Silk.NET.OpenGL.FramebufferAttachment Attachment;
    internal InternalFormat InternalFormat;
    internal PixelFormat PixelFormat;
    internal PixelType PixelType;

    public FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType attachmentType, FramebufferAttachment.ReadWriteMode readWriteMode)
    {
        AttachmentType = attachmentType;
        ReadWriteMode = readWriteMode;

        switch (attachmentType)
        {
            case FramebufferAttachment.AttachmentType.Colour:
                Attachment = Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0;
                InternalFormat = InternalFormat.Rgb;
                PixelFormat = PixelFormat.Rgb;
                PixelType = PixelType.UnsignedByte;
                break;
            case FramebufferAttachment.AttachmentType.Depth:
                Attachment = Silk.NET.OpenGL.FramebufferAttachment.DepthStencilAttachment;
                InternalFormat = InternalFormat.Depth24Stencil8;
                PixelFormat = PixelFormat.DepthStencil;
                PixelType = PixelType.UnsignedInt248;
                break;
        }
    }
}
