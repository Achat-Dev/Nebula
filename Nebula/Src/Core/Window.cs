using Nebula.Rendering;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Nebula;

internal class Window : IDisposable
{
    private bool m_isOpen = true;
    private readonly IWindow m_window;

    // Temporary
    private TransformComponent m_transform = new TransformComponent();
    private Entity[] m_pointLightEntites = new Entity[3];
    private PointLightComponent[] m_pointLights = new PointLightComponent[3];
    private CameraComponent m_camera;

    private Model m_monkeyModel;
    private Model m_cubeModel;
    private Model m_testModel;
    private Model m_sphereModel;

    private ShaderInstance m_shaderInstance;
    private ShaderInstance m_textureShaderInstance;
    private float m_lightRange = 1f;
    private float m_lightIntensity = 1f;

    private Texture m_albedoMap;
    private Texture m_normalMap;
    private Texture m_metallicMap;
    private Texture m_roughnessMap;
    private Texture m_ambientOcclusionMap;

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

    private void OnLoad()
    {
        Input.Init(m_window.CreateInput());
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));
        Nebula.Rendering.Assimp.Init();
        Nebula.Rendering.UniformBuffer.CreateDefaults();
        Renderer.Init();

        // Temporary
        m_monkeyModel = Model.Load("Art/Models/Monkey.obj");
        m_cubeModel = Model.Load("Art/Models/Cube.obj");
        m_testModel = Model.Load("Art/Models/Test.obj");
        m_sphereModel = Model.Load("Art/Models/Sphere.obj");

        Shader shader = Shader.Create(Shader.DefaultType.PBRFlat);
        m_shaderInstance = new ShaderInstance(shader);

        shader = Shader.Create(Shader.DefaultType.PBRTextured);
        m_textureShaderInstance = new ShaderInstance(shader);

        m_albedoMap = new Texture("Art/Textures/Bricks_Albedo.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_normalMap = new Texture("Art/Textures/Bricks_NormalGL.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_metallicMap = new Texture("Art/Textures/Metal_Metallic.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_roughnessMap = new Texture("Art/Textures/Bricks_Roughness.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_ambientOcclusionMap = new Texture("Art/Textures/Bricks_AmbientOcclusion.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);

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

        Lighting.GetDirectionalLight().SetIntensity(1f);
    }

    private void OnUpdate(double deltaTime)
    {
        Scene.GetActive().Update((float)deltaTime);
        Input.RefreshInputStates();

        float dt = (float)deltaTime;

        TransformComponent cameraTransform = m_camera.GetEntity().GetTransform();
        if (Input.IsKeyDown(Key.W)) cameraTransform.Translate(cameraTransform.GetForward() * dt);
        if (Input.IsKeyDown(Key.A)) cameraTransform.Translate(-cameraTransform.GetRight() * dt);
        if (Input.IsKeyDown(Key.S)) cameraTransform.Translate(-cameraTransform.GetForward() * dt);
        if (Input.IsKeyDown(Key.D)) cameraTransform.Translate(cameraTransform.GetRight() * dt);
        if (Input.IsKeyDown(Key.Space)) cameraTransform.Translate(cameraTransform.GetUp() * dt);
        if (Input.IsKeyDown(Key.ControlLeft)) cameraTransform.Translate(-cameraTransform.GetUp() * dt);
        if (Input.IsKeyDown(Key.Q)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Up, -40 * dt));
        if (Input.IsKeyDown(Key.E)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Up, 40 * dt));
        if (Input.IsKeyDown(Key.R)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Right, -40 * dt));
        if (Input.IsKeyDown(Key.T)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Right, 40 * dt));
        if (Input.IsKeyDown(Key.Z)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Forward, -40 * dt));
        if (Input.IsKeyDown(Key.U)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Forward, 40 * dt));

        if (Input.IsKeyDown(Key.Y)) m_shaderInstance.SetMetallic(m_shaderInstance.GetMetallic() - dt);
        if (Input.IsKeyDown(Key.X)) m_shaderInstance.SetMetallic(m_shaderInstance.GetMetallic() + dt);
        if (Input.IsKeyDown(Key.C)) m_shaderInstance.SetRoughness(m_shaderInstance.GetRoughness() - dt);
        if (Input.IsKeyDown(Key.V)) m_shaderInstance.SetRoughness(m_shaderInstance.GetRoughness() + dt);
        if (Input.IsKeyDown(Key.B)) m_lightRange -= dt;
        if (Input.IsKeyDown(Key.N)) m_lightRange += dt;
        if (Input.IsKeyDown(Key.M)) m_lightIntensity -= dt;
        if (Input.IsKeyDown(Key.Comma)) m_lightIntensity += dt;

        Logger.EngineInfo($"Metallic: {m_shaderInstance.GetMetallic()} | Roughness: {m_shaderInstance.GetRoughness()} | Range: {m_lightRange} | Intensity: {m_lightIntensity}");

        float piThird = (MathF.PI * 2f) / 3f;
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            m_pointLightEntites[i].GetTransform().SetWorldPosition(new Vector3(MathF.Sin((float)m_window.Time + piThird * i) * 4, 0, MathF.Cos((float)m_window.Time + piThird * i) * 4));
            m_pointLights[i].SetRange(m_lightRange);
            m_pointLights[i].SetIntensity(m_lightIntensity);
        }
    }

    private unsafe void OnRender(double deltaTime)
    {
        Renderer.Clear();
        Renderer.StartFrame(m_camera);

        //m_monkeyModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-3f, 0f, 0f)), m_shaderInstance);
        //m_cubeModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(0f, 0f, 0f)), m_shaderInstance);
        //m_testModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(3f, 0f, 0f)), m_shaderInstance);
        m_sphereModel.Draw(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(-1f, 0f, 0f)), m_shaderInstance);
        m_sphereModel.DrawTextured(System.Numerics.Matrix4x4.CreateTranslation(new System.Numerics.Vector3(1f, 0f, 0f)), m_textureShaderInstance, m_albedoMap, m_normalMap, m_metallicMap, m_roughnessMap, m_ambientOcclusionMap);

        // Light sources
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            //m_cubeModel.Draw(m_pointLightEntites[i].GetTransform().GetWorldMatrix(), m_material);
            //Renderer.DrawUnlitMesh(m_vao, m_lightSourceShader, m_pointLightEntites[i].GetTransform().GetWorldMatrix(), m_pointLights[i].GetColour());
        }
    }

    private void OnClose()
    {
        Logger.EngineInfo("Closing window");
        m_isOpen = false;
        Game.Closing?.Invoke();

        Shader.DisposeCache();
        UniformBuffer.DisposeCache();
        // Temporary
        m_monkeyModel.Dispose();
        m_cubeModel.Dispose();
        m_testModel.Dispose();
        m_sphereModel.Dispose();
    }
}
