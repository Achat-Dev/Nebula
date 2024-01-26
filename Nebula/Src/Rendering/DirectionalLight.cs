namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction = new Vector3(-0.2f, -1f, -0.3f);

    private Colour m_colour = Colour.White;
    private float m_ambientStrength = 0.2f;
    private float m_diffuseStrength = 0.5f;
    private float m_specularStrength = 1f;

    public Vector3 GetDirection()
    {
        return m_direction;
    }

    public Vector3 GetAmbient()
    {
        return (Vector3)m_colour * m_ambientStrength;
    }

    public Vector3 GetDiffuse()
    {
        return (Vector3)m_colour * m_diffuseStrength;
    }

    public Vector3 GetSpecular()
    {
        return (Vector3)m_colour * m_specularStrength;
    }

    public void SetDirection(Vector3 direction)
    {
        m_direction = direction;
    }

    public void SetColour(Colour colour)
    {
        m_colour = colour;
    }

    public void SetAmbientStrength(float strength)
    {
        m_ambientStrength = strength;
    }

    public void SetDiffuseStrength(float strength)
    {
        m_diffuseStrength = strength;
    }

    public void SetSpecularStrength(float strength)
    {
        m_specularStrength = strength;
    }
}
