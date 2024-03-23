namespace Nebula.Rendering;

internal class CubemapConfig : TextureConfigBase
{
    public Cubemap.MappingType MappingType;

    private CubemapConfig() { }

    public CubemapConfig(Cubemap.MappingType mappingType, Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode filterMode, bool generateMipMaps, int maxMipMapLevel)
        : this(mappingType, format, dataType, wrapMode, filterMode, filterMode, generateMipMaps, maxMipMapLevel)
    {

    }

    public CubemapConfig(Cubemap.MappingType mappingType, Texture.Format format, Texture.DataType dataType, Texture.WrapMode wrapMode, Texture.FilterMode minFilterMode, Texture.FilterMode maxFilterMode, bool generateMipMaps, int maxMipMapLevel)
    : base(format, dataType, wrapMode, minFilterMode, maxFilterMode, generateMipMaps, maxMipMapLevel)
    {
        MappingType = mappingType;
    }
}
