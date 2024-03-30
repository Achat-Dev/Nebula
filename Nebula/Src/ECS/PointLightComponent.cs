namespace Nebula;

public class PointLightComponent : StartableComponent
{
    private float m_range = 10f;
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

    internal static uint GetPointLightCount()
    {
        return s_pointLightCount;
    }

    internal static HashSet<PointLightComponent> GetPointLights()
    {
        return s_pointLights;
    }

    internal static float[] GetPointLightData()
    {
        float[] data = new float[s_pointLightCount * 8];

        int i = 0;
        Vector3 position;
        Vector3 colour;

        foreach (PointLightComponent pointLight in s_pointLights)
        {
            /*data[i++] = pointLight.m_range;
            data[i++] = pointLight.m_intensity * pointLight.m_intensity;
            i += 2;
            position = pointLight.GetEntity().GetTransform().GetWorldPosition();
            data[i++] = position.X;
            data[i++] = position.Y;
            data[i++] = position.Z;
            i++;
            colour = ((Vector3)pointLight.m_colour);
            data[i++] = colour.X;
            data[i++] = colour.Y;
            data[i++] = colour.Z;
            i++;*/

            position = pointLight.GetEntity().GetTransform().GetWorldPosition();
            data[i++] = position.X;
            data[i++] = position.Y;
            data[i++] = position.Z;
            data[i++] = pointLight.m_range;
            colour = ((Vector3)pointLight.m_colour);
            data[i++] = colour.X;
            data[i++] = colour.Y;
            data[i++] = colour.Z;
            data[i++] = pointLight.m_intensity * pointLight.m_intensity;
        }

        return data;
    }

    internal Matrix4x4[] GetViewProjectionMatrices()
    {
        Matrix4x4[] viewMatrices = Utils.CubemapUtils.GetViewMatrices(GetEntity().GetTransform().GetWorldPosition());
        Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(90f, 1f, 0.001f, m_range);

        for (int i = 0; i < viewMatrices.Length; i++)
        {
            viewMatrices[i] *= projectionMatrix;
        }
        return viewMatrices;
    }
}
