namespace Nebula.Rendering;

public class Material
{
    private Colour m_colour = Colour.White;
    private float m_shininess = 32;

    public Colour GetColour()
    {
        return m_colour;
    }

    public float GetShininess()
    {
        return m_shininess;
    }
}
