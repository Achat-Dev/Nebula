using Nebula.Rendering;

namespace Nebula;

public class ModelRendererComponent : Component
{
    private Model m_model;
    private ShaderInstance m_shaderInstance;

    public override void OnDestroy()
    {
        base.OnDestroy();
        Renderer.UnregisterModelRenderer(this);
    }

    internal void Draw()
    {
        m_model.Draw(GetEntity().GetTransform().GetWorldMatrix(), m_shaderInstance);
    }

    private void UpdateRegistration()
    {
        if (m_model != null && m_shaderInstance != null)
        {
            Renderer.RegisterModelRenderer(this);
        }
        else
        {
            Renderer.UnregisterModelRenderer(this);
        }
    }

    public Model GetModel()
    {
        return m_model;
    }

    public void SetModel(Model model)
    {
        m_model = model;
        UpdateRegistration();
    }

    public ShaderInstance GetShaderInstance()
    {
        return m_shaderInstance;
    }

    public void SetShaderInstance(ShaderInstance shaderInstance)
    {
        m_shaderInstance = shaderInstance;
        UpdateRegistration();
    }
}
