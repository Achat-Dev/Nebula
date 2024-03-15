namespace Nebula;

public static class AssetLoader
{
    private static string s_assetPath;

    internal static void Init()
    {
        Logger.EngineInfo("Initialising asset loader");
        s_assetPath = Game.GetProcessPath() + "Assets/";
    }

    public static string LoadAsFileContent(string path)
    {
        if (GetStream(path, out Stream stream))
        {
            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
            {
                GargabeCollection.QueueCollection();
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
                GargabeCollection.QueueCollection();
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

    private static bool GetStream(string path, out Stream stream)
    {
        path = s_assetPath + path;
        stream = File.OpenRead(path);
        if (stream == null)
        {
            Logger.EngineError("Failed to load engine internal resource at path {0}", path);
            return false;
        }
        else
        {
            return true;
        }
    }
}
