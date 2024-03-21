namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;

    private static readonly Matrix4x4 s_projectionMatrix = Matrix4x4.CreateOrthographicFieldOfView(-10f, 10f, 0.1f, 20f);

    internal DirectionalLight()
    {
        SetDirection(new Vector3(50f, -30f, 0f));
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        Quaternion rotation = Quaternion.FromEulerAngles(m_direction);
        Matrix4x4 directionalLightViewMatrix = Matrix4x4.CreateLookAt(-(rotation * Vector3.Forward), Vector3.Zero, rotation * Vector3.Up);
        return directionalLightViewMatrix * s_projectionMatrix;
    }

    public Vector3 GetDirection()
    {
        return m_direction;
    }

    public void SetDirection(Vector3 eulerAngles)
    {
        m_direction = eulerAngles;
    }

    public Colour GetColour()
    {
        return m_colour;
    }

    public void SetColour(Colour colour)
    {
        m_colour = colour;
    }

    public float GetIntensity()
    {
        return m_intensity;
    }

    public void SetIntensity(float intensity)
    {
        m_intensity = intensity;
    }
}
