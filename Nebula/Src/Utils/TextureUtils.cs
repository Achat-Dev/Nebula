using Nebula.Rendering;
using Silk.NET.OpenGL;

using GL = Nebula.Rendering.GL;

namespace Nebula.Utils;

public static class TextureUtils
{
    public static void SetBaseTextureParameters(TextureTarget textureTarget, TextureConfigBase config)
    {
        int wrapMode = (int)config.WrapMode;
        GL.Get().TexParameter(textureTarget, TextureParameterName.TextureWrapS, wrapMode);
        GL.Get().TexParameter(textureTarget, TextureParameterName.TextureWrapT, wrapMode);

        GL.Get().TexParameter(textureTarget, TextureParameterName.TextureMinFilter, (int)config.MinFilterMode);
        GL.Get().TexParameter(textureTarget, TextureParameterName.TextureMagFilter, (int)config.MaxFilterMode);

        GL.Get().TexParameter(textureTarget, TextureParameterName.TextureBorderColor, [1f, 1f, 1f, 1f]);

        if (config.GenerateMipMaps)
        {
            GL.Get().TexParameter(textureTarget, TextureParameterName.TextureBaseLevel, 0);
            GL.Get().TexParameter(textureTarget, TextureParameterName.TextureMaxLevel, config.MaxMipMapLevel);
            GL.Get().GenerateMipmap(textureTarget);
        }
    }
}
