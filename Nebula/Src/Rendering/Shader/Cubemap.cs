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

    private unsafe Cubemap(Texture hdrTexture, Vector2i faceSize)
    {
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

        // Don't dispose this as this is the vao from the cached cube model
        // | Disposing this will make the cube model unable to render
        VertexArrayObject vao = Model.Load("Art/Models/Cube.obj", VertexFlags.Position).GetMeshes()[0].GetVao();

        hdrTexture.Bind(Texture.Unit.Texture0);
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
        IDisposable disposable = framebuffer;
        disposable.Dispose();

        hdrTexture.Delete();

        GL.Get().Viewport(Game.GetWindowSize());
    }

    private unsafe Cubemap(Cubemap cubemap, Vector2i faceSize)
    {
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

        // Don't dispose this as this is the vao from the cached cube model
        // | Disposing this will make the cube model unable to render
        VertexArrayObject vao = Model.Load("Art/Models/Cube.obj", VertexFlags.Position).GetMeshes()[0].GetVao();

        cubemap.Bind(Texture.Unit.Texture0);
        Shader mappingShader = Shader.Create("Shader/EquirectangularToCubemap.vert", "Shader/IrradianceConvolution.frag", false);
        mappingShader.Use();
        mappingShader.SetInt(mappingShader.GetCachedUniformLocation("u_environmentMap"), 0);
        mappingShader.SetMat4(mappingShader.GetCachedUniformLocation("u_projection"), captureProjection);

        for (int i = 0; i < 6; i++)
        {
            mappingShader.SetMat4(mappingShader.GetCachedUniformLocation("u_view"), captureViews[i]);
            GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, r_handle, 0);
            GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            vao.Draw();
        }

        framebuffer.Unbind();
        IDisposable disposable = framebuffer;
        disposable.Dispose();

        GL.Get().Viewport(Game.GetWindowSize());
    }

    public static Cubemap Create(string pathRight, string pathLeft, string pathTop, string pathBottom, string pathFront, string pathBack)
    {
        // Caching of cubemaps only uses pathRight as the key right now
        // | The assumption is that the individual images of cubemaps aren't being mixed
        // | But if they are mixed this can lead to retrieving the wrong cubemap from cache
        if (Cache.CubemapCache.GetValue(pathRight, out Cubemap cubemap))
        {
            Logger.EngineVerbose($"Cubemap from path \"{pathRight}\", ... already exists, returning cached instance");
            return cubemap;
        }
        Logger.EngineDebug($"Creating cubemap from path \"{pathRight}\", ...");
        cubemap = new Cubemap(pathRight, pathLeft, pathTop, pathBottom, pathFront, pathBack);
        Cache.CubemapCache.CacheData(pathRight, cubemap);
        return cubemap;
    }

    public static Cubemap Create(Texture hdrTexture, Vector2i faceSize)
    {
        uint handle = hdrTexture.GetHandle();

        if (Cache.CubemapCache.GetValue(handle, out Cubemap cubemap))
        {
            Logger.EngineVerbose($"Cubemap from texture with handle {handle} already exists, returning cached instance");
            return cubemap;
        }
        Logger.EngineDebug($"Creating cubemap from texture with handle {handle}");
        cubemap = new Cubemap(hdrTexture, faceSize);
        Cache.CubemapCache.CacheData(handle, cubemap);
        return cubemap;
    }

    public static Cubemap CreateIrradiance(Cubemap cubemap, Vector2i faceSize)
    {
        if (Cache.CubemapCache.GetValue(cubemap.r_handle, out Cubemap result))
        {
            Logger.EngineVerbose($"Irradiance cubemap from cubemap with handle {cubemap.r_handle} already exists, returning cached instance");
            return result;
        }
        Logger.EngineDebug($"Creating irradiance cubemap from cubemap with handle {cubemap.r_handle}");
        result = new Cubemap(cubemap, faceSize);
        Cache.CubemapCache.CacheData(cubemap.r_handle, cubemap);
        return result;
    }

    internal void Bind(Texture.Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
    }

    public void Delete()
    {
        object key = Cache.CubemapCache.GetKey(this);

        Logger.EngineDebug($"Deleting cubemap loaded from path / from texture with handle \"{key}\"");

        IDisposable disposable = this;
        disposable.Dispose();

        Cache.CubemapCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
