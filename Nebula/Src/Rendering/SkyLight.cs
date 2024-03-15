namespace Nebula.Rendering;

public class SkyLight
{
    private float m_intensity = 1f;
    private Skybox m_skybox;

    internal SkyLight() { }

    internal void SetupModelRendering()
    {
        if (m_skybox != null)
        {
            m_skybox.GetIrradianceMap().Bind(Texture.Unit.Texture0);
            m_skybox.GetPrefilteredMap().Bind(Texture.Unit.Texture1);
            m_skybox.GetBrdfLut().Bind(Texture.Unit.Texture2);
        }
    }

    internal void SetupSkyboxRendering()
    {
        if (m_skybox != null)
        {
            m_skybox.GetEnvironmentMap().Bind(Texture.Unit.Texture0);
        }
    }

    public float GetIntensity()
    {
        return m_intensity;
    }

    public void SetIntensity(float intensity)
    {
        m_intensity = intensity;
    }

    public Skybox GetSkybox()
    {
        return m_skybox;
    }

    public void SetSkybox(Skybox skybox)
    {
        m_skybox = skybox;
    }
}
