namespace Nebula.Rendering;

internal class FramebufferAttachmentConfig : TextureConfigBase
{
    public FramebufferAttachment.AttachmentType AttachmentType;
    public FramebufferAttachment.ReadWriteMode ReadWriteMode;

    public class Defaults
    {
        public static FramebufferAttachmentConfig Colour()
        {
            return new FramebufferAttachmentConfig(
                FramebufferAttachment.AttachmentType.Colour,
                FramebufferAttachment.ReadWriteMode.Readable,
                Texture.Format.Rgb,
                Texture.DataType.UnsignedByte,
                Texture.WrapMode.ClampToEdge,
                Texture.FilterMode.Linear,
                false,
                0);
        }

        public static FramebufferAttachmentConfig Depth()
        {
            return new FramebufferAttachmentConfig(
                FramebufferAttachment.AttachmentType.Depth,
                FramebufferAttachment.ReadWriteMode.Writeonly,
                Texture.Format.Depth,
                Texture.DataType.Float,
                Texture.WrapMode.ClampToEdge,
                Texture.FilterMode.Linear,
                false,
                0);
        }

        public static FramebufferAttachmentConfig DepthStencil()
        {
            return new FramebufferAttachmentConfig(
                FramebufferAttachment.AttachmentType.DepthStencil,
                FramebufferAttachment.ReadWriteMode.Writeonly,
                Texture.Format.DepthStencil,
                Texture.DataType.UnsignedInt248,
                Texture.WrapMode.ClampToEdge,
                Texture.FilterMode.Linear,
                false,
                0);
        }
    }

    private FramebufferAttachmentConfig() { }

    public FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType attachmentType, FramebufferAttachment.ReadWriteMode readWriteMode, Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode filterMode, bool generateMipMaps, int maxMipMapLevel)
        : this(attachmentType, readWriteMode, format, dataType, wrapMode, filterMode, filterMode, generateMipMaps, maxMipMapLevel)
    {

    }

    public FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType attachmentType, FramebufferAttachment.ReadWriteMode readWriteMode, Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode minFilterMode, Texture.FilterMode maxFilterMode, bool generateMipMaps, int maxMipMapLevel)
    : base(format, dataType, wrapMode, minFilterMode, maxFilterMode, generateMipMaps, maxMipMapLevel)
    {
        AttachmentType = attachmentType;
        ReadWriteMode = readWriteMode;
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
