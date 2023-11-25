using Silk.NET.Windowing;

namespace Nebula;

internal class Window
{
    private readonly IWindow m_window;

    public Window(string title, Vector2i size, bool vSync)
    {
        Logger.EngineInfo("Creating window");

        WindowOptions options = WindowOptions.Default;
        options.Title = title;
        options.Size = size;
        options.VSync = vSync;

        m_window = Silk.NET.Windowing.Window.Create(options);
        m_window.Load += OnLoad;
        m_window.Update += OnUpdate;
        m_window.Render += OnRender;
    }

    public void Open()
    {
        Logger.EngineInfo("Opening window");
        m_window.Run();
    }

    public void Close()
    {

    }

    private void OnLoad()
    {

    }

    private void OnUpdate(double deltaTime)
    {
        Logger.EngineInfo($"Update: {deltaTime}");
    }

    private void OnRender(double deltaTime)
    {
        Logger.EngineInfo($"Render: {deltaTime}");
    }
}
