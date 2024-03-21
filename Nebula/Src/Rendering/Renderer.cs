using StbImageSharp;

using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public static class Renderer
{
    private static Shader s_skyboxShader;
    private static Shader s_screenShader;
    private static VertexArrayObject s_skyboxVao;
    private static VertexArrayObject s_screenVao;
    private static HashSet<ModelRendererComponent> s_modelRenderers = new HashSet<ModelRendererComponent>();

    private static Vector2i s_shadowMapSize = new Vector2i(1024, 1024);
    private static Framebuffer s_shadowMapFramebuffer;
    private static Shader s_shadowMapDepthShader;
    private static Matrix4x4 s_directionalLightProjectionMatrix;

    internal static void Init()
    {
        Logger.EngineInfo("Initialising renderer");

        StbImage.stbi_ldr_to_hdr_gamma(1f);
        StbImage.stbi_hdr_to_ldr_gamma(1f);

        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
        GL.Get().Enable(EnableCap.TextureCubeMapSeamless);
        GL.Get().CullFace(TriangleFace.Back);
        GL.Get().FrontFace(FrontFaceDirection.CW);

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
        Skybox skybox = new Skybox(skyboxTexture, SkyboxConfig.DefaultSmall);
        Scene.GetActive().GetSkyLight().SetSkybox(skybox);
        skyboxTexture.Delete();

        FramebufferAttachmentConfig depthConfig = FramebufferAttachmentConfig.DefaultDepth;
        depthConfig.ReadWriteMode = FramebufferAttachment.ReadWriteMode.Readable;
        s_shadowMapFramebuffer = new Framebuffer(s_shadowMapSize, depthConfig);
        s_shadowMapDepthShader = Shader.Create("Shader/DepthMap.vert", "Shader/DepthMap.frag", false);
        s_directionalLightProjectionMatrix = Matrix4x4.CreateOrthographicFieldOfView(-10f, 10f, 0.1f, 20f);
    }

    public static void SetClearColour(Colour colour)
    {
        GL.Get().ClearColor(colour);
    }

    internal static void Render(CameraComponent camera)
    {
        Scene scene = Scene.GetActive();

        UpdateUniformBuffers(camera);

        // Render shadows
        s_shadowMapFramebuffer.Bind();
        GL.Get().Enable(EnableCap.DepthTest);
        //GL.Get().DrawBuffer(DrawBufferMode.None);
        //GL.Get().ReadBuffer(ReadBufferMode.None);
        GL.Get().Viewport(s_shadowMapSize);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        DirectionalLight directionalLight = scene.GetDirectionalLight();
        Vector3 direction = directionalLight.GetDirection();
        Quaternion rotation = Quaternion.FromEulerAngles(direction);
        direction = rotation * Vector3.Forward;
        Matrix4x4 directionalLightViewMatrix = Matrix4x4.CreateLookAt(-direction, Vector3.Zero, rotation * Vector3.Up);
        Matrix4x4 directionalLightViewProjectionMatrix = directionalLightViewMatrix * s_directionalLightProjectionMatrix;

        s_shadowMapDepthShader.Use();
        s_shadowMapDepthShader.SetMat4("u_viewProjection", directionalLightViewProjectionMatrix);

        foreach (var modelRenderer in s_modelRenderers)
        {
            s_shadowMapDepthShader.SetMat4("u_modelMatrix", modelRenderer.GetEntity().GetTransform().GetWorldMatrix());
            List<Mesh> meshes = modelRenderer.GetModel().GetMeshes();
            foreach (var mesh in meshes)
            {
                mesh.GetVao().Draw();
            }
        }

        GL.Get().Viewport(Game.GetWindowSize());
        s_shadowMapFramebuffer.Unbind();
        Framebuffer framebuffer = camera.GetFramebuffer();
        framebuffer.Bind();
        //GL.Get().DrawBuffer(DrawBufferMode.ColorAttachment0);
        //GL.Get().ReadBuffer(ReadBufferMode.ColorAttachment0);

        // Render to framebuffer
        scene.GetSkyLight().SetupModelRendering();
        GL.Get().Enable(EnableCap.CullFace);
        GL.Get().Enable(EnableCap.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        foreach (var modelRenderer in s_modelRenderers)
        {
            modelRenderer.Draw();
        }

        // Render skybox
        GL.Get().Disable(EnableCap.CullFace);
        GL.Get().DepthFunc(GLEnum.Lequal);

        Matrix4x4 viewProjectionMatrix = camera.GetViewProjectionMatrix();
        viewProjectionMatrix.M41 = 0;
        viewProjectionMatrix.M42 = 0;
        viewProjectionMatrix.M43 = 0;
        viewProjectionMatrix.M44 = 0;

        scene.GetSkyLight().SetupSkyboxRendering();
        s_skyboxShader.Use();
        s_skyboxShader.SetMat4("u_viewProjection", viewProjectionMatrix);
        s_skyboxVao.Draw();

        // Render framebuffer to screen
        framebuffer.Unbind();
        s_screenShader.Use();

        GL.Get().DepthFunc(GLEnum.Less);
        GL.Get().Disable(EnableCap.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit);

        framebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Colour).Bind(Texture.Unit.Texture0);
        s_screenVao.Draw();
    }

    private static void UpdateUniformBuffers(CameraComponent camera)
    {
        UniformBuffer cameraBuffer = UniformBuffer.GetAtLocation(UniformBuffer.DefaultType.Camera);
        cameraBuffer.BufferData(0, camera.GetEntity().GetTransform().GetWorldPosition());

        UniformBuffer lightBuffer = UniformBuffer.GetAtLocation(UniformBuffer.DefaultType.Lights);
        lightBuffer.BufferData(0, Scene.GetActive().GetSkyLight().GetIntensity());
        lightBuffer.BufferData(4, PointLightComponent.GetPointLightCount());

        DirectionalLight directionalLight = Scene.GetActive().GetDirectionalLight();
        Vector4 directionalLightColour = ((Vector4)directionalLight.GetColour()) * directionalLight.GetIntensity();
        Vector4 directionalLightDirection = (Vector4)(Quaternion.FromEulerAngles(directionalLight.GetDirection()) * Vector3.Forward);
        lightBuffer.BufferData(16, directionalLightDirection, directionalLightColour);
        lightBuffer.BufferData(48, PointLightComponent.GetPointLightData());

        UniformBuffer matrixBuffer = UniformBuffer.GetAtLocation(UniformBuffer.DefaultType.Matrices);
        matrixBuffer.BufferData(0, camera.GetViewProjectionMatrix());
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
    }
}
