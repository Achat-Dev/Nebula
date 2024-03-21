namespace Nebula.Rendering;

public class TextureConfig : TextureConfigBase
{
    public class Defaults
    {
        public static TextureConfig Rgb()
        {
            return new TextureConfig(Texture.Format.Rgb, Texture.DataType.UnsignedByte, Texture.WrapMode.Repeat, Texture.FilterMode.Linear, true, 8);
        }

        public static TextureConfig Rgba()
        {
            return new TextureConfig(Texture.Format.Rgba, Texture.DataType.UnsignedByte, Texture.WrapMode.Repeat, Texture.FilterMode.Linear, true, 8);
        }

        public static TextureConfig Hdr()
        {
            return new TextureConfig(Texture.Format.Hdr, Texture.DataType.Float, Texture.WrapMode.Repeat, Texture.FilterMode.Linear, true, 8);
        }
    }

    private TextureConfig() { }

    public TextureConfig(Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode filterMode, bool generateMipMaps, int maxMipMapLevel)
        : base(format, dataType, wrapMode, filterMode, generateMipMaps, maxMipMapLevel)
    {

    }
}
