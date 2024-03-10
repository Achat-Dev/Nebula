using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

internal class Cubemap : ICacheable, IDisposable, ITextureBindable
{
    public enum CubemapType
    {
        Skybox,
        Irradiance,
        Prefiltered,
    }

    private readonly uint r_handle;

    private unsafe Cubemap(params string[] paths)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        ImageResult imageResult;
        for (int i = 0; i < paths.Length; i++)
        {
            imageResult = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(paths[i], out int dataSize), ColorComponents.RedGreenBlue);
            fixed (void* d = imageResult.Data)
            {
                GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgb, (uint)imageResult.Width, (uint)imageResult.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, d);
            }
        }

        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    }

    private Cubemap(ITextureBindable bindable, CubemapType cubemapType, Vector2i faceSize)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        Framebuffer framebuffer = null;
        Shader mappingShader = null;
        VertexArrayObject vao = null;
        Matrix4x4[] viewMatrices = null;

        switch (cubemapType)
        {
            case CubemapType.Skybox:
                CreateFaceTextures(faceSize, Texture.FilterMode.Linear, Texture.FilterMode.Linear);
                SetupCapturing(faceSize, "Shader/EquirectangularToCubemap.frag", out framebuffer, out mappingShader, out vao, out viewMatrices);
                break;
            case CubemapType.Irradiance:
                CreateFaceTextures(faceSize, Texture.FilterMode.Linear, Texture.FilterMode.Linear);
                SetupCapturing(faceSize, "Shader/IrradianceConvolution.frag", out framebuffer, out mappingShader, out vao, out viewMatrices);
                break;
            case CubemapType.Prefiltered:
                CreateFaceTextures(faceSize, Texture.FilterMode.LinearMipmapLinear, Texture.FilterMode.Linear);
                GL.Get().GenerateMipmap(TextureTarget.TextureCubeMap);
                SetupCapturing(faceSize, "Shader/Prefilter.frag", out framebuffer, out mappingShader, out vao, out viewMatrices);
                break;
        }

        GL.Get().Viewport(faceSize);
        bindable.Bind(Texture.Unit.Texture0);

        if (cubemapType == CubemapType.Prefiltered)
        {
            int maxMipLevels = 5;
            for (int mip = 0; mip < maxMipLevels; mip++)
            {
                int mipWidth = (int)(faceSize.X * MathF.Pow(0.5f, mip));
                int mipHeight = (int)(faceSize.Y * MathF.Pow(0.5f, mip));
                Vector2i mipSize = new Vector2i(mipWidth, mipHeight);

                framebuffer.Resize(mipSize);
                GL.Get().Viewport(mipSize);

                float roughness = (float)mip / (float)(maxMipLevels - 1);
                mappingShader.SetFloat("u_roughness", roughness);

                CaptureFaces(mappingShader, vao, mip, viewMatrices);
            }
        }
        else
        {
            CaptureFaces(mappingShader, vao, 0, viewMatrices);
        }

        CleanupCapture(framebuffer);
    }

    public static Cubemap Create(string pathRight, string pathLeft, string pathTop, string pathBottom, string pathFront, string pathBack)
    {
        int hash = HashCode.Combine(pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);

        if (Cache.CubemapCache.TryGetValue(hash, out Cubemap cubemap))
        {
            Logger.EngineVerbose($"Cubemap with hash \"{hash}\" already exists, returning cached instance");
            return cubemap;
        }

        Logger.EngineDebug($"Creating cubemap from path \"{pathRight}\", \"{pathLeft}\", \"{pathTop}\", \"{pathBottom}\", \"{pathFront}\", \"{pathBack}\"");
        cubemap = new Cubemap(pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);
        Cache.CubemapCache.CacheData(hash, cubemap);
        return cubemap;
    }

    public static Cubemap Create(ITextureBindable textureBindable, CubemapType cubemapType, Vector2i faceSize)
    {
        int hash = HashCode.Combine(textureBindable, cubemapType, faceSize);

        if (Cache.CubemapCache.TryGetValue(hash, out Cubemap cubemap))
        {
            Logger.EngineVerbose($"Cubemap with hash \"{hash}\" already exists, returning cached instance");
            return cubemap;
        }

        Logger.EngineDebug($"Creating cubemap from texture \"{textureBindable}\" with a cubemap type of {cubemapType} and a face size of {faceSize}");
        cubemap = new Cubemap(textureBindable, cubemapType, faceSize);
        Cache.CubemapCache.CacheData(hash, cubemap);
        return cubemap;
    }

    private unsafe void CreateFaceTextures(Vector2i faceSize, Texture.FilterMode minFilterMode, Texture.FilterMode maxFilterMode)
    {
        for (int i = 0; i < 6; i++)
        {
            GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgb16f, (uint)faceSize.X, (uint)faceSize.Y, 0, PixelFormat.Rgb, PixelType.Float, null);
        }

        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)minFilterMode);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)maxFilterMode);
    }

    private void SetupCapturing(Vector2i faceSize, string fragmentPath, out Framebuffer framebuffer, out Shader mappingShader, out VertexArrayObject vao, out Matrix4x4[] viewMatrices)
    {
        FramebufferAttachmentConfig depthAttachmentConfig = new FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType.Depth, FramebufferAttachment.ReadWriteMode.Writeonly);
        framebuffer = new Framebuffer(faceSize, depthAttachmentConfig);
        framebuffer.Bind();

        mappingShader = Shader.Create("Shader/EquirectangularToCubemap.vert", fragmentPath, false);
        mappingShader.Use();
        mappingShader.SetInt("u_environmentMap", 0);
        mappingShader.SetMat4("u_projection", Matrix4x4.CreatePerspectiveFieldOfView(90f, 1f, 0.1f, 100f));

        // Don't dispose this as this is the vao from the cached cube model
        // | Disposing this will make the cube model unable to render
        vao = Model.Load("Art/Models/Cube.obj", VertexFlags.Position).GetMeshes()[0].GetVao();

        viewMatrices =
        [
            Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(Vector3.Zero, -Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Forward),
            Matrix4x4.CreateLookAt(Vector3.Zero, -Vector3.Up, -Vector3.Forward),
            Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.Forward, -Vector3.Up),
            Matrix4x4.CreateLookAt(Vector3.Zero, -Vector3.Forward, -Vector3.Up),
        ];
    }

    private void CaptureFaces(Shader mappingShader, VertexArrayObject vao, int level, Matrix4x4[] viewMatrices)
    {
        for (int i = 0; i < 6; i++)
        {
            mappingShader.SetMat4("u_view", viewMatrices[i]);
            GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, r_handle, level);
            GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            vao.Draw();
        }
    }

    private void CleanupCapture(Framebuffer framebuffer)
    {
        framebuffer.Unbind();
        IDisposable disposable = framebuffer;
        disposable.Dispose();
        GL.Get().Viewport(Game.GetWindowSize());
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
            Logger.EngineDebug($"Deleting cubemap with hash \"{key}\"");
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
