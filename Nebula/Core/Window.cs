using Nebula.Rendering;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Nebula;

internal sealed class Window : IDisposable
{
    private readonly IWindow m_window;

    // Temporary
    private TransformComponent m_transform = new TransformComponent();

    private BufferObject<float> m_vbo;
    private BufferObject<uint> m_ibo;
    private VertexArrayObject m_vao;
    private float[] m_vertices =
    {
        // Vertex coordinates   // Normals
         0.5f,  0.5f,  0.5f,    // 0.0f,  0.0f,  1.0f,  // Back face
         0.5f, -0.5f,  0.5f,    // 0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,    // 0.0f,  0.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,    // 0.0f,  0.0f,  1.0f,

        -0.5f,  0.5f,  0.5f,    //-1.0f,  0.0f,  0.0f,  // Left face
        -0.5f, -0.5f,  0.5f,    //-1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,    //-1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,    //-1.0f,  0.0f,  0.0f,

        -0.5f,  0.5f, -0.5f,    // 0.0f,  0.0f, -1.0f,  // Front face
        -0.5f, -0.5f, -0.5f,    // 0.0f,  0.0f, -1.0f,
         0.5f, -0.5f, -0.5f,    // 0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,    // 0.0f,  0.0f, -1.0f,

         0.5f,  0.5f, -0.5f,    // 1.0f,  0.0f,  0.0f,  // Right face
         0.5f, -0.5f, -0.5f,    // 1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,    // 1.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,    // 1.0f,  0.0f,  0.0f,

         0.5f,  0.5f, -0.5f,    // 0.0f,  1.0f,  0.0f,  // Top face
         0.5f,  0.5f,  0.5f,    // 0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,    // 0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,    // 0.0f,  1.0f,  0.0f,

         0.5f, -0.5f, -0.5f,    // 0.0f, -1.0f,  0.0f,  // Bottom face
         0.5f, -0.5f,  0.5f,    // 0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,    // 0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,    // 0.0f, -1.0f,  0.0f,
    };
    private uint[] m_indices =
    {
        0, 1, 2,
        0, 2, 3,
        4, 5, 6,
        4, 6, 7,
        8, 9, 10,
        8, 10, 11,
        12, 13, 14,
        12, 14, 15,
        16, 17, 18,
        16, 18, 19,
        20, 21, 22,
        20, 22, 23,
    };

    private float[] m_vertices2 =
    {
         0.0f,  0.5f, 0.0f,
         0.4f, -0.5f, 0.0f,
        -0.4f, -0.5f, 0.0f,
    };
    private uint[] m_indices2 =
    {
        0, 1, 2,
    };

    private Nebula.Rendering.Shader m_shader;

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
        m_window.Closing += OnClose;
    }

    public void Open()
    {
        Logger.EngineInfo("Opening window");
        m_window.Run();
    }

    public void Close()
    {
        Logger.EngineInfo("Closing window");
        m_window.Close();
    }

    public void Dispose()
    {
        Logger.EngineInfo("Disposing window");
        m_window.Dispose();
    }

    private void OnLoad()
    {
        Input.Init(m_window.CreateInput());
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));

        // Temporary
        m_vbo = new BufferObject<float>(m_vertices2, BufferTargetARB.ArrayBuffer);
        m_ibo = new BufferObject<uint>(m_indices2, BufferTargetARB.ElementArrayBuffer);

        BufferLayout bufferLayout = new BufferLayout(BufferElement.Float3);
        m_vao = new VertexArrayObject(m_vbo, m_ibo, bufferLayout);

        m_shader = ShaderLibrary.Get(DefaultShader.Fallback);

        Nebula.Rendering.GL.Get().ClearColor(System.Drawing.Color.LightBlue);
    }

    private void OnUpdate(double deltaTime)
    {
        Scene.GetActive().Update();
        Input.RefreshInputStates();
    }

    private unsafe void OnRender(double deltaTime)
    {
        // Temporary
        Nebula.Rendering.GL.Get().Clear((uint)ClearBufferMask.ColorBufferBit);
        m_vao.Bind();
        m_shader.Use();
        m_shader.SetMat4("u_model", m_transform.GetWorldMatrix());
        Nebula.Rendering.GL.Get().DrawElements(PrimitiveType.Triangles, (uint)m_indices2.Length, DrawElementsType.UnsignedInt, null);
    }

    private void OnClose()
    {
        ShaderLibrary.Dispose();
        // Temporary
        m_vbo.Dispose();
        m_ibo.Dispose();
        m_vao.Dispose();
    }
}
