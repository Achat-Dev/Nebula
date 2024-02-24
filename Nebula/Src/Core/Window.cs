using Nebula.Rendering;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Nebula;

internal class Window : IDisposable
{
    private bool m_isOpen = true;
    private readonly IWindow m_window;

    public static event Action<Vector2i> Resizing;

    // Temporary
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

    private void OnResize(Silk.NET.Maths.Vector2D<int> size)
    {
        GL.Get().Viewport(size);
        Resizing?.Invoke(size);
    }

    private void OnLoad()
    {
        Nebula.Input.Init(m_window.CreateInput());
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));
        Nebula.Rendering.Assimp.Init();
        Nebula.Rendering.UniformBuffer.CreateDefaults();
        Nebula.Rendering.Renderer.Init();

        // Temporary
        // Model loading
        m_monkeyModel = Model.Load("Art/Models/Monkey.obj");
        m_cubeModel = Model.Load("Art/Models/Cube.obj");
        m_testModel = Model.Load("Art/Models/Test.obj");
        m_sphereModel = Model.Load("Art/Models/Sphere.obj");

        // Texture loading
        m_albedoMap = new Texture("Art/Textures/Bricks_Albedo.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_normalMap = new Texture("Art/Textures/Bricks_NormalGL.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_metallicMap = new Texture("Art/Textures/Metal_Metallic.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_roughnessMap = new Texture("Art/Textures/Bricks_Roughness.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);
        m_ambientOcclusionMap = new Texture("Art/Textures/Bricks_AmbientOcclusion.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear);

        // PBR Flat
        m_shaderInstance = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRFlat));
        m_shaderInstance.SetVec3("u_albedo", (Vector3)Colour.White);
        m_shaderInstance.SetFloat("u_metallic", 0.5f);
        m_shaderInstance.SetFloat("u_roughness", 0.5f);

        Entity pbrFlatEntity = new Entity("PBR flat");
        pbrFlatEntity.GetTransform().SetWorldPosition(new Vector3(-1f, 0f, 0f));
        ModelRendererComponent modelRenderer = pbrFlatEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(m_sphereModel);
        modelRenderer.SetShaderInstance(m_shaderInstance);

        // PBR Textured
        m_textureShaderInstance = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRTextured));
        m_textureShaderInstance.SetTexture("u_albedoMap", m_albedoMap, Texture.Unit.Texture0);
        m_textureShaderInstance.SetTexture("u_normalMap", m_normalMap, Texture.Unit.Texture1);
        //m_textureShaderInstance.SetTexture("u_metallicMap", m_metallicMap, Texture.Unit.Texture2);
        m_textureShaderInstance.SetTexture("u_roughnessMap", m_roughnessMap, Texture.Unit.Texture3);
        m_textureShaderInstance.SetInt("u_metallicMap", 2);
        //m_textureShaderInstance.SetTexture("u_ambientOcclusionMap", m_ambientOcclusionMap, Texture.Unit.Texture4);

        Entity pbrTexturedEntity = new Entity("PBR textured");
        pbrTexturedEntity.GetTransform().SetWorldPosition(new Vector3(1f, 0f, 0f));
        modelRenderer = pbrTexturedEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(m_sphereModel);
        modelRenderer.SetShaderInstance(m_textureShaderInstance);

        // Camera
        Entity cameraEntity = new Entity("Camera");
        m_camera = cameraEntity.AddComponent<CameraComponent>();
        TransformComponent transform = cameraEntity.GetTransform();
        transform.Translate(new Vector3(0, 0, -5));

        ShaderInstance[] flatShaderInstances = new ShaderInstance[3];
        // Lighting
        for (int i = 0; i < 3; i++)
        {
            m_pointLightEntites[i] = new Entity();
            m_pointLights[i] = m_pointLightEntites[i].AddComponent<PointLightComponent>();
            modelRenderer = m_pointLightEntites[i].AddComponent<ModelRendererComponent>();
            modelRenderer.SetModel(m_cubeModel);
            flatShaderInstances[i] = new ShaderInstance(Shader.Create(Shader.DefaultType.Colour));
            modelRenderer.SetShaderInstance(flatShaderInstances[i]);
            m_pointLights[i].GetEntity().GetTransform().SetLocalScale(Vector3.One * 0.2f);
        }
        m_pointLights[0].SetColour(Colour.Red);
        m_pointLights[1].SetColour(Colour.Green);
        m_pointLights[2].SetColour(Colour.Blue);
        flatShaderInstances[0].SetVec3("u_colour", (Vector3)Colour.Red);
        flatShaderInstances[1].SetVec3("u_colour", (Vector3)Colour.Green);
        flatShaderInstances[2].SetVec3("u_colour", (Vector3)Colour.Blue);

        Lighting.GetDirectionalLight().SetIntensity(1f);
    }

    private void OnUpdate(double deltaTime)
    {
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

        if (Input.IsKeyPressed(Key.AltLeft))
        {
            cameraTransform.SetLocalScale(Vector3.One);
            cameraTransform.SetWorldPosition(Vector3.Zero);
            cameraTransform.SetLocalRotation(Quaternion.Identity);
        }

        float piThird = (MathF.PI * 2f) / 3f;
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            m_pointLightEntites[i].GetTransform().SetWorldPosition(new Vector3(MathF.Sin((float)m_window.Time + piThird * i) * 4, 0, MathF.Cos((float)m_window.Time + piThird * i) * 4));
            m_pointLights[i].SetRange(m_lightRange);
            m_pointLights[i].SetIntensity(m_lightIntensity);
        }

        Scene.GetActive().Update((float)deltaTime);
        Input.RefreshInputStates();
    }

    private void OnRender(double deltaTime)
    {
        Renderer.Clear();
        Renderer.StartFrame(m_camera);
        Renderer.RenderFrame();
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
