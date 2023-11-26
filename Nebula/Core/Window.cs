using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Nebula;

internal sealed class Window : IDisposable
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
        Logger.EngineInfo("Closing window");
        Dispose();
    }

    public void Dispose()
    {
        Logger.EngineInfo("Disposing window");
        m_window.Dispose();
    }

    private void OnLoad()
    {
        Input.Init(m_window.CreateInput());
    }

    private void OnUpdate(double deltaTime)
    {
        Input.RefreshKeyboardState();
    }

    private void OnRender(double deltaTime)
    {

    }
}
