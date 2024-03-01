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
    private CameraComponent m_camera;
    private Entity[] m_pointLightEntites = new Entity[3];

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
        Resizing?.Invoke(size);
    }

    private unsafe void OnLoad()
    {
        Nebula.Input.Init(m_window.CreateInput());
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));
        Nebula.Rendering.Assimp.Init();
        Nebula.Rendering.Renderer.Init();
        Nebula.Rendering.UniformBuffer.CreateDefaults();

        // Temporary
        // PBR Flat
        ShaderInstance shaderInstance = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRFlat));
        shaderInstance.SetInt("u_irradianceMap", 0);
        shaderInstance.SetVec3("u_albedo", (Vector3)Colour.White);
        shaderInstance.SetFloat("u_metallic", 0.5f);
        shaderInstance.SetFloat("u_roughness", 0.5f);

        Entity pbrFlatEntity = new Entity("PBR flat");
        pbrFlatEntity.GetTransform().SetWorldPosition(new Vector3(-1f, 0f, 0f));
        ModelRendererComponent modelRenderer = pbrFlatEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Sphere.obj"));
        modelRenderer.SetShaderInstance(shaderInstance);

        // PBR Textured
        Texture albedoMap = Texture.Create("Art/Textures/Bricks_Albedo.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear, Texture.Format.Rgba);
        Texture normalMap = Texture.Create("Art/Textures/Bricks_NormalGL.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear, Texture.Format.Rgba);
        Texture roughnessMap = Texture.Create("Art/Textures/Bricks_Roughness.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear, Texture.Format.Rgba);
        Texture ambientOcclusionMap = Texture.Create("Art/Textures/Bricks_AmbientOcclusion.jpg", Texture.WrapMode.Repeat, Texture.FilterMode.Linear, Texture.Format.Rgba);

        shaderInstance = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRTextured));
        shaderInstance.SetInt("u_irradianceMap", 0);
        shaderInstance.SetTexture("u_albedoMap", albedoMap, Texture.Unit.Texture1);
        shaderInstance.SetTexture("u_normalMap", normalMap, Texture.Unit.Texture2);
        shaderInstance.SetInt("u_metallicMap", 3);
        shaderInstance.SetTexture("u_roughnessMap", roughnessMap, Texture.Unit.Texture4);
        shaderInstance.SetTexture("u_ambientOcclusionMap", ambientOcclusionMap, Texture.Unit.Texture5);

        Entity pbrTexturedEntity = new Entity("PBR textured");
        pbrTexturedEntity.GetTransform().SetWorldPosition(new Vector3(1f, 0f, 0f));
        modelRenderer = pbrTexturedEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Sphere.obj"));
        modelRenderer.SetShaderInstance(shaderInstance);

        // Camera
        Entity cameraEntity = new Entity("Camera");
        m_camera = cameraEntity.AddComponent<CameraComponent>();
        cameraEntity.GetTransform().Translate(new Vector3(0, 0, -5));

        PointLightComponent[] pointLights = new PointLightComponent[3];
        ShaderInstance[] flatShaderInstances = new ShaderInstance[3];
        // Lighting
        for (int i = 0; i < 3; i++)
        {
            m_pointLightEntites[i] = new Entity();
            pointLights[i] = m_pointLightEntites[i].AddComponent<PointLightComponent>();
            pointLights[i].GetEntity().GetTransform().SetLocalScale(Vector3.One * 0.2f);
            pointLights[i].SetRange(0f);

            modelRenderer = m_pointLightEntites[i].AddComponent<ModelRendererComponent>();
            modelRenderer.SetModel(Model.Load("Art/Models/Cube.obj"));
            flatShaderInstances[i] = new ShaderInstance(Shader.Create(Shader.DefaultType.UnlitFlat));
            modelRenderer.SetShaderInstance(flatShaderInstances[i]);
        }

        pointLights[0].SetColour(Colour.Red);
        pointLights[1].SetColour(Colour.Green);
        pointLights[2].SetColour(Colour.Blue);
        flatShaderInstances[0].SetVec3("u_colour", (Vector3)Colour.Red);
        flatShaderInstances[1].SetVec3("u_colour", (Vector3)Colour.Green);
        flatShaderInstances[2].SetVec3("u_colour", (Vector3)Colour.Blue);

        Lighting.GetDirectionalLight().SetIntensity(0f);
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
        }

        Scene.GetActive().Update((float)deltaTime);
        Input.RefreshInputStates();
    }

    private unsafe void OnRender(double deltaTime)
    {
        Nebula.Rendering.Renderer.Render(m_camera);
    }

    private void OnClose()
    {
        Logger.EngineInfo("Closing window");
        m_isOpen = false;
        Game.Closing?.Invoke();
        Renderer.Dispose();
        GL.Dispose();
        Assimp.Dispose();
        Cache.Dispose();
    }
}
