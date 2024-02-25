using Nebula.Rendering;

namespace Nebula;

public class PointLightComponent : StartableComponent
{
    private float m_range = 1f;
    private float m_intensity = 1f;
    private Colour m_colour = Colour.White;

    public override void OnCreate()
    {
        Lighting.AddPointLight(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Lighting.RemovePointLight(this);
    }

    public float GetRange()
    {
        return m_range;
    }

    public void SetRange(float range)
    {
        m_range = range;
    }

    public float GetIntensity()
    {
        return m_intensity;
    }

    public void SetIntensity(float intensity)
    {
        m_intensity = intensity;
    }

    public Colour GetColour()
    {
        return m_colour;
    }

    public void SetColour(Colour colour)
    {
        m_colour = colour;
    }
}
