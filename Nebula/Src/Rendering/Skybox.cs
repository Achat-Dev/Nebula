using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public class Skybox : IDisposable
{
    private readonly Cubemap r_environmentMap;
    private readonly Cubemap r_irradianceMap;
    private readonly Cubemap r_prefilteredMap;
    private readonly uint r_brdfLutHandle;

    public Skybox(Texture hdrTexture)
    {
        r_environmentMap = Cubemap.Create(hdrTexture, Cubemap.CubemapType.Skybox, new Vector2i(512, 512));
        r_irradianceMap = Cubemap.Create(r_environmentMap, Cubemap.CubemapType.Irradiance, new Vector2i(32, 32));
        r_prefilteredMap = Cubemap.Create(r_environmentMap, Cubemap.CubemapType.Prefiltered, new Vector2i(128, 128));
        r_brdfLutHandle = CreateBrdfLut(new Vector2i(512, 512));
    }

    private unsafe uint CreateBrdfLut(Vector2i size)
    {
        uint handle = GL.Get().GenTexture();
        GL.Get().ActiveTexture(TextureUnit.Texture0);
        GL.Get().BindTexture(TextureTarget.Texture2D, handle);

        GL.Get().TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.RG16f, (uint)size.X, (uint)size.Y, 0, PixelFormat.RG, PixelType.Float, null);

        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        FramebufferAttachmentConfig depthAttachmentConfig = new FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType.Depth, FramebufferAttachment.ReadWriteMode.Writeonly);
        Framebuffer framebuffer = new Framebuffer(size, depthAttachmentConfig);
        framebuffer.Bind();

        Shader mappingShader = Shader.Create("Shader/Brdf.vert", "Shader/Brdf.frag", false);
        mappingShader.Use();

        VertexArrayObject vao = Model.Load("Art/Models/Plane.obj").GetMeshes()[0].GetVao();

        GL.Get().Viewport(size);
        GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, handle, 0);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        vao.Draw();

        framebuffer.Unbind();
        IDisposable disposable = framebuffer;
        disposable.Dispose();

        GL.Get().Viewport(Game.GetWindowSize());

        return handle;
    }

    internal Cubemap GetEnvironmentMap()
    {
        return r_environmentMap;
    }

    internal Cubemap GetIrradianceMap()
    {
        return r_irradianceMap;
    }

    internal Cubemap GetPrefilteredMap()
    {
        return r_prefilteredMap;
    }

    internal uint GetBrdfLutHandle()
    {
        return r_brdfLutHandle;
    }

    public void Dispose()
    {
        GL.Get().DeleteTexture(r_brdfLutHandle);
    }
}
