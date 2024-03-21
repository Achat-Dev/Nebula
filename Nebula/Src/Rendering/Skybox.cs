﻿namespace Nebula.Rendering;

public class Skybox : IDisposable
{
    private readonly Cubemap r_environmentMap;
    private readonly Cubemap r_irradianceMap;
    private readonly Cubemap r_prefilteredMap;
    private readonly Texture r_brdfLutHandle;

    public Skybox(Texture hdrTexture, SkyboxConfig config)
    {
        CubemapConfig cubemapConfig = new CubemapConfig(Cubemap.CubemapType.Skybox, Texture.Format.Rgb, Texture.DataType.Float, Texture.WrapMode.ClampToEdge, Texture.FilterMode.Linear, false, 0);
        r_environmentMap = Cubemap.Create(hdrTexture, cubemapConfig, config.EnvironmentMapSize);

        cubemapConfig.CubemapType = Cubemap.CubemapType.Irradiance;
        r_irradianceMap = Cubemap.Create(r_environmentMap, cubemapConfig, config.IrradianceMapSize);

        cubemapConfig.CubemapType = Cubemap.CubemapType.Prefiltered;
        cubemapConfig.MinFilterMode = Texture.FilterMode.LinearMipmapLinear;
        r_prefilteredMap = Cubemap.Create(r_environmentMap, cubemapConfig, config.PrefilteredMapSize);

        TextureConfig brdfLutConfig = new TextureConfig(Texture.Format.Rg, Texture.DataType.Float, Texture.WrapMode.ClampToEdge, Texture.FilterMode.Linear, false, 0);
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
