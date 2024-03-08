using System.Text;

namespace Nebula.Rendering;

public struct TextureConfig
{
    public Texture.Format Format;
    public Texture.DataType DataType;
    public Texture.WrapMode WrapMode;
    public Texture.FilterMode FilterMode;
    public bool GenerateMipMaps;
    public int MaxMipMapLevel;

    public static readonly TextureConfig DefaultRgb = new TextureConfig(Texture.Format.Rgb, Texture.DataType.UnsignedByte, Texture.WrapMode.Repeat, Texture.FilterMode.Linear, true, 8);
    public static readonly TextureConfig DefaultRgba = new TextureConfig();
    public static readonly TextureConfig DefaultHdr = new TextureConfig(Texture.Format.Hdr, Texture.DataType.Float, Texture.WrapMode.Repeat, Texture.FilterMode.Linear, true, 8);

    public TextureConfig()
    {
        Format = Texture.Format.Rgba;
        DataType = Texture.DataType.UnsignedByte;
        WrapMode = Texture.WrapMode.Repeat;
        FilterMode = Texture.FilterMode.Linear;
        GenerateMipMaps = true;
        MaxMipMapLevel = 8;
    }

    public TextureConfig(Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode filterMode, bool generateMipMaps, int maxMipMapLevel)
    {
        Format = format;
        DataType = dataType;
        WrapMode = wrapMode;
        FilterMode = filterMode;
        GenerateMipMaps = generateMipMaps;
        MaxMipMapLevel = maxMipMapLevel;
    }

    public int GetWrapMode()
    {
        return (int)WrapMode;
    }

    public int GetFilterMode()
    {
        return (int)FilterMode;
    }

    public Silk.NET.OpenGL.InternalFormat GetInternalFormat()
    {
        switch (Format)
        {
            case Texture.Format.R:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Red;
                    case Texture.DataType.Float:        return Silk.NET.OpenGL.InternalFormat.R16f;
                    default:                            return Silk.NET.OpenGL.InternalFormat.Red;
                }
            case Texture.Format.Rg:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.RG;
                    case Texture.DataType.Float:        return Silk.NET.OpenGL.InternalFormat.RG16f;
                    default:                            return Silk.NET.OpenGL.InternalFormat.RG;
                }
            // Rgb and Hdr are aliases
            // | Texture.Format.Hdr == Texture.Format.Rgb
            case Texture.Format.Rgb:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Rgb;
                    case Texture.DataType.Float:        return Silk.NET.OpenGL.InternalFormat.Rgb16f;
                    default:                            return Silk.NET.OpenGL.InternalFormat.Rgb;
                }
            case Texture.Format.Rgba:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Rgba;
                    case Texture.DataType.Float:        return Silk.NET.OpenGL.InternalFormat.Rgba16f;
                    default:                            return Silk.NET.OpenGL.InternalFormat.Rgba;
                }
            default:
                switch (DataType)
                {
                    case Texture.DataType.UnsignedByte: return Silk.NET.OpenGL.InternalFormat.Rgb;
                    case Texture.DataType.Float:        return Silk.NET.OpenGL.InternalFormat.Rgb16f;
                    default:                            return Silk.NET.OpenGL.InternalFormat.Rgb;
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

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("wrap mode: \"");
        stringBuilder.Append(WrapMode);
        stringBuilder.Append("\", filter mode: \"");
        stringBuilder.Append(FilterMode);
        stringBuilder.Append("\", format: \"");
        stringBuilder.Append(Format);
        stringBuilder.Append("\" (Rgb and Hdr are aliases) and data type: \"");
        stringBuilder.Append(DataType);
        if (GenerateMipMaps)
        {
            stringBuilder.Append("\", texture uses mip maps with a max level of \"");
            stringBuilder.Append(MaxMipMapLevel);
            stringBuilder.Append("\"");
        }
        else
        {
            stringBuilder.Append("\", texture doesn't use mip maps");
        }
        return stringBuilder.ToString();
    }
}
