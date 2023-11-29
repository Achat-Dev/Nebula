namespace Nebula.Rendering;

public enum DefaultShader
{
    // None is the same as the fallback shader
    None = 0,
    Fallback = 0,
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
            default:
                shader = new Shader(EngineResources.LoadAsFileContent("Shader/Fallback.vert"), EngineResources.LoadAsFileContent("Shader/Fallback.frag"));
                break;
        }

        s_shaders.Add(shaderType, shader);
        return shader;
    }
}
