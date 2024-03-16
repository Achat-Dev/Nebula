namespace Nebula.Rendering;

internal struct FramebufferAttachmentConfig
{
    public FramebufferAttachment.AttachmentType AttachmentType;
    public FramebufferAttachment.ReadWriteMode ReadWriteMode;
    public TextureConfig TextureConfig;

    public static readonly FramebufferAttachmentConfig DefaultColour = new FramebufferAttachmentConfig(
        FramebufferAttachment.AttachmentType.Colour,
        FramebufferAttachment.ReadWriteMode.Readable,
        new TextureConfig(Texture.Format.Rgb, Texture.DataType.UnsignedByte, Texture.WrapMode.ClampToEdge, Texture.FilterMode.Linear, false, 0));

    public static readonly FramebufferAttachmentConfig DefaultDepth = new FramebufferAttachmentConfig(
        FramebufferAttachment.AttachmentType.Depth,
        FramebufferAttachment.ReadWriteMode.Writeonly,
        new TextureConfig(Texture.Format.Depth, Texture.DataType.Float, Texture.WrapMode.ClampToEdge, Texture.FilterMode.Linear, false, 0));

    public static readonly FramebufferAttachmentConfig DefaultDepthStencil = new FramebufferAttachmentConfig(
        FramebufferAttachment.AttachmentType.DepthStencil,
        FramebufferAttachment.ReadWriteMode.Writeonly,
        new TextureConfig(Texture.Format.DepthStencil, Texture.DataType.UnsignedInt248, Texture.WrapMode.ClampToEdge, Texture.FilterMode.Linear, false, 0));

    public FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType attachmentType, FramebufferAttachment.ReadWriteMode readWriteMode, TextureConfig textureConfig)
    {
        AttachmentType = attachmentType;
        ReadWriteMode = readWriteMode;
        TextureConfig = textureConfig;
    }

    internal Silk.NET.OpenGL.FramebufferAttachment GetSilkAttachment()
    {
        switch (AttachmentType)
        {
            case FramebufferAttachment.AttachmentType.Colour:
                return Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0;
            case FramebufferAttachment.AttachmentType.Depth:
                return Silk.NET.OpenGL.FramebufferAttachment.DepthAttachment;
            case FramebufferAttachment.AttachmentType.DepthStencil:
                return Silk.NET.OpenGL.FramebufferAttachment.DepthStencilAttachment;
            default:
                return Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0;
        }
    }
}
