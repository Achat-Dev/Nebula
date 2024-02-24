using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public static class Renderer
{
    private static List<ModelRendererComponent> s_modelRenderers = new List<ModelRendererComponent>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising Renderer");
        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
        GL.Get().Enable(GLEnum.DepthTest);
    }

    internal static void Clear()
    {
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    internal static void RegisterModelRenderer(ModelRendererComponent modelRenderer)
    {
        s_modelRenderers.Add(modelRenderer);
    }

    internal static void RemoveModelRenderer(ModelRendererComponent modelRenderer)
    {
        s_modelRenderers.Remove(modelRenderer);
    }

    internal static void RenderFrame()
    {
        for (int i = 0; i < s_modelRenderers.Count; i++)
        {
            s_modelRenderers[i].DrawLit();
        }
    }

    internal static void StartFrame(CameraComponent camera)
    {
        UniformBuffer cameraBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Camera);
        cameraBuffer.BufferData(0, camera.GetEntity().GetTransform().GetWorldPosition());

        DirectionalLight directionalLight = Lighting.GetDirectionalLight();
        Vector4 directionalLightColour = ((Vector4)directionalLight.GetColour()) * directionalLight.GetIntensity();

        UniformBuffer lightBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Lights);
        lightBuffer.BufferData(0, Lighting.GetPointLightCount());
        lightBuffer.BufferData(16, (Vector4)directionalLight.GetDirection(), directionalLightColour);
        lightBuffer.BufferData(48, Lighting.GetPointLightData());

        UniformBuffer matrixBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Matrices);
        matrixBuffer.BufferData(0, camera.GetViewProjectionMatrix());
    }

    internal static unsafe void DrawLitMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        vao.Bind();
        Shader shader = shaderInstance.GetShader();
        shader.Use();
        shaderInstance.SubmitDataToShader();

        shader.SetMat4("u_model", modelMatrix);
        if (modelMatrix.GetDeterminant() != 0f)
        {
            modelMatrix.Invert();
            modelMatrix.Transpose();
            shader.SetMat3("u_modelNormalMatrix", (Matrix3x3)modelMatrix);
        }
        else
        {
            shader.SetMat3("u_modelNormalMatrix", Matrix3x3.Identity);
        }

        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }

    internal static unsafe void DrawUnlitMesh(VertexArrayObject vao, Shader shader, Matrix4x4 modelMatrix, Colour colour)
    {
        vao.Bind();
        shader.Use();
        shader.SetMat4("u_model", modelMatrix);
        //shader.SetVec3("u_colour", (Vector3)colour);
        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }
}
