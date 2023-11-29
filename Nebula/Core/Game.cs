using Nebula.Rendering;

namespace Nebula;

public sealed class Game
{
    private bool m_isRunning = false;
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
            Environment.Exit(1);
        }
        s_instance = this;
        m_title = title;

        Logger.Init(LogLevel.Info);
        Logger.EngineInfo("Creating game");
        EngineResources.Init();

        m_window = new Window(title, size, vSync);
        Scene.Load();
    }

    public void Start()
    {
        Logger.EngineInfo("Starting game");
        m_isRunning = true;
        m_window.Open();

        // Since m_window.Open is a blocking call, this code is only called after the window has been closed
        Close();
        m_window.Dispose();
        Logger.Flush();
    }

    public static void Close()
    {
        if (s_instance.m_isRunning)
        {
            Logger.EngineInfo("Closing game");
            s_instance.m_isRunning = false;
            Closing?.Invoke();
            s_instance.m_window.Close();
        }
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
