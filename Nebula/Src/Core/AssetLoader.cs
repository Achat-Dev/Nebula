namespace Nebula;

internal static class AssetLoader
{
    private static string s_assetPath;

    internal static void Init()
    {
        Logger.EngineInfo("Initialising asset loader");
        s_assetPath = Game.GetProcessPath() + "Assets/";
    }

    internal static string LoadAsFileContent(string path)
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

    internal static byte[] LoadAsByteArray(string path, out int dataSize)
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

    internal static Stream LoadAsStream(string path)
    {
        GetStream(path, out Stream stream);
        return stream;
    }

    private static bool GetStream(string path, out Stream stream)
    {
        path = s_assetPath + path;
        stream = File.OpenRead(path);
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
