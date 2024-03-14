using Silk.NET.OpenGL;
using StbImageSharp;

namespace Nebula.Rendering;

public class Texture : ICacheable, IDisposable, ITextureBindable
{
    public enum Format
    {
        R = 6403,
        Rg = 33319,
        Rgb = 6407,
        Rgba = 6408,
        Hdr = 6407,
    }

    public enum DataType
    {
        UnsignedByte = 5121,
        Float = 5126,
    }

    public enum WrapMode
    {
        ClampToBorder = 33069,
        ClampToEdge = 33071,
        MirroredRepeat = 33648,
        Repeat = 10497,
    }

    public enum FilterMode
    {
        Linear = 9729,
        LinearMipmapLinear = 9987,
        Nearest = 9728,
    }

    public enum Unit
    {
        Texture0 = 33984,
        Texture1,
        Texture2,
        Texture3,
        Texture4,
        Texture5,
        Texture6,
        Texture7,
        Texture8,
        Texture9,
        Texture10,
        Texture11,
        Texture12,
        Texture13,
        Texture14,
        Texture15,
        Texture16,
        Texture17,
        Texture18,
        Texture19,
        Texture20,
        Texture21,
        Texture22,
        Texture23,
        Texture24,
        Texture25,
        Texture26,
        Texture27,
        Texture28,
        Texture29,
        Texture30,
        Texture31,
    }

    private readonly uint r_handle;

    private unsafe Texture(string path, TextureConfig config)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Unit.Texture0);

        if (config.DataType == DataType.Float)
        {
            ImageResultFloat imageResultFloat = ImageResultFloat.FromMemory(AssetLoader.LoadAsByteArray(path, out _), config.GetColorComponents());

            // Clamp loaded data to avoid artifacts
            for (int i = 0; i < imageResultFloat.Data.Length; i++)
            {
                imageResultFloat.Data[i] = Math.Clamp(imageResultFloat.Data[i], -100f, 100f);
            }

            fixed (void* d = imageResultFloat.Data)
            {
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, config.GetInternalFormat(), (uint)imageResultFloat.Width, (uint)imageResultFloat.Height, 0, config.GetPixelFormat(), config.GetPixelType(), d);
            }
        }
        else
        {
            ImageResult imageResult = ImageResult.FromMemory(AssetLoader.LoadAsByteArray(path, out _), config.GetColorComponents());

            fixed (void* d = imageResult.Data)
            {
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, config.GetInternalFormat(), (uint)imageResult.Width, (uint)imageResult.Height, 0, config.GetPixelFormat(), config.GetPixelType(), d);
            }
        }

        int wrapMode = (int)config.WrapMode;
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapMode);

        int filterMode = (int)config.FilterMode;
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filterMode);

        if (config.GenerateMipMaps)
        {
            GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, config.MaxMipMapLevel);
            GL.Get().GenerateMipmap(TextureTarget.Texture2D);
        }

        GargabeCollection.QueueCollection();
    }

    private unsafe Texture(Shader captureShader, Vector2i size, TextureConfig config)
    {
        r_handle = GL.Get().GenTexture();
        Bind(Unit.Texture0);

        GL.Get().TexImage2D(TextureTarget.Texture2D, 0, config.GetInternalFormat(), (uint)size.X, (uint)size.Y, 0, config.GetPixelFormat(), config.GetPixelType(), null);

        int wrapMode = (int)config.WrapMode;
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapMode);

        int filterMode = (int)config.FilterMode;
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filterMode);
        GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filterMode);

        if (config.GenerateMipMaps)
        {
            GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.Get().TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, config.MaxMipMapLevel);
            GL.Get().GenerateMipmap(TextureTarget.Texture2D);
        }

        FramebufferAttachmentConfig depthAttachmentConfig = new FramebufferAttachmentConfig(FramebufferAttachment.AttachmentType.Depth, FramebufferAttachment.ReadWriteMode.Writeonly);
        Framebuffer framebuffer = new Framebuffer(size, depthAttachmentConfig);
        framebuffer.Bind();

        captureShader.Use();

        VertexArrayObject vao = Model.Load("Art/Models/Plane.obj", VertexFlags.Position | VertexFlags.UV).GetMeshes()[0].GetVao();

        GL.Get().Viewport(size);
        GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, Silk.NET.OpenGL.FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, r_handle, 0);
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        vao.Draw();

        framebuffer.Unbind();
        IDisposable disposable = framebuffer;
        disposable.Dispose();

        GL.Get().Viewport(Game.GetWindowSize());
    }

    public static Texture Create(string path, TextureConfig config, bool flipVertical = false)
    {
        if (Cache.TextureCache.TryGetValue(path, out Texture texture))
        {
            Logger.EngineVerbose($"Texture from path \"{path}\" already exists, returning cached instance");
            return texture;
        }

        Logger.EngineDebug($"Creating texture from path \"{path}\" with {config.ToString()}");
        if (flipVertical)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
        }
        texture = new Texture(path, config);
        StbImage.stbi_set_flip_vertically_on_load(0);
        Cache.TextureCache.CacheData(path, texture);
        return texture;
    }

    public static Texture CreateFromCapture(Shader captureShader, Vector2i size, TextureConfig config)
    {
        // Don't cache captured textures because they are unique every time
        return new Texture(captureShader, size, config);
    }

    public void Bind(Unit textureUnit)
    {
        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
    }

    public void Delete()
    {
        if (Cache.TextureCache.TryGetKey(this, out string key))
        {
            Logger.EngineDebug($"Deleting texture loaded from path \"{key}\"");
            Cache.TextureCache.RemoveData(key);
        }

        IDisposable disposable = this;
        disposable.Dispose();
    }

    void IDisposable.Dispose()
    {
        GL.Get().DeleteTexture(r_handle);
    }
}
