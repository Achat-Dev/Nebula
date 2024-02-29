using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

internal class Cubemap : ICacheable, IDisposable
{
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

    private unsafe Cubemap(HDRTexture texture)
    {
        Vector2i faceSize = new Vector2i(512, 512);

        r_handle = GL.Get().GenTexture();
        Bind(Texture.Unit.Texture0);

        for (int i = 0; i < 6; i++)
        {
            GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgb16f, (uint)faceSize.X, (uint)faceSize.Y, 0, PixelFormat.Rgb, PixelType.Float, null);
        }

        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.Get().Viewport(faceSize);

        FramebufferAttachment depthAttachment = new FramebufferAttachment(faceSize, FramebufferAttachment.AttachmentType.Depth, FramebufferAttachment.ReadWriteMode.Writeonly);
        Framebuffer framebuffer = new Framebuffer(depthAttachment);
        framebuffer.Bind();

        Matrix4x4 captureProjection = Matrix4x4.CreatePerspectiveFieldOfView(90f, 1f, 0.1f, 100f);
        Matrix4x4[] captureViews =
        {
            Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(Vector3.Zero, -Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.Up, Vector3.Forward),
            Matrix4x4.CreateLookAt(Vector3.Zero, -Vector3.Up, -Vector3.Forward),
            Matrix4x4.CreateLookAt(Vector3.Zero, Vector3.Forward, -Vector3.Up),
            Matrix4x4.CreateLookAt(Vector3.Zero, -Vector3.Forward, -Vector3.Up),
        };

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
        VertexArrayObject<float> vao = new VertexArrayObject<float>(skyboxVbo, skyboxIbo, new BufferLayout(BufferElement.Vec3));

        texture.Bind(Texture.Unit.Texture0);
        Shader mappingShader = Shader.Create("Shader/EquirectangularToCubemap.vert", "Shader/EquirectangularToCubemap.frag", false);
        mappingShader.Use();
        mappingShader.SetInt(mappingShader.GetCachedUniformLocation("u_equirectangularMap"), 0);
        mappingShader.SetMat4(mappingShader.GetCachedUniformLocation("u_projection"), captureProjection);

        for (int i = 0; i < 6; i++)
        {
            mappingShader.SetMat4(mappingShader.GetCachedUniformLocation("u_view"), captureViews[i]);
            GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, r_handle, 0);
            GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            vao.Draw();
        }

        framebuffer.Unbind();
        framebuffer.Dispose();

        texture.Dispose();

        GL.Get().Viewport(Game.GetWindowSize());
    }

    public static Cubemap Create(string pathRight, string pathLeft, string pathTop, string pathBottom, string pathFront, string pathBack)
    {
        // Caching of cubemaps only uses pathRight as the key right now
        // | The assumption is that the individual images of cubemaps aren't being mixed
        // | But if they are mixed this can lead to retrieving the wrong cubemap from cache
        if (Cache.CubemapCache.GetValue(pathRight, out Cubemap cubemap))
        {
            Logger.EngineDebug($"Cubemap from path \"{pathRight}\", ... already exists, returning cached instance");
            return cubemap;
        }
        Logger.EngineDebug($"Creating cubemap from path \"{pathRight}\", ...");
        cubemap = new Cubemap(pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);
        Cache.CubemapCache.CacheData(pathRight, cubemap);
        return cubemap;
    }

    public static Cubemap Create(HDRTexture texture)
    {
        Cubemap cubemap = new Cubemap(texture);
        return cubemap;
    }

    internal void Bind(Texture.Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
    }

    public void Delete()
    {
        string key = Cache.CubemapCache.GetKey(this);

        Logger.EngineDebug($"Deleting cubemap loaded from path {key}");

        IDisposable disposable = this;
        disposable.Dispose();

        Cache.TextureCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
