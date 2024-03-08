namespace Nebula.Rendering;

public class Skybox : IDisposable
{
    private readonly Cubemap r_environmentMap;
    private readonly Cubemap r_irradianceMap;
    private readonly Cubemap r_prefilteredMap;
    private readonly Texture r_brdfLutHandle;

    public Skybox(Texture hdrTexture)
    {
        r_environmentMap = Cubemap.Create(hdrTexture, Cubemap.CubemapType.Skybox, new Vector2i(512, 512));
        r_irradianceMap = Cubemap.Create(r_environmentMap, Cubemap.CubemapType.Irradiance, new Vector2i(32, 32));
        r_prefilteredMap = Cubemap.Create(r_environmentMap, Cubemap.CubemapType.Prefiltered, new Vector2i(128, 128));
        TextureConfig brdfLutConfig = TextureConfig.DefaultHdr;
        brdfLutConfig.GenerateMipMaps = false;
        r_brdfLutHandle = Texture.CreateFromCapture(Shader.Create("Shader/Brdf.vert", "Shader/Brdf.frag", false), new Vector2i(512, 512), brdfLutConfig);
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
