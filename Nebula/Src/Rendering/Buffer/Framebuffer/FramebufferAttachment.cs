using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal class FramebufferAttachment : IDisposable, ITextureBindable
{
    public enum AttachmentType
    {
        Colour,
        Depth,
        DepthStencil,
    }

    public enum TextureType
    {
        Texture,
        TextureArray,
        Cubemap,
        CubemapArray,
        Renderbuffer,
    }

    private readonly uint r_handle;

    private readonly AttachmentType r_attachmentType;
    private readonly TextureType r_textureType;
    private readonly uint r_arraySize;
    private readonly InternalFormat r_internalFormat;
    private readonly PixelFormat r_pixelFormat;
    private readonly PixelType r_pixelType;

    internal FramebufferAttachment(FramebufferAttachmentConfig config, Vector2i size)
    {
        r_attachmentType = config.AttachmentType;
        r_textureType = config.TextureType;
        r_arraySize = config.ArraySize;
        r_internalFormat = config.GetInternalFormat();
        r_pixelFormat = config.GetPixelFormat();
        r_pixelType = config.GetPixelType();

        switch (r_textureType)
        {
            case TextureType.Texture:
                r_handle = GenerateTexture(config, size);
                break;
            case TextureType.TextureArray:
                r_handle = GenerateTextureArray(config, size);
                break;
            case TextureType.Cubemap:
                r_handle = GenerateCubemap(config, size);
                break;
            case TextureType.CubemapArray:
                r_handle = GenerateCubemapArray(config, size);
                break;
            case TextureType.Renderbuffer:
                r_handle = GenerateRenderbuffer(config, size);
                break;
        }
    }

    internal unsafe void Resize(Vector2i size)
    {
        switch (r_textureType)
        {
            case TextureType.Texture:
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
                GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
                GL.Get().BindTexture(TextureTarget.Texture2D, 0);
                break;
            case TextureType.TextureArray:
                GL.Get().BindTexture(TextureTarget.Texture2DArray, r_handle);
                for (uint i = 0; i < r_arraySize; i++)
                {
                    GL.Get().TexImage3D(TextureTarget.Texture2DArray, 0, r_internalFormat, (uint)size.X, (uint)size.Y, i, 0, r_pixelFormat, r_pixelType, null);
                }
                GL.Get().BindTexture(TextureTarget.Texture2DArray, 0);
                break;
            case TextureType.Cubemap:
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
                for (int i = 0; i < 6; i++)
                {
                    GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
                }
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, 0);
                break;
            case TextureType.CubemapArray:
                GL.Get().BindTexture(TextureTarget.TextureCubeMapArray, r_handle);
                uint faceCount = r_arraySize * 6;
                for (uint i = 0; i < faceCount; i++)
                {
                    GL.Get().TexImage3D(TextureTarget.TextureCubeMapArray, 0, r_internalFormat, (uint)size.X, (uint)size.Y, i, 0, r_pixelFormat, r_pixelType, null);
                }
                GL.Get().BindTexture(TextureTarget.TextureCubeMapArray, 0);
                break;
            case TextureType.Renderbuffer:
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, r_handle);
                GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_internalFormat, (uint)size.X, (uint)size.Y);
                GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                break;
        }
    }

    private unsafe uint GenerateTexture(FramebufferAttachmentConfig config, Vector2i size)
    {
        uint handle = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.Texture2D, handle);

        GL.Get().TexImage2D(TextureTarget.Texture2D, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);

        Utils.TextureUtils.SetBaseTextureParameters(TextureTarget.Texture2D, config);

        GL.Get().BindTexture(TextureTarget.Texture2D, 0);
        GL.Get().FramebufferTexture2D(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), TextureTarget.Texture2D, handle, 0);

        return handle;
    }

    private unsafe uint GenerateTextureArray(FramebufferAttachmentConfig config, Vector2i size)
    {
        uint handle = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.Texture2DArray, handle);

        for (uint i = 0; i < config.ArraySize; i++)
        {
            GL.Get().TexImage3D(TextureTarget.Texture2DArray, 0, r_internalFormat, (uint)size.X, (uint)size.Y, i, 0, r_pixelFormat, r_pixelType, null);
        }

        Utils.TextureUtils.SetBaseTextureParameters(TextureTarget.Texture2DArray, config);

        GL.Get().BindTexture(TextureTarget.Texture2DArray, 0);
        GL.Get().FramebufferTexture(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), handle, 0);

        return handle;
    }

    private unsafe uint GenerateCubemap(FramebufferAttachmentConfig config, Vector2i size)
    {
        uint handle = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.TextureCubeMap, handle);

        for (int i = 0; i < 6; i++)
        {
            GL.Get().TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, r_internalFormat, (uint)size.X, (uint)size.Y, 0, r_pixelFormat, r_pixelType, null);
        }

        Utils.TextureUtils.SetBaseTextureParameters(TextureTarget.TextureCubeMap, config);
        GL.Get().TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)config.WrapMode);

        GL.Get().BindTexture(TextureTarget.TextureCubeMap, 0);
        GL.Get().FramebufferTexture(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), handle, 0);

        return handle;
    }

    private unsafe uint GenerateCubemapArray(FramebufferAttachmentConfig config, Vector2i size)
    {
        uint handle = GL.Get().GenTexture();
        GL.Get().BindTexture(TextureTarget.TextureCubeMapArray, handle);

        uint faceCount = config.ArraySize * 6;
        for (uint i = 0; i < faceCount; i++)
        {
            GL.Get().TexImage3D(TextureTarget.TextureCubeMapArray, 0, r_internalFormat, (uint)size.X, (uint)size.Y, i, 0, r_pixelFormat, r_pixelType, null);
        }

        Utils.TextureUtils.SetBaseTextureParameters(TextureTarget.TextureCubeMapArray, config);
        GL.Get().TexParameter(TextureTarget.TextureCubeMapArray, TextureParameterName.TextureWrapR, (int)config.WrapMode);

        GL.Get().BindTexture(TextureTarget.TextureCubeMapArray, 0);
        GL.Get().FramebufferTexture(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), handle, 0);

        return handle;
    }

    private unsafe uint GenerateRenderbuffer(FramebufferAttachmentConfig config, Vector2i size)
    {
        uint handle = GL.Get().GenRenderbuffer();

        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, handle);
        GL.Get().RenderbufferStorage(RenderbufferTarget.Renderbuffer, r_internalFormat, (uint)size.X, (uint)size.Y);
        GL.Get().BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        GL.Get().FramebufferRenderbuffer(FramebufferTarget.Framebuffer, config.GetSilkAttachment(), RenderbufferTarget.Renderbuffer, handle);

        return handle;
    }

    public void Bind(Texture.Unit textureUnit)
    {
        if (r_textureType == TextureType.Renderbuffer)
        {
            Logger.EngineError("Trying to bind a writeonly framebuffer attachment");
            return;
        }

        GL.Get().ActiveTexture((TextureUnit)textureUnit);
        switch (r_textureType)
        {
            case TextureType.Texture:
                GL.Get().BindTexture(TextureTarget.Texture2D, r_handle);
                break;
            case TextureType.TextureArray:
                GL.Get().BindTexture(TextureTarget.Texture2DArray, r_handle);
                break;
            case TextureType.Cubemap:
                GL.Get().BindTexture(TextureTarget.TextureCubeMap, r_handle);
                break;
            case TextureType.CubemapArray:
                GL.Get().BindTexture(TextureTarget.TextureCubeMapArray, r_handle);
                break;
        }
    }

    public uint GetHandle()
    {
        if (r_textureType == TextureType.Renderbuffer)
        {
            Logger.EngineError("Trying to access a writeonly framebuffer attachment");
            return 0;
        }
        return r_handle;
    }

    internal AttachmentType GetAttachmentType()
    {
        return r_attachmentType;
    }

    void IDisposable.Dispose()
    {
        if (r_textureType == TextureType.Renderbuffer)
        {
            GL.Get().DeleteRenderbuffer(r_handle);
        }
        else
        {
            GL.Get().DeleteTexture(r_handle);
        }
    }
}
