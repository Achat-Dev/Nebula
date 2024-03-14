using StbImageSharp;

using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public static class Renderer
{
    private static Shader s_skyboxShader;
    private static Shader s_screenShader;
    private static Skybox s_skybox;
    private static VertexArrayObject s_skyboxVao;
    private static VertexArrayObject s_screenVao;
    private static HashSet<ModelRendererComponent> s_modelRenderers = new HashSet<ModelRendererComponent>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising renderer");

        StbImage.stbi_ldr_to_hdr_gamma(1f);
        StbImage.stbi_hdr_to_ldr_gamma(1f);

        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
        GL.Get().Enable(EnableCap.TextureCubeMapSeamless);

        s_skyboxShader = Shader.Create("Shader/Skybox.vert", "Shader/Skybox.frag", false);
        s_screenShader = Shader.Create("Shader/Output.vert", "Shader/Output.frag", false);

        // These don't have to be disposed because they are the vaos of the cube and plane model
        // | The vaos are disposed automatically when the cache is disposed
        s_skyboxVao = Model.Load("Art/Models/Cube.obj", VertexFlags.Position).GetMeshes()[0].GetVao();
        s_screenVao = Model.Load("Art/Models/Plane.obj", VertexFlags.Position | VertexFlags.UV).GetMeshes()[0].GetVao();

        TextureConfig skyboxConfig = TextureConfig.DefaultHdr;
        skyboxConfig.WrapMode = Texture.WrapMode.ClampToEdge;
        skyboxConfig.GenerateMipMaps = false;
        Texture skyboxTexture = Texture.Create("Art/Textures/Skybox_RuralRoad.hdr", skyboxConfig, true);
        s_skybox = new Skybox(skyboxTexture, SkyboxConfig.DefaultSmall);
        skyboxTexture.Delete();
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

        s_skybox.GetIrradianceMap().Bind(Texture.Unit.Texture0);
        s_skybox.GetPrefilteredMap().Bind(Texture.Unit.Texture1);
        s_skybox.GetBrdfLut().Bind(Texture.Unit.Texture2);

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

        s_skybox.GetEnvironmentMap().Bind(Texture.Unit.Texture0);
        s_skyboxShader.Use();
        s_skyboxShader.SetMat4("u_viewProjection", viewProjectionMatrix);
        s_skyboxVao.Draw();

        // Render framebuffer to screen
        framebuffer.Unbind();
        s_screenShader.Use();

        GL.Get().DepthFunc(GLEnum.Less);
        GL.Get().Disable(GLEnum.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit);

        framebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Colour).Bind(Texture.Unit.Texture0);
        s_screenVao.Draw();
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

    internal static void DrawMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        shaderInstance.SetMat4("u_modelMatrix", modelMatrix);

        if (shaderInstance.GetShader().IsLit())
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
    }
}
