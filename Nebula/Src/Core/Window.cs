using Nebula.Rendering;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Nebula;

internal class Window : IDisposable
{
    private bool m_isOpen = true;
    private readonly IWindow m_window;

    public Window(string title, Vector2i size, bool vSync)
    {
        Logger.EngineInfo("Creating window");

        WindowOptions options = WindowOptions.Default;
        options.Title = title;
        options.Size = size;
        options.VSync = vSync;

        m_window = Silk.NET.Windowing.Window.Create(options);
        m_window.Resize += OnResize;
        m_window.Load += OnLoad;
        m_window.Update += OnUpdate;
        m_window.Render += OnRender;
        m_window.Closing += OnClose;
    }

    public void Open()
    {
        Logger.EngineInfo("Opening window");
        m_window.Run();
    }

    public void Close()
    {
        if (m_isOpen)
        {
            m_window.Close();
        }
    }

    public void Dispose()
    {
        Logger.EngineInfo("Disposing window");
        m_window.Dispose();
    }

    internal Vector2i GetSize()
    {
        return m_window.Size;
    }

    private void OnResize(Silk.NET.Maths.Vector2D<int> size)
    {
        GL.Get().Viewport(size);
        Game.InvokeResizing(size);
    }

    private unsafe void OnLoad()
    {
        Nebula.GargabeCollection.Init(10f);
        Nebula.AssetLoader.Init();
        Nebula.Input.Init(m_window.CreateInput());
        Nebula.Rendering.ShaderParser.Init();
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));
        Nebula.Rendering.Assimp.Init();
        Nebula.Rendering.Renderer.Init();
        Nebula.Rendering.Lighting.Init();

        Nebula.Scene.Load();

        Game.InvokeInitialised();
    }

    private void OnUpdate(double deltaTime)
    {
        float dt = (float)deltaTime;
        Nebula.Scene.GetActive().Update(dt);
        Nebula.Input.RefreshInputStates();
        Nebula.GargabeCollection.Update(dt);
    }

    private unsafe void OnRender(double deltaTime)
    {
        Nebula.Rendering.Renderer.Render();
    }

    private void OnClose()
    {
        Logger.EngineInfo("Closing window");
        m_isOpen = false;
        Game.InvokeClosing();
        Nebula.Rendering.Lighting.Dispose();
        Nebula.Rendering.Renderer.Dispose();
        Nebula.Rendering.GL.Dispose();
        Nebula.Rendering.Assimp.Dispose();
        Nebula.Cache.Dispose();
    }
}
