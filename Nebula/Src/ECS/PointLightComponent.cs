namespace Nebula;

public class PointLightComponent : StartableComponent
{
    private float m_range = 1f;
    private float m_intensity = 1f;
    private Colour m_colour = Colour.White;

    private static uint s_pointLightCount = 0;
    private static readonly HashSet<PointLightComponent> s_pointLights = new HashSet<PointLightComponent>();

    public override void OnCreate()
    {
        if (s_pointLights.Add(this))
        {
            s_pointLightCount++;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (s_pointLights.Remove(this))
        {
            s_pointLightCount = Math.Max(s_pointLightCount - 1, 0);
        }
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

    internal static uint GetPointLightCount()
    {
        return s_pointLightCount;
    }

    internal static float[] GetPointLightData()
    {
        float[] data = new float[s_pointLightCount * 12];

        int i = 0;
        Vector3 position;
        Vector3 colour;

        foreach (PointLightComponent pointLight in s_pointLights)
        {
            data[i++] = pointLight.GetRange();
            i += 3;
            position = pointLight.GetEntity().GetTransform().GetWorldPosition();
            data[i++] = position.X;
            data[i++] = position.Y;
            data[i++] = position.Z;
            i++;
            colour = ((Vector3)pointLight.GetColour()) * pointLight.GetIntensity();
            data[i++] = colour.X;
            data[i++] = colour.Y;
            data[i++] = colour.Z;
            i++;
        }

        return data;
    }
}
