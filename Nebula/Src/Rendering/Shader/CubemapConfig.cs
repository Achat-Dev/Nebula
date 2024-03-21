namespace Nebula.Rendering;

internal class CubemapConfig : TextureConfigBase
{
    public Cubemap.CubemapType CubemapType;

    private CubemapConfig() { }

    public CubemapConfig(Cubemap.CubemapType cubemapType, Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode filterMode, bool generateMipMaps, int maxMipMapLevel)
        : this(cubemapType, format, dataType, wrapMode, filterMode, filterMode, generateMipMaps, maxMipMapLevel)
    {

    }

    public CubemapConfig(Cubemap.CubemapType cubemapType, Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode minFilterMode, Texture.FilterMode maxFilterMode, bool generateMipMaps, int maxMipMapLevel)
    : base(format, dataType, wrapMode, minFilterMode, maxFilterMode, generateMipMaps, maxMipMapLevel)
    {
        CubemapType = cubemapType;
    }
}
