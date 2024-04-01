using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal static class Lighting
{
    private static Vector2i s_directionalShadowMapSize = new Vector2i(1024, 1024);
    private static Vector2i s_pointShadowMapSize = new Vector2i(128, 128);

    private static Shader s_directionalShadowMapShader;
    private static Shader s_omnidirectionalShadowMapShader;

    private static Framebuffer s_directionalShadowMapFramebuffer;
    private static Framebuffer s_omnidirectionalShadowMapFramebuffer;

    private static uint s_pointLightCount = 0;
    private static readonly HashSet<PointLightComponent> s_pointLights = new HashSet<PointLightComponent>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising lighting");

        FramebufferAttachmentConfig directionalDepthConfig = FramebufferAttachmentConfig.Defaults.Depth();
        directionalDepthConfig.TextureType = FramebufferAttachment.TextureType.Texture;
        s_directionalShadowMapFramebuffer = new Framebuffer(s_directionalShadowMapSize, directionalDepthConfig);
        s_directionalShadowMapShader = Shader.Create("Shader/Shadows/DirectionalShadowMap.vert", "Shader/Shadows/DirectionalShadowMap.frag", false);

        FramebufferAttachmentConfig omnidirectionalDepthConfig = FramebufferAttachmentConfig.Defaults.Depth();
        omnidirectionalDepthConfig.TextureType = FramebufferAttachment.TextureType.CubemapArray;
        omnidirectionalDepthConfig.ArraySize = 4;
        s_omnidirectionalShadowMapFramebuffer = new Framebuffer(s_pointShadowMapSize, omnidirectionalDepthConfig);
        s_omnidirectionalShadowMapShader = Shader.Create("Shader/Shadows/OmnidirectionalShadowMap.vert", "Shader/Shadows/OmnidirectionalShadowMap.geom", "Shader/Shadows/OmnidirectionalShadowMap.frag", false);
    }

    public static void RenderDirectionalShadows(HashSet<ModelRendererComponent> modelRenderers, ref Matrix4x4 lightSpaceViewProjection)
    {
        s_directionalShadowMapFramebuffer.Bind();

        GL.Get().Enable(EnableCap.DepthTest);
        GL.Get().DepthFunc(DepthFunction.Less);

        GL.Get().Enable(EnableCap.CullFace);
        GL.Get().CullFace(TriangleFace.Front);
        GL.Get().Viewport(s_directionalShadowMapSize);
        GL.Get().Clear(ClearBufferMask.DepthBufferBit);

        s_directionalShadowMapShader.Use();
        s_directionalShadowMapShader.SetMat4("u_viewProjection", lightSpaceViewProjection);

        foreach (var modelRenderer in modelRenderers)
        {
            s_directionalShadowMapShader.SetMat4("u_modelMatrix", modelRenderer.GetEntity().GetTransform().GetWorldMatrix());
            List<Mesh> meshes = modelRenderer.GetModel().GetMeshes();
            foreach (var mesh in meshes)
            {
                mesh.GetVao().Draw();
            }
        }
    }

    public static void RenderPointShadows(HashSet<ModelRendererComponent> modelRenderers)
    {
        s_omnidirectionalShadowMapFramebuffer.Bind();

        GL.Get().Viewport(s_pointShadowMapSize);
        GL.Get().Clear(ClearBufferMask.DepthBufferBit);

        s_omnidirectionalShadowMapShader.Use();

        int index = 0;
        foreach (var pointLight in s_pointLights)
        {
            s_omnidirectionalShadowMapShader.SetVec3("u_lightPosition", pointLight.GetEntity().GetTransform().GetWorldPosition());
            s_omnidirectionalShadowMapShader.SetFloat("u_farPlane", pointLight.GetRange());
            s_omnidirectionalShadowMapShader.SetInt("u_lightIndex", index);
            Matrix4x4[] viewProjectionMatrices = pointLight.GetViewProjectionMatrices();
            for (int i = 0; i < 6; i++)
            {
                s_omnidirectionalShadowMapShader.SetMat4($"u_viewProjections[{i}]", viewProjectionMatrices[i]);
            }

            foreach (var modelRenderer in modelRenderers)
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
    }

    public static void BindDirectionalShadowMap()
    {
        s_directionalShadowMapFramebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Depth).Bind(Texture.Unit.Texture3);
    }

    public static void BindPointShadowMap()
    {
        s_omnidirectionalShadowMapFramebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Depth).Bind(Texture.Unit.Texture4);
    }

    internal static uint GetPointLightCount()
    {
        return s_pointLightCount;
    }

    internal static HashSet<PointLightComponent> GetPointLights()
    {
        return s_pointLights;
    }

    internal static float[] GetPointLightData()
    {
        float[] data = new float[s_pointLightCount * 8];

        int i = 0;
        Vector3 position;
        Vector3 colour;

        foreach (PointLightComponent pointLight in s_pointLights)
        {
            position = pointLight.GetEntity().GetTransform().GetWorldPosition();
            data[i++] = position.X;
            data[i++] = position.Y;
            data[i++] = position.Z;
            data[i++] = pointLight.GetRange();
            colour = ((Vector3)pointLight.GetColour());
            data[i++] = colour.X;
            data[i++] = colour.Y;
            data[i++] = colour.Z;
            data[i++] = pointLight.GetIntensity() * pointLight.GetIntensity();
        }

        return data;
    }

    internal static void RegisterPointLight(PointLightComponent pointLight)
    {
        if (s_pointLights.Add(pointLight))
        {
            s_pointLightCount++;
        }
    }

    internal static void UnregisterPointLight(PointLightComponent pointLight)
    {
        if (s_pointLights.Remove(pointLight))
        {
            s_pointLightCount = Math.Max(s_pointLightCount - 1, 0);
        }
    }

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing lighting");
        IDisposable disposable = s_directionalShadowMapFramebuffer;
        disposable.Dispose();
        disposable = s_omnidirectionalShadowMapFramebuffer;
        disposable.Dispose();
    }
}
