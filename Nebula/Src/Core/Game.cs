namespace Nebula;

public class Game
{
    private string m_title;
    private Window m_window;

    public static Action Closing;

    private static Game s_instance;

    public Game(string title, Vector2i size, bool vSync)
    {
        if (s_instance != null)
        {
            // Logger is already initialised here
            Logger.EngineFatal("Cannot create multiple game instances");
            s_instance.Dispose();
            Environment.Exit(1);
        }
        s_instance = this;
        m_title = title;

        Logger.Init(LogLevel.Info);
        Logger.EngineInfo("Creating game");
        AssetLoader.Init();

        m_window = new Window(title, size, vSync);
        Scene.Load();
    }

    public void Start()
    {
        Logger.EngineInfo("Starting game");
        m_window.Open();

        // Since m_window.Open is a blocking call, this code is only called after the window has been closed
        Dispose();
    }

    private void Dispose()
    {
        Logger.EngineInfo("Disposing game");
        m_window.Close();
        m_window.Dispose();
        Logger.Flush();
    }

    internal static Vector2i GetWindowSize()
    {
        return s_instance.m_window.GetSize();
    }

    public static void Close()
    {
        s_instance.m_window.Close();
    }

    public static string GetPersistentPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Nebula/" + s_instance.m_title + '/';
    }

    public static string GetProcessPath()
    {
        return Path.GetDirectoryName(Environment.ProcessPath) + '/';
    }
}
