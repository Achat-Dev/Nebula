using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public static class Renderer
{
    private static Cubemap s_skybox;
    private static Shader s_skyboxShader;
    private static Shader s_screenShader;
    private static VertexArrayObject<float> s_skyboxVao;
    private static RawVertexArrayObject s_screenRvao;
    private static HashSet<ModelRendererComponent> s_modelRenderers = new HashSet<ModelRendererComponent>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising renderer");

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

        BufferObject<float> screenVbo = new BufferObject<float>(screenVertices, BufferTargetARB.ArrayBuffer);
        BufferLayout bufferLayout = new BufferLayout(BufferElement.Vec2, BufferElement.Vec2);
        s_screenRvao = new RawVertexArrayObject(screenVbo, bufferLayout);

        float[] skyboxVertices =
        {
            // positions
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
        };

        uint[] skyboxIndices =
        {
            0, 1, 2,
            2, 3, 0,
            4, 1, 0,
            0, 5, 4,
            2, 6, 7,
            7, 3, 2,
            4, 5, 7,
            7, 6, 4,
            0, 3, 7,
            7, 5, 0,
            1, 4, 2,
            2, 4, 6,
        };

        BufferObject<float> skyboxVbo = new BufferObject<float>(skyboxVertices, BufferTargetARB.ArrayBuffer);
        BufferObject<uint> skyboxIbo = new BufferObject<uint>(skyboxIndices, BufferTargetARB.ElementArrayBuffer);
        s_skyboxVao = new VertexArrayObject<float>(skyboxVbo, skyboxIbo, new BufferLayout(BufferElement.Vec3));

        s_skyboxShader = Shader.Create("Shader/Skybox.vert", "Shader/Skybox.frag", false);
        s_skybox = Cubemap.Create("Art/Textures/Cubemap_Right.jpg",
            "Art/Textures/Cubemap_Left.jpg",
            "Art/Textures/Cubemap_Top.jpg",
            "Art/Textures/Cubemap_Bottom.jpg",
            "Art/Textures/Cubemap_Front.jpg",
            "Art/Textures/Cubemap_Back.jpg");

        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
    }

    public static void SetClearColour(Colour colour)
    {
        GL.Get().ClearColor(colour);
    }

    internal static void Render(CameraComponent camera)
    {
        Framebuffer framebuffer = camera.GetFramebuffer();
        framebuffer.Bind();

        UpdateUniformBuffers(camera);

        // Render to framebuffer
        GL.Get().Enable(GLEnum.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        foreach (var modelRenderer in s_modelRenderers)
        {
            modelRenderer.Draw();
        }

        // Render skybox
        GL.Get().DepthFunc(GLEnum.Lequal);

        Matrix4x4 viewProjectionMatrix = camera.GetViewProjectionMatrix();
        viewProjectionMatrix.M41 = 0;
        viewProjectionMatrix.M42 = 0;
        viewProjectionMatrix.M43 = 0;
        viewProjectionMatrix.M44 = 0;

        s_skybox.Bind(Texture.Unit.Texture0);
        s_skyboxShader.Use();
        s_skyboxShader.SetMat4(s_skyboxShader.GetCachedUniformLocation("u_viewProjection"), viewProjectionMatrix);
        s_skyboxVao.Draw();

        // Render framebuffer to screen
        framebuffer.Unbind();
        s_screenShader.Use();

        GL.Get().DepthFunc(GLEnum.Less);
        GL.Get().Disable(GLEnum.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit);

        framebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Colour).Bind(Texture.Unit.Texture0);
        s_screenRvao.Draw();
    }

    private static void UpdateUniformBuffers(CameraComponent camera)
    {
        UniformBuffer cameraBuffer = UniformBuffer.GetAtLocation(UniformBuffer.DefaultType.Camera);
        cameraBuffer.BufferData(0, camera.GetEntity().GetTransform().GetWorldPosition());

        DirectionalLight directionalLight = Lighting.GetDirectionalLight();
        Vector4 directionalLightColour = ((Vector4)directionalLight.GetColour()) * directionalLight.GetIntensity();

        UniformBuffer lightBuffer = UniformBuffer.GetAtLocation(UniformBuffer.DefaultType.Lights);
        lightBuffer.BufferData(0, Lighting.GetPointLightCount());
        lightBuffer.BufferData(16, (Vector4)directionalLight.GetDirection(), directionalLightColour);
        lightBuffer.BufferData(48, Lighting.GetPointLightData());

        UniformBuffer matrixBuffer = UniformBuffer.GetAtLocation(UniformBuffer.DefaultType.Matrices);
        matrixBuffer.BufferData(0, camera.GetViewProjectionMatrix());
    }

    internal static void DrawMesh<T>(VertexArrayObject<T> vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
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
        s_screenRvao.Dispose();
        s_skyboxVao.Dispose();
    }
}
