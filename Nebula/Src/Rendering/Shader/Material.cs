namespace Nebula.Rendering;

public class Material
{
    private Colour m_colour = Colour.White;
    private float m_shininess = 32;
    private float m_metallic = 0.5f;
    private float m_roughness = 0.5f;

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

    public float GetMetallic()
    {
        return m_metallic;
    }

    public void SetMetallic(float metallic)
    {
        m_metallic = Math.Clamp(metallic, 0f, 1f);
    }

    public float GetRoughness()
    {
        return m_roughness;
    }

    public void SetRoughness(float roughness)
    {
        m_roughness = Math.Clamp(roughness, 0f, 1f);
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
