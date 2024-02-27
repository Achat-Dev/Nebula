namespace Nebula.Rendering;

internal static class Assimp
{
    private static Silk.NET.Assimp.Assimp r_assimp;

    internal static void Init()
    {
        Logger.EngineInfo("Initialising Assimp");
        r_assimp = Silk.NET.Assimp.Assimp.GetApi();
    }

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing Assimp");
        r_assimp.Dispose();
    }

    public static Silk.NET.Assimp.Assimp Get()
    {
        return r_assimp;
    }
}
