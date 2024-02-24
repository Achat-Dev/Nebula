using Nebula.Rendering;

namespace Nebula;

public class ModelRendererComponent : StartableComponent
{
    private Model m_model;
    private ShaderInstance m_shaderInstance;

    public override void OnCreate()
    {
        Renderer.RegisterModelRenderer(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Renderer.RemoveModelRenderer(this);
    }

    internal void DrawLit()
    {
        m_model.DrawLit(GetEntity().GetTransform().GetWorldMatrix(), m_shaderInstance);
    }

    public Model GetModel()
    {
        return m_model;
    }

    public void SetModel(Model model)
    {
        m_model = model;
    }

    public ShaderInstance GetShaderInstance()
    {
        return m_shaderInstance;
    }

    public void SetShaderInstance(ShaderInstance shaderInstance)
    {
        m_shaderInstance = shaderInstance;
    }
}
