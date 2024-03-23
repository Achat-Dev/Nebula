using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

internal class Cubemap : ICacheable, IDisposable, ITextureBindable
{
    public enum MappingType
    {
        Skybox,
        Irradiance,
        Prefiltered,
    }

    private readonly uint r_handle;

    private unsafe Cubemap(CubemapConfig config, params string[] paths)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        for (int i = 0; i < paths.Length; i++)
        {
            ImageResult imageResult = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(paths[i], out int dataSize), config.GetColorComponents());
            fixed (void* d = imageResult.Data)
            {
                GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, config.GetInternalFormat(), (uint)imageResult.Width, (uint)imageResult.Height, 0, config.GetPixelFormat(), config.GetPixelType(), d);
            }
        }

        SetTextureParameters(config);
    }

    private unsafe Cubemap(ITextureBindable bindable, CubemapConfig config, Vector2i faceSize)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        // Create cubemap
        for (int i = 0; i < 6; i++)
        {
            GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, config.GetInternalFormat(), (uint)faceSize.X, (uint)faceSize.Y, 0, config.GetPixelFormat(), config.GetPixelType(), null);
        }

        SetTextureParameters(config);

        // Setup capturing
        Framebuffer framebuffer = new Framebuffer(faceSize, FramebufferAttachmentConfig.Defaults.DepthStencil());
        framebuffer.Bind();
        // Don't dispose this as this is the vao from the cached cube model
        // | Disposing this will make the cube model unable to render
        VertexArrayObject vao = Model.Load("Art/Models/Cube.obj", VertexFlags.Position).GetMeshes()[0].GetVao();

        Matrix4x4[] viewMatrices = GetViewMatrices(Vector3.Zero);

        Shader mappingShader = null;
        switch (config.MappingType)
        {
            case MappingType.Skybox:
                mappingShader = Shader.Create("Shader/EquirectangularToCubemap.vert", "Shader/EquirectangularToCubemap.frag", false);
                break;
            case MappingType.Irradiance:
                mappingShader = Shader.Create("Shader/EquirectangularToCubemap.vert", "Shader/IrradianceConvolution.frag", false);
                break;
            case MappingType.Prefiltered:
                mappingShader = Shader.Create("Shader/EquirectangularToCubemap.vert", "Shader/Prefilter.frag", false);
                GL.Get().GenerateMipmap(TextureTarget.TextureCubeMap);
                break;
        }
        mappingShader.Use();
        mappingShader.SetInt("u_environmentMap", 0);
        mappingShader.SetMat4("u_projection", Matrix4x4.CreatePerspectiveFieldOfView(90f, 1f, 0.1f, 100f));

        // Capture faces
        GL.Get().Viewport(faceSize);
        bindable.Bind(Texture.Unit.Texture0);

        if (config.MappingType == MappingType.Prefiltered)
        {
            int maxMipLevels = 5;
            for (int mipLevel = 0; mipLevel < maxMipLevels; mipLevel++)
            {
                int mipWidth = (int)(faceSize.X * MathF.Pow(0.5f, mipLevel));
                int mipHeight = (int)(faceSize.Y * MathF.Pow(0.5f, mipLevel));
                Vector2i mipSize = new Vector2i(mipWidth, mipHeight);

                framebuffer.Resize(mipSize);
                GL.Get().Viewport(mipSize);

                float roughness = (float)mipLevel / (float)(maxMipLevels - 1);
                mappingShader.SetFloat("u_roughness", roughness);

                for (int i = 0; i < 6; i++)
                {
                    mappingShader.SetMat4("u_view", viewMatrices[i]);
                    GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, r_handle, mipLevel);
                    GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    vao.Draw();
                }
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                mappingShader.SetMat4("u_view", viewMatrices[i]);
                GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, r_handle, 0);
                GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                vao.Draw();
            }
        }

        // Cleanup capture
        framebuffer.Unbind();
        IDisposable disposable = framebuffer;
        disposable.Dispose();
        GL.Get().Viewport(Game.GetWindowSize());
    }

    public static Cubemap Create(string pathRight, string pathLeft, string pathTop, string pathBottom, string pathFront, string pathBack, CubemapConfig config)
    {
        int hash = HashCode.Combine(pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack, config);

        if (Cache.CubemapCache.TryGetValue(hash, out Cubemap cubemap))
        {
            Logger.EngineVerbose("Cubemap with hash \"{0}\" already exists, returning cached instance", hash);
            return cubemap;
        }

        Logger.EngineDebug("Creating cubemap from path {0}, {1}, {2}, {3}, {4}, {5}", pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);
        cubemap = new Cubemap(config, pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);
        Cache.CubemapCache.CacheData(hash, cubemap);
        return cubemap;
    }

    public static Cubemap Create(ITextureBindable textureBindable, CubemapConfig config, Vector2i faceSize)
    {
        int hash = HashCode.Combine(textureBindable, config, faceSize);

        if (Cache.CubemapCache.TryGetValue(hash, out Cubemap cubemap))
        {
            Logger.EngineVerbose("Cubemap with hash \"{0}\" already exists, returning cached instance", hash);
            return cubemap;
        }

        Logger.EngineBegin(LogLevel.Debug)
            .Debug("Creating cubemap from texture")
            .Verbose(" with a cubemap type of \"{0}\" and a face size of {1}", config, faceSize)
            .Write();

        cubemap = new Cubemap(textureBindable, config, faceSize);
        Cache.CubemapCache.CacheData(hash, cubemap);
        return cubemap;
    }

    private void SetTextureParameters(CubemapConfig config)
    {
        int wrapMode = (int)config.WrapMode;
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, wrapMode);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, wrapMode);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, wrapMode);

        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)config.MinFilterMode);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)config.MaxFilterMode);

        if (config.GenerateMipMaps)
        {
            GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0);
            GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, config.MaxMipMapLevel);
            GL.Get().GenerateMipmap(TextureTarget.TextureCubeMap);
        }
    }

    public static Matrix4x4[] GetViewMatrices(Vector3 position)
    {
        return new Matrix4x4[]
        {
            Matrix4x4.CreateLookAt(position, position + Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(position, position - Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(position, position + Vector3.Up, Vector3.Forward),
            Matrix4x4.CreateLookAt(position, position - Vector3.Up, -Vector3.Forward),
            Matrix4x4.CreateLookAt(position, position + Vector3.Forward, -Vector3.Up),
            Matrix4x4.CreateLookAt(position, position - Vector3.Forward, -Vector3.Up),
        };
    }

    public void Bind(Texture.Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
    }

    public void Delete()
    {
        if (Cache.CubemapCache.TryGetKey(this, out int key))
        {
            Logger.EngineDebug("Deleting cubemap with hash {0}", key);
            Cache.CubemapCache.RemoveData(key);
        }

        IDisposable disposable = this;
        disposable.Dispose();
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
