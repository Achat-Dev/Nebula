namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;

    internal DirectionalLight()
    {
        SetDirection(new Vector3(50f, -30f, 0f));
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
