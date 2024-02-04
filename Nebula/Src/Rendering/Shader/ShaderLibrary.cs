namespace Nebula.Rendering;

public enum DefaultShader
{
    // None is the same as the fallback shader
    None = 0,
    Fallback = 0,
    Colour,
    Gouraud,
    Phong,
}

public static class ShaderLibrary
{
    private static readonly Dictionary<DefaultShader, Shader> s_shaders = new Dictionary<DefaultShader, Shader>();

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing shader library");
        foreach (var item in s_shaders)
        {
            item.Value.Dispose();
        }
        s_shaders.Clear();
    }

    public static Shader Get(DefaultShader shaderType)
    {
        if (s_shaders.TryGetValue(shaderType, out Shader shader))
        {
            return shader;
        }

        switch (shaderType)
        {
            case DefaultShader.Colour:
                shader = new Shader(AssetLoader.LoadAsFileContent("Shader/Colour.vert"), AssetLoader.LoadAsFileContent("Shader/Colour.frag"));
                break;
            case DefaultShader.Gouraud:
                shader = new Shader(AssetLoader.LoadAsFileContent("Shader/Gouraud.vert"), AssetLoader.LoadAsFileContent("Shader/Gouraud.frag"));
                break;
            case DefaultShader.Phong:
                shader = new Shader(AssetLoader.LoadAsFileContent("Shader/Phong.vert"), AssetLoader.LoadAsFileContent("Shader/Phong.frag"));
                break;
            default:
                shader = new Shader(AssetLoader.LoadAsFileContent("Shader/Fallback.vert"), AssetLoader.LoadAsFileContent("Shader/Fallback.frag"));
                break;
        }

        s_shaders.Add(shaderType, shader);
        return shader;
    }
}
