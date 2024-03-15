namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;

    internal DirectionalLight()
    {
        SetDirection(new Vector3(-0.5f, -0.663f, 0.557f));
    }

    public Vector3 GetDirection()
    {
        return m_direction;
    }

    public void SetDirection(Vector3 direction)
    {
        m_direction = direction.Normalised();
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
