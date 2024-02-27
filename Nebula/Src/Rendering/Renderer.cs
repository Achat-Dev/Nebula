using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public static class Renderer
{
    private static Framebuffer s_framebuffer;
    private static Shader s_screenShader;
    private static RawVertexArrayObject s_screenRvao;
    private static HashSet<ModelRendererComponent> s_modelRenderers = new HashSet<ModelRendererComponent>();

    internal static void Init(Vector2i windowSize)
    {
        Logger.EngineInfo("Initialising renderer");

        FramebufferAttachment colourAttachment = new FramebufferAttachment(windowSize, FramebufferAttachment.AttachmentType.Colour, FramebufferAttachment.ReadWriteMode.Readable);
        FramebufferAttachment depthAttachment = new FramebufferAttachment(windowSize, FramebufferAttachment.AttachmentType.Depth, FramebufferAttachment.ReadWriteMode.Writeonly);
        s_framebuffer = new Framebuffer(windowSize, colourAttachment, depthAttachment);
        s_screenShader = Shader.Create("Shader/Output.vert", "Shader/Output.frag", false);

        float[] screenVertices =
        {
            // Position     // UV
            -1f,  1f,       0f, 1f,
            -1f, -1f,       0f, 0f,
             1f, -1f,       1f, 0f,

            -1f,  1f,       0f, 1f,
             1f, -1f,       1f, 0f,
             1f,  1f,       1f, 1f,
        };

        BufferObject<float> rvbo = new BufferObject<float>(screenVertices, BufferTargetARB.ArrayBuffer);
        BufferLayout bufferLayout = new BufferLayout(BufferElement.Vec2, BufferElement.Vec2);
        s_screenRvao = new RawVertexArrayObject(rvbo, bufferLayout);

        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
    }

    public static void SetClearColour(Colour colour)
    {
        GL.Get().ClearColor(colour);
    }

    internal static void Render(CameraComponent camera)
    {
        // Render to framebuffer
        s_framebuffer.Bind();
        GL.Get().Enable(GLEnum.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        UpdateUniformBuffers(camera);

        foreach (var modelRenderer in s_modelRenderers)
        {
            modelRenderer.Draw();
        }

        // Render framebuffer to screen
        s_framebuffer.Unbind();
        s_screenShader.Use();
        GL.Get().Disable(GLEnum.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit);
        GL.Get().ActiveTexture(TextureUnit.Texture0);
        GL.Get().BindTexture(TextureTarget.Texture2D, s_framebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Colour));
        s_screenRvao.Draw();
    }

    private static void UpdateUniformBuffers(CameraComponent camera)
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

    internal static void DrawMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        shaderInstance.SetMat4("u_modelMatrix", modelMatrix);
        if (shaderInstance.GetShader().UsesNormalMatrix())
        {
            if (modelMatrix.GetDeterminant() != 0f)
            {
                modelMatrix.Invert();
                modelMatrix.Transpose();
                shaderInstance.SetMat3("u_normalMatrix", (Matrix3x3)modelMatrix);
            }
            else
            {
                shaderInstance.SetMat3("u_normalMatrix", Matrix3x3.Identity);
            }
        }

        shaderInstance.GetShader().Use();
        shaderInstance.SubmitDataToShader();
        vao.Draw();
    }

    internal static void RegisterModelRenderer(ModelRendererComponent modelRenderer)
    {
        s_modelRenderers.Add(modelRenderer);
    }

    internal static void UnregisterModelRenderer(ModelRendererComponent modelRenderer)
    {
        s_modelRenderers.Remove(modelRenderer);
    }

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing renderer");
        s_framebuffer.Dispose();
        s_screenRvao.Dispose();
    }
}
