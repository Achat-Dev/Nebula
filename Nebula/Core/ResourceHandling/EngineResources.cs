using System.Reflection;

namespace Nebula;

internal static class EngineResources
{
    private static Assembly s_assembly;

    public static void Init()
    {
        Logger.EngineInfo("Initialising engine resources");
        s_assembly = typeof(EngineResources).Assembly;
    }

    public static string LoadAsFileContent(string path)
    {
        if (GetStream(path, out Stream stream))
        {
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        return string.Empty;
    }

    public static byte[] LoadAsByteArray(string path, out int dataSize)
    {
        if (GetStream(path, out Stream stream))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byte[] data = memoryStream.ToArray();
                dataSize = sizeof(byte) * data.Length;
                return data;
            }
        }
        dataSize = 0;
        return default(byte[]);
    }

    public static Stream LoadAsStream(string path)
    {
        GetStream(path, out Stream stream);
        return stream;
    }

    internal static bool GetStream(string path, out Stream stream)
    {
        path = path.Replace('/', '.');
        path = "Nebula.Assets." + path;
        stream = s_assembly.GetManifestResourceStream(path);
        if (stream == null)
        {
            Logger.EngineError($"Failed to load engine internal resource at path \"{path}\"");
            return false;
        }
        else
        {
            return true;
        }
    }
}
