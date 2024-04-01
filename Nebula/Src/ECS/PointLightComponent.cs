using Nebula.Rendering;

namespace Nebula;

public class PointLightComponent : StartableComponent
{
    private float m_range = 10f;
    private float m_intensity = 1f;
    private Colour m_colour = Colour.White;

    public override void OnCreate()
    {
        Lighting.RegisterPointLight(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Lighting.UnregisterPointLight(this);
    }

    internal Matrix4x4[] GetViewProjectionMatrices()
    {
        Matrix4x4[] viewMatrices = Utils.CubemapUtils.GetViewMatrices(GetEntity().GetTransform().GetWorldPosition());
        Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspective(90f, 1f, 0.001f, m_range);

        for (int i = 0; i < viewMatrices.Length; i++)
        {
            viewMatrices[i] *= projectionMatrix;
        }
        return viewMatrices;
    }

    public float GetRange()
    {
        return m_range;
    }

    public void SetRange(float range)
    {
        m_range = MathF.Max(range, 0.0011f);
    }

    public float GetIntensity()
    {
        return m_intensity;
    }

    public void SetIntensity(float intensity)
    {
        m_intensity = MathF.Max(intensity, 0.0011f);
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
