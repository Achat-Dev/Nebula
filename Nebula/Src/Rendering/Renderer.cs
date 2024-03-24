using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

public static class Renderer
{
    private static Shader s_skyboxShader;
    private static Shader s_screenShader;
    private static VertexArrayObject s_skyboxVao;
    private static VertexArrayObject s_screenVao;
    private static HashSet<ModelRendererComponent> s_modelRenderers = new HashSet<ModelRendererComponent>();

    private static Vector2i s_shadowMapSize = new Vector2i(512, 512);
    private static Framebuffer s_directionalShadowMapFramebuffer;
    private static Shader s_directionalShadowMapShader;
    private static Framebuffer s_omnidirectionalShadowMapFramebuffer;
    private static Shader s_omnidirectionalShadowMapShader;

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

        TextureConfig skyboxConfig = TextureConfig.Defaults.Hdr();
        skyboxConfig.WrapMode = Texture.WrapMode.ClampToEdge;
        skyboxConfig.GenerateMipMaps = false;
        Texture skyboxTexture = Texture.Create("Art/Textures/Skybox_RuralRoad.hdr", skyboxConfig, true);
        Skybox skybox = new Skybox(skyboxTexture, SkyboxConfig.Defaults.Small());
        Scene.GetActive().GetSkyLight().SetSkybox(skybox);
        skyboxTexture.Delete();

        FramebufferAttachmentConfig directionalDepthConfig = FramebufferAttachmentConfig.Defaults.Depth();
        directionalDepthConfig.TextureType = FramebufferAttachment.TextureType.Texture;
        s_directionalShadowMapFramebuffer = new Framebuffer(s_shadowMapSize, directionalDepthConfig);
        s_directionalShadowMapShader = Shader.Create("Shader/DirectionalShadowMap.vert", "Shader/DirectionalShadowMap.frag", false);

        FramebufferAttachmentConfig omnidirectionalDepthConfig = FramebufferAttachmentConfig.Defaults.Depth();
        omnidirectionalDepthConfig.TextureType = FramebufferAttachment.TextureType.CubemapArray;
        omnidirectionalDepthConfig.ArraySize = 4;
        s_omnidirectionalShadowMapFramebuffer = new Framebuffer(s_shadowMapSize, omnidirectionalDepthConfig);
        s_omnidirectionalShadowMapShader = Shader.Create("Shader/OmnidirectionalShadowMap.vert", "Shader/OmnidirectionalShadowMap.geom", "Shader/OmnidirectionalShadowMap.frag", false);
    }

    public static void SetClearColour(Colour colour)
    {
        GL.Get().ClearColor(colour);
    }

    internal static void Render(CameraComponent camera)
    {
        Scene scene = Scene.GetActive();
        Matrix4x4 lightSpaceViewProjection = scene.GetDirectionalLight().GetViewProjectionMatrix();

        UpdateUniformBuffers(camera, ref lightSpaceViewProjection);

        // Render shadows
        // Directional shadows
        s_directionalShadowMapFramebuffer.Bind();

        GL.Get().Enable(EnableCap.DepthTest);
        GL.Get().DepthFunc(DepthFunction.Less);

        GL.Get().Enable(EnableCap.CullFace);
        GL.Get().CullFace(TriangleFace.Front);
        GL.Get().Viewport(s_shadowMapSize);
        GL.Get().Clear(ClearBufferMask.DepthBufferBit);

        s_directionalShadowMapShader.Use();
        s_directionalShadowMapShader.SetMat4("u_viewProjection", lightSpaceViewProjection);

        foreach (var modelRenderer in s_modelRenderers)
        {
            s_directionalShadowMapShader.SetMat4("u_modelMatrix", modelRenderer.GetEntity().GetTransform().GetWorldMatrix());
            List<Mesh> meshes = modelRenderer.GetModel().GetMeshes();
            foreach (var mesh in meshes)
            {
                mesh.GetVao().Draw();
            }
        }

        // Omnidirectional shadows
        s_omnidirectionalShadowMapFramebuffer.Bind();

        GL.Get().Clear(ClearBufferMask.DepthBufferBit);

        HashSet<PointLightComponent> pointLights = PointLightComponent.GetPointLights();
        s_omnidirectionalShadowMapShader.Use();

        int index = 0;
        foreach (var pointLight in pointLights)
        {
            s_omnidirectionalShadowMapShader.SetVec3("u_lightPosition", pointLight.GetEntity().GetTransform().GetWorldPosition());
            s_omnidirectionalShadowMapShader.SetFloat("u_farPlane", pointLight.GetRange());
            s_omnidirectionalShadowMapShader.SetInt("u_lightIndex", index);
            Matrix4x4[] viewProjectionMatrices = pointLight.GetViewProjectionMatrices();
            for (int i = 0; i < 6; i++)
            {
                s_omnidirectionalShadowMapShader.SetMat4($"u_viewProjections[{i}]", viewProjectionMatrices[i]);
            }

            foreach (var modelRenderer in s_modelRenderers)
            {
                s_omnidirectionalShadowMapShader.SetMat4("u_modelMatrix", modelRenderer.GetEntity().GetTransform().GetWorldMatrix());
                List<Mesh> meshes = modelRenderer.GetModel().GetMeshes();
                foreach (var mesh in meshes)
                {
                    mesh.GetVao().Draw();
                }
            }
            index++;
        }

        Framebuffer framebuffer = camera.GetFramebuffer();
        framebuffer.Bind();

        GL.Get().CullFace(TriangleFace.Back);
        GL.Get().Viewport(Game.GetWindowSize());
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Render to framebuffer
        scene.GetSkyLight().SetupModelRendering();
        s_directionalShadowMapFramebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Depth).Bind(Texture.Unit.Texture3);
        s_omnidirectionalShadowMapFramebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Depth).Bind(Texture.Unit.Texture4);
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

        scene.GetSkyLight().SetupSkyboxRendering();
        s_skyboxShader.Use();
        s_skyboxShader.SetMat4("u_viewProjection", viewProjectionMatrix);
        s_skyboxVao.Draw();

        // Render framebuffer to screen
        framebuffer.Unbind();
        s_screenShader.Use();

        GL.Get().Disable(EnableCap.DepthTest);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit);

        framebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Colour).Bind(Texture.Unit.Texture0);
        s_screenVao.Draw();
    }

    private static void UpdateUniformBuffers(CameraComponent camera, ref Matrix4x4 lightSpaceViewProjection)
    {
        UniformBuffer cameraBuffer = UniformBuffer.Defaults.Camera;
        cameraBuffer.BufferData(0, camera.GetEntity().GetTransform().GetWorldPosition());

        UniformBuffer lightBuffer = UniformBuffer.Defaults.Lights;
        lightBuffer.BufferData(0, Scene.GetActive().GetSkyLight().GetIntensity());
        lightBuffer.BufferData(4, PointLightComponent.GetPointLightCount());

        DirectionalLight directionalLight = Scene.GetActive().GetDirectionalLight();
        Vector4 directionalLightColour = ((Vector4)directionalLight.GetColour()) * directionalLight.GetIntensity();
        Vector4 directionalLightDirection = (Vector4)(Quaternion.FromEulerAngles(directionalLight.GetDirection()) * Vector3.Forward);
        lightBuffer.BufferData(16, directionalLightDirection, directionalLightColour);
        lightBuffer.BufferData(48, PointLightComponent.GetPointLightData());

        UniformBuffer matrixBuffer = UniformBuffer.Defaults.Matrices;
        matrixBuffer.BufferData(0, camera.GetViewProjectionMatrix());
        matrixBuffer.BufferData(64, lightSpaceViewProjection);
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
