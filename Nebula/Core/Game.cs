namespace Nebula;

public sealed class Game
{
    private string m_title;
    private Window m_window;

    private static Game s_instance;

    public Game(string title, Vector2i size, bool vSync)
    {
        if (s_instance != null)
        {
            // Logger is already initialised here
            Logger.EngineFatal("Cannot create multiple game instances");
            Environment.Exit(1);
        }
        s_instance = this;

        Logger.Init(LogLevel.Info);
        Logger.EngineInfo("Creating game");

        m_title = title;
        m_window = new Window(title, size, vSync);
        Scene.Load();
    }

    public void Start()
    {
        Logger.EngineInfo("Starting game");
        m_window.Open();
    }

    public static string GetPersistentPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Nebula/" + s_instance.m_title;
    }

    public static string GetProcessPath()
    {
        return Environment.ProcessPath;
    }
}
