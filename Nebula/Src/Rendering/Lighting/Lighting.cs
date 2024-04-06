using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal static class Lighting
{
    private static Vector2i s_directionalShadowMapSize = new Vector2i(1024, 1024);
    private static Vector2i s_pointShadowMapSize = new Vector2i(128, 128);

    private static Shader s_directionalShadowMapShader;
    private static Shader s_pointShadowMapShader;

    private static Framebuffer s_directionalShadowMapFramebuffer;
    private static Framebuffer s_pointShadowMapFramebuffer;

    private static uint s_pointLightCount = 0;
    private static readonly HashSet<PointLightComponent> s_pointLights = new HashSet<PointLightComponent>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising lighting");

        FramebufferAttachmentConfig directionalDepthConfig = FramebufferAttachmentConfig.Defaults.Depth();
        directionalDepthConfig.TextureType = FramebufferAttachment.TextureType.TextureArray;
        directionalDepthConfig.WrapMode = Texture.WrapMode.ClampToBorder;
        directionalDepthConfig.ArraySize = Settings.Lighting.CascadeCount + 1;
        s_directionalShadowMapFramebuffer = new Framebuffer(s_directionalShadowMapSize, directionalDepthConfig);
        s_directionalShadowMapShader = Shader.Create("Shader/Shadows/DirectionalShadowMap.vert",
            "Shader/Shadows/DirectionalShadowMap.geom",
            "Shader/Shadows/DirectionalShadowMap.frag",
            false);

        FramebufferAttachmentConfig pointDepthConfig = FramebufferAttachmentConfig.Defaults.Depth();
        pointDepthConfig.TextureType = FramebufferAttachment.TextureType.CubemapArray;
        pointDepthConfig.ArraySize = Settings.Lighting.MaxDynamicShadowCasters + 1;
        s_pointShadowMapFramebuffer = new Framebuffer(s_pointShadowMapSize, pointDepthConfig);
        s_pointShadowMapShader = Shader.Create("Shader/Shadows/PointShadowMap.vert",
            "Shader/Shadows/PointShadowMap.geom",
            "Shader/Shadows/PointShadowMap.frag",
            false);
    }

    public static void RenderDirectionalShadows(HashSet<ModelRendererComponent> modelRenderers)
    {
        s_directionalShadowMapFramebuffer.Bind();

        GL.Get().Enable(EnableCap.DepthTest);
        GL.Get().DepthFunc(DepthFunction.Less);

        GL.Get().Enable(EnableCap.CullFace);
        GL.Get().CullFace(TriangleFace.Front);
        GL.Get().Viewport(s_directionalShadowMapSize);
        GL.Get().Clear(ClearBufferMask.DepthBufferBit);

        s_directionalShadowMapShader.Use();

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
        s_pointShadowMapFramebuffer.Bind();

        GL.Get().Viewport(s_pointShadowMapSize);
        GL.Get().Clear(ClearBufferMask.DepthBufferBit);

        s_pointShadowMapShader.Use();

        int index = 0;
        foreach (var pointLight in s_pointLights)
        {
            s_pointShadowMapShader.SetVec3("u_lightPosition", pointLight.GetEntity().GetTransform().GetWorldPosition());
            s_pointShadowMapShader.SetFloat("u_farPlane", pointLight.GetRange());
            s_pointShadowMapShader.SetInt("u_lightIndex", index);
            Matrix4x4[] viewProjectionMatrices = pointLight.GetViewProjectionMatrices();
            for (int i = 0; i < 6; i++)
            {
                s_pointShadowMapShader.SetMat4($"u_viewProjections[{i}]", viewProjectionMatrices[i]);
            }

            foreach (var modelRenderer in modelRenderers)
            {
                s_pointShadowMapShader.SetMat4("u_modelMatrix", modelRenderer.GetEntity().GetTransform().GetWorldMatrix());
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
        s_pointShadowMapFramebuffer.GetAttachment(FramebufferAttachment.AttachmentType.Depth).Bind(Texture.Unit.Texture4);
    }

    internal static float[] GetPointLightData()
    {
        float[] data = new float[s_pointLightCount * 12];

        int i = 0;
        int shadowCasterCount = 0;

        foreach (PointLightComponent pointLight in s_pointLights)
        {
            Vector3 position = pointLight.GetEntity().GetTransform().GetWorldPosition();
            data[i++] = position.X;
            data[i++] = position.Y;
            data[i++] = position.Z;
            data[i++] = pointLight.GetRange();
            Vector3 colour = (Vector3)pointLight.GetColour();
            data[i++] = colour.X;
            data[i++] = colour.Y;
            data[i++] = colour.Z;
            data[i++] = pointLight.GetIntensity() * pointLight.GetIntensity();
            bool isShadowCaster = pointLight.GetIsShadowCaster();
            if (isShadowCaster && shadowCasterCount <= Settings.Lighting.MaxDynamicShadowCasters)
            {
                data[i++] = 1f;
                shadowCasterCount++;
            }
            else
            {
                data[i++] = 0f;
            }
            data[i++] = 0f;
            data[i++] = 0f;
            data[i++] = 0f;
        }

        if (shadowCasterCount > Settings.Lighting.MaxDynamicShadowCasters)
        {
            Logger.EngineWarn("Exceeding max dynamic shadow caster count");
        }

        return data;
    }

    internal static void RegisterPointLight(PointLightComponent pointLight)
    {
        if (s_pointLightCount >= Settings.Lighting.MaxPointLights)
        {
            Logger.EngineWarn("Exceeding max point light count");
            return;
        }

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

    internal static int GetDirectionalShadowMapSize()
    {
        return s_directionalShadowMapSize.X;
    }

    public static void SetDirectionalShadowMapSize(int size)
    {
        if (size <= 0)
        {
            Logger.EngineWarn("Trying to set directional shadow map size to {0}, value must be greater than 0", size);
            return;
        }

        s_directionalShadowMapSize = new Vector2i(size, size);
        s_directionalShadowMapFramebuffer.Resize(s_directionalShadowMapSize);
    }

    public static void SetPointShadowMapSize(int size)
    {
        if (size <= 0)
        {
            Logger.EngineWarn("Trying to set point shadow map size to {0}, value must be greater than 0", size);
            return;
        }

        s_pointShadowMapSize = new Vector2i(size, size);
        s_pointShadowMapFramebuffer.Resize(s_pointShadowMapSize);
    }

    internal static uint GetPointLightCount()
    {
        return s_pointLightCount;
    }

    internal static HashSet<PointLightComponent> GetPointLights()
    {
        return s_pointLights;
    }

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing lighting");
        IDisposable disposable = s_directionalShadowMapFramebuffer;
        disposable.Dispose();
        disposable = s_pointShadowMapFramebuffer;
        disposable.Dispose();
    }
}
