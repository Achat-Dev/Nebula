namespace Nebula.Rendering;

public class Material
{
    private Colour m_colour = Colour.White;
    private float m_shininess = 32;

    private readonly Shader r_shader;

    private Material(Shader shader)
    {
        r_shader = shader;
    }

    public static Material Create(Shader shader)
    {
        return new Material(shader);
    }

    public Colour GetColour()
    {
        return m_colour;
    }

    public float GetShininess()
    {
        return m_shininess;
    }

    public Shader GetShader()
    {
        return r_shader;
    }
}
