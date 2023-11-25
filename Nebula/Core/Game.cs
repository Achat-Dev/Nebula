namespace Nebula;

public class Game
{
    private string m_title;
    private Window m_window;

    private static Game s_instance;

    public Game(string title)
    {
        Logger.EngineAssert(s_instance != null, "Cannot create multiple instances of Game");
        s_instance = this;

        Logger.Init(LogLevel.Info);
        Logger.EngineInfo("Creating game");

        m_title = title;

        m_window = new Window(title, new Silk.NET.Maths.Vector2D<int>(1280, 720), true);
    }

    public void Start()
    {
        Logger.EngineInfo("Starting game");
        m_window.Open();
    }

    public static string GetPersistentPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/BackfischEngine/" + s_instance.m_title;
    }

    public static string GetProcessPath()
    {
        return Environment.ProcessPath;
    }
}
