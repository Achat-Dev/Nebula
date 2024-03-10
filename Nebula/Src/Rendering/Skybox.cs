namespace Nebula.Rendering;

public class Skybox : IDisposable
{
    private readonly Cubemap r_environmentMap;
    private readonly Cubemap r_irradianceMap;
    private readonly Cubemap r_prefilteredMap;
    private readonly Texture r_brdfLutHandle;

    public Skybox(Texture hdrTexture, SkyboxConfig config)
    {
        r_environmentMap = Cubemap.Create(hdrTexture, Cubemap.CubemapType.Skybox, config.EnvironmentMapSize);
        r_irradianceMap = Cubemap.Create(r_environmentMap, Cubemap.CubemapType.Irradiance, config.IrradianceMapSize);
        r_prefilteredMap = Cubemap.Create(r_environmentMap, Cubemap.CubemapType.Prefiltered, config.PrefilteredMapSize);
        TextureConfig brdfLutConfig = TextureConfig.DefaultHdr;
        brdfLutConfig.GenerateMipMaps = false;
        r_brdfLutHandle = Texture.CreateFromCapture(Shader.Create("Shader/Brdf.vert", "Shader/Brdf.frag", false), config.EnvironmentMapSize, brdfLutConfig);
    }

    internal Cubemap GetEnvironmentMap()
    {
        return r_environmentMap;
    }

    internal Cubemap GetIrradianceMap()
    {
        return r_irradianceMap;
    }

    internal Cubemap GetPrefilteredMap()
    {
        return r_prefilteredMap;
    }

    internal Texture GetBrdfLut()
    {
        return r_brdfLutHandle;
    }

    public void Dispose()
    {
        r_environmentMap.Delete();
        r_irradianceMap.Delete();
        r_prefilteredMap.Delete();
        r_brdfLutHandle.Delete();
    }
}
