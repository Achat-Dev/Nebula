namespace Nebula.Rendering;

public abstract class TextureConfigBase
{
    public Texture.Format Format;
    public Texture.DataType DataType;
    public Texture.WrapMode WrapMode;
    public Texture.FilterMode MinFilterMode;
    public Texture.FilterMode MaxFilterMode;
    public bool GenerateMipMaps;
    public int MaxMipMapLevel;

    // Needed for private constructors in inheriting classes
    protected TextureConfigBase() { }

    protected TextureConfigBase(Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode filterMode, bool generateMipMaps, int maxMipMapLevel)
    {
        Format = format;
        DataType = dataType;
        WrapMode = wrapMode;
        MinFilterMode = filterMode;
        MaxFilterMode = filterMode;
        GenerateMipMaps = generateMipMaps;
        MaxMipMapLevel = maxMipMapLevel;
    }

    protected TextureConfigBase(Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode minFilterMode, Texture.FilterMode maxFilterMode, bool generateMipMaps, int maxMipMapLevel)
    {
        Format = format;
        DataType = dataType;
        WrapMode = wrapMode;
        MinFilterMode = minFilterMode;
        MaxFilterMode = maxFilterMode;
        GenerateMipMaps = generateMipMaps;
        MaxMipMapLevel = maxMipMapLevel;
    }

    public Silk.NET.OpenGL.InternalFormat GetInternalFormat()
    {
        switch (Format)
        {
            case Texture.Format.R:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Red;
                    case Texture.DataType.Float: return Silk.NET.OpenGL.InternalFormat.R16f;
                    default: return Silk.NET.OpenGL.InternalFormat.Red;
                }
            case Texture.Format.Rg:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.RG;
                    case Texture.DataType.Float: return Silk.NET.OpenGL.InternalFormat.RG16f;
                    default: return Silk.NET.OpenGL.InternalFormat.RG;
                }
            // Rgb and Hdr are aliases
            // | Texture.Format.Hdr == Texture.Format.Rgb
            case Texture.Format.Rgb:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Rgb;
                    case Texture.DataType.Float: return Silk.NET.OpenGL.InternalFormat.Rgb16f;
                    default: return Silk.NET.OpenGL.InternalFormat.Rgb;
                }
            case Texture.Format.Rgba:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Rgba;
                    case Texture.DataType.Float: return Silk.NET.OpenGL.InternalFormat.Rgba16f;
                    default: return Silk.NET.OpenGL.InternalFormat.Rgba;
                }
            case Texture.Format.Depth:
                return Silk.NET.OpenGL.InternalFormat.DepthComponent;
            case Texture.Format.DepthStencil:
                return Silk.NET.OpenGL.InternalFormat.DepthStencil;
            default:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Rgb;
                    case Texture.DataType.Float: return Silk.NET.OpenGL.InternalFormat.Rgb16f;
                    default: return Silk.NET.OpenGL.InternalFormat.Rgb;
                }
        }
    }

    public Silk.NET.OpenGL.PixelFormat GetPixelFormat()
    {
        return (Silk.NET.OpenGL.PixelFormat)Format;
    }

    public Silk.NET.OpenGL.PixelType GetPixelType()
    {
        return (Silk.NET.OpenGL.PixelType)DataType;
    }

    public StbImageSharp.ColorComponents GetColorComponents()
    {
        switch (Format)
        {
            case Texture.Format.R:
                return StbImageSharp.ColorComponents.Grey;
            case Texture.Format.Rg:
                return StbImageSharp.ColorComponents.RedGreenBlue;
            case Texture.Format.Rgb:
                return StbImageSharp.ColorComponents.RedGreenBlue;
            case Texture.Format.Rgba:
                return StbImageSharp.ColorComponents.RedGreenBlueAlpha;
            default:
                return StbImageSharp.ColorComponents.RedGreenBlue;
        }
    }
}
