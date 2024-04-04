using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using StbImageSharp;

namespace Nebula.Rendering;

public static class Renderer
{
    private static Framebuffer s_framebuffer;
    private static Shader s_skyboxShader;
    private static Shader s_screenShader;
    private static VertexArrayObject s_skyboxVao;
    private static VertexArrayObject s_screenVao;
    private static HashSet<ModelRendererComponent> s_modelRenderers = new HashSet<ModelRendererComponent>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising renderer");

        // Configure stb image
        StbImage.stbi_ldr_to_hdr_gamma(1f);
        StbImage.stbi_hdr_to_ldr_gamma(1f);

        // Configure OpenGL
        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
        GL.Get().Enable(EnableCap.TextureCubeMapSeamless);
        GL.Get().CullFace(TriangleFace.Back);
        GL.Get().FrontFace(FrontFaceDirection.CW);

        // Setup rendering stuff
        s_framebuffer = new Framebuffer(Game.GetWindowSize(), FramebufferAttachmentConfig.Defaults.Colour(), FramebufferAttachmentConfig.Defaults.DepthStencil());
        Game.Resizing += (size) => s_framebuffer.Resize(size);

        s_skyboxShader = Shader.Create("Shader/Skybox.vert", "Shader/Skybox.frag", false);
        s_screenShader = Shader.Create("Shader/Output.vert", "Shader/Output.frag", false);

        // These don't have to be disposed because they are the vaos of the cube and plane model
        // | The vaos are disposed automatically when the cache is disposed
        s_skyboxVao = Model.Load("Art/Models/Cube.obj", VertexFlags.Position).GetMeshes()[0].GetVao();
        s_screenVao = Model.Load("Art/Models/Plane.obj", VertexFlags.Position | VertexFlags.UV).GetMeshes()[0].GetVao();
    }

    public static void SetClearColour(Colour colour)
    {
        GL.Get().ClearColor(colour);
    }

    internal static void Render()
    {
        Scene scene = Scene.GetActive();
        Camera camera = scene.GetCamera();
        Matrix4x4 lightSpaceViewProjection = scene.GetDirectionalLight().GetViewProjectionMatrix();

        UpdateUniformBuffers();

        // Render shadows
        Lighting.RenderDirectionalShadows(s_modelRenderers);
        Lighting.RenderPointShadows(s_modelRenderers);

        // Render to framebuffer
        s_framebuffer.Bind();

        GL.Get().CullFace(TriangleFace.Back);
        GL.Get().Viewport(Game.GetWindowSize());
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        scene.GetSkyLight().BindPBRMaps();
        Lighting.BindDirectionalShadowMap();
        Lighting.BindPointShadowMap();
        foreach (var modelRenderer in s_modelRenderers)
        {
            modelRenderer.Draw();
        }

        // Render skybox
        GL.Get().Disable(EnableCap.CullFace);
        GL.Get().DepthFunc(DepthFunction.Lequal);

        Matrix4x4 viewProjectionMatrix = camera.GetViewProjectionMatrix();
        viewProjectionMatrix.M41 = 0;
        viewProjectionMatrix.M42 = 0;
        viewProjectionMatrix.M43 = 0;
        viewProjectionMatrix.M44 = 0;

        scene.GetSkyLight().BindSkyboxMap();
        s_skyboxShader.Use();
        s_skyboxShader.SetMat4("u_viewProjection", viewProjectionMatrix);
        s_skyboxVao.Draw();

        // Render framebuffer to screen
        s_framebuffer.Unbind();
        s_screenShader.Use();

        GL.Get().Disable(EnableCap.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit);

        s_framebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Colour).Bind(Texture.Unit.Texture0);
        s_screenVao.Draw();
    }

    private static void UpdateUniformBuffers()
    {
        Camera camera = Scene.GetActive().GetCamera();

        UniformBuffer cameraBuffer = UniformBuffer.Defaults.Camera;
        cameraBuffer.BufferData(0, camera.GetTransform().GetWorldPosition());

        UniformBuffer lightBuffer = UniformBuffer.Defaults.Lights;
        lightBuffer.BufferData(0, Scene.GetActive().GetSkyLight().GetIntensity());
        lightBuffer.BufferData(4, Lighting.GetPointLightCount());

        DirectionalLight directionalLight = Scene.GetActive().GetDirectionalLight();
        Vector4 directionalLightColour = ((Vector4)directionalLight.GetColour()) * directionalLight.GetIntensity();
        Vector4 directionalLightDirection = (Vector4)(Quaternion.FromEulerAngles(directionalLight.GetDirection()) * Vector3.Forward);
        lightBuffer.BufferData(16, directionalLightDirection, directionalLightColour);
        lightBuffer.BufferData(48, Lighting.GetPointLightData());

        UniformBuffer matrixBuffer = UniformBuffer.Defaults.Matrices;
        matrixBuffer.BufferData(0, camera.GetViewProjectionMatrix());
        matrixBuffer.BufferData(64, Scene.GetActive().GetDirectionalLight().GetViewProjectionMatrix());
    }

    internal static void DrawMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        Shader shader = shaderInstance.GetShader();
        shader.Use();
        shader.SetMat4("u_modelMatrix", modelMatrix);

        if (shader.IsLit())
        {
            if (modelMatrix.GetDeterminant() != 0f)
            {
                modelMatrix.Invert();
                modelMatrix.Transpose();
                shader.SetMat3("u_normalMatrix", (Matrix3x3)modelMatrix);
            }
            else
            {
                shader.SetMat3("u_normalMatrix", Matrix3x3.Identity);
            }
        }

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
        IDisposable disposable = s_framebuffer;
        disposable.Dispose();
    }
}
