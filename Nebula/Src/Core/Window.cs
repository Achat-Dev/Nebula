using Nebula.Rendering;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Nebula;

internal class Window : IDisposable
{
    private readonly IWindow m_window;

    // Temporary
    private TransformComponent m_transform = new TransformComponent();
    private Entity[] m_pointLightEntites = new Entity[3];
    private PointLightComponent[] m_pointLights = new PointLightComponent[3];
    private CameraComponent m_camera;

    private Model m_monkeyModel;
    private Model m_cubeModel;
    private Model m_testModel;

    private Material m_material = new Material();
    private Nebula.Rendering.Shader m_shader;
    private Nebula.Rendering.Shader m_lightSourceShader;

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
        Nebula.Rendering.Assimp.Init();
        Renderer.Init();

        // Temporary
        m_monkeyModel = Model.Load("Art/Models/Monkey.obj");
        m_cubeModel = Model.Load("Art/Models/Cube.obj");
        m_testModel = Model.Load("Art/Models/Test.obj");

        m_shader = ShaderLibrary.Get(DefaultShader.Phong);
        m_lightSourceShader = ShaderLibrary.Get(DefaultShader.Colour);

        Entity entity = new Entity("Camera");
        m_camera = entity.AddComponent<CameraComponent>();
        TransformComponent transform = entity.GetTransform();
        transform.Translate(new Vector3(0, 0, -5));
        for (int i = 0; i < 3; i++)
        {
            m_pointLightEntites[i] = new Entity();
            m_pointLights[i] = m_pointLightEntites[i].AddComponent<PointLightComponent>();
            m_pointLights[i].GetEntity().GetTransform().SetLocalScale(Vector3.One * 0.2f);
        }
        m_pointLights[0].SetColour(Colour.Red);
        m_pointLights[1].SetColour(Colour.Green);
        m_pointLights[2].SetColour(Colour.Blue);
    }

    private void OnUpdate(double deltaTime)
    {
        Scene.GetActive().Update((float)deltaTime);
        Input.RefreshInputStates();

        TransformComponent cameraTransform = m_camera.GetEntity().GetTransform();
        if (Input.IsKeyDown(Key.W)) cameraTransform.Translate(cameraTransform.GetForward() * (float)deltaTime);
        if (Input.IsKeyDown(Key.A)) cameraTransform.Translate(-cameraTransform.GetRight() * (float)deltaTime);
        if (Input.IsKeyDown(Key.S)) cameraTransform.Translate(-cameraTransform.GetForward() * (float)deltaTime);
        if (Input.IsKeyDown(Key.D)) cameraTransform.Translate(cameraTransform.GetRight() * (float)deltaTime);
        if (Input.IsKeyDown(Key.Space)) cameraTransform.Translate(cameraTransform.GetUp() * (float)deltaTime);
        if (Input.IsKeyDown(Key.ControlLeft)) cameraTransform.Translate(-cameraTransform.GetUp() * (float)deltaTime);
        if (Input.IsKeyDown(Key.Q)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Up, -40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.E)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Up, 40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.R)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Right, -40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.T)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Right, 40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.Z)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Forward, -40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.U)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Forward, 40 * (float)deltaTime));

        float piThird = (MathF.PI * 2f) / 3f;
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            m_pointLightEntites[i].GetTransform().SetWorldPosition(new Vector3(MathF.Sin((float)m_window.Time + piThird * i) * 4, 0, MathF.Cos((float)m_window.Time + piThird * i) * 4));
        }
    }

    private unsafe void OnRender(double deltaTime)
    {
        Renderer.Clear();
        Renderer.StartFrame(m_camera);

        m_monkeyModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-3f, 0f, 0f)), m_shader, m_material);
        m_cubeModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(0f, 0f, 0f)), m_shader, m_material);
        m_testModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(3f, 0f, 0f)), m_shader, m_material);

        // Light sources
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            m_cubeModel.Draw(m_pointLightEntites[i].GetTransform().GetWorldMatrix(), m_shader, m_material);
            //Renderer.DrawUnlitMesh(m_vao, m_lightSourceShader, m_pointLightEntites[i].GetTransform().GetWorldMatrix(), m_pointLights[i].GetColour());
        }
    }

    private void OnClose()
    {
        ShaderLibrary.Dispose();
        // Temporary
        m_monkeyModel.Dispose();
        m_cubeModel.Dispose();
        m_testModel.Dispose();
    }
}
