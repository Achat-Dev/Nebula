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
    private Vector3 m_eulerAngles;
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
        Nebula.GargabeCollection.Init(10f);
        Nebula.AssetLoader.Init();
        Nebula.Input.Init(m_window.CreateInput());
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));
        Nebula.Rendering.Assimp.Init();
        Nebula.Rendering.Renderer.Init();
        Nebula.Rendering.UniformBuffer.CreateDefaults();

        // Temporary
        // PBR Flat
        ShaderInstance shaderInstanceFlat = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRFlat));
        //ShaderInstance shaderInstanceFlat = new ShaderInstance(Shader.Create("Shader/ShadowMappingDepthMap.vert", "Shader/ShadowMappingDepthMap.frag", false));
        shaderInstanceFlat.SetInt("u_irradianceMap", 0);
        shaderInstanceFlat.SetInt("u_prefilteredMap", 1);
        shaderInstanceFlat.SetInt("u_brdfLut", 2);
        shaderInstanceFlat.SetInt("u_depthMap", 3);
        shaderInstanceFlat.SetVec3("u_albedo", (Vector3)Colour.White);
        shaderInstanceFlat.SetFloat("u_metallic", 0.1f);
        shaderInstanceFlat.SetFloat("u_roughness", 0.1f);

        Entity pbrFlatEntity = new Entity("PBR flat");
        pbrFlatEntity.GetTransform().SetWorldPosition(new Vector3(-1f, 0f, 0f));
        ModelRendererComponent modelRenderer = pbrFlatEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Sphere.obj"));
        modelRenderer.SetShaderInstance(shaderInstanceFlat);

        // PBR Textured
        TextureConfig textureConfig = TextureConfig.Defaults.Rgba();
        Texture albedoMap = Texture.Create("Art/Textures/Metal_Albedo.jpg", textureConfig);
        Texture normalMap = Texture.Create("Art/Textures/Metal_NormalGL.jpg", textureConfig);
        Texture metallicMap = Texture.Create("Art/Textures/Metal_Metallic.jpg", textureConfig);
        Texture roughnessMap = Texture.Create("Art/Textures/Metal_Roughness.jpg", textureConfig);

        ShaderInstance shaderInstanceTextured = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRTextured));
        //ShaderInstance shaderInstanceTextured = new ShaderInstance(Shader.Create("Shader/ShadowMappingDepthMap.vert", "Shader/ShadowMappingDepthMap.frag", false));
        shaderInstanceTextured.SetInt("u_irradianceMap", 0);
        shaderInstanceTextured.SetInt("u_prefilteredMap", 1);
        shaderInstanceTextured.SetInt("u_brdfLut", 2);
        shaderInstanceTextured.SetInt("u_depthMap", 3);
        shaderInstanceTextured.SetTexture("u_albedoMap", albedoMap, Texture.Unit.Texture4);
        shaderInstanceTextured.SetTexture("u_normalMap", normalMap, Texture.Unit.Texture5);
        //shaderInstanceTextured.SetInt("u_metallicMap", 6);
        shaderInstanceTextured.SetTexture("u_metallicMap", metallicMap, Texture.Unit.Texture6);
        shaderInstanceTextured.SetTexture("u_roughnessMap", roughnessMap, Texture.Unit.Texture7);

        Entity pbrTexturedEntity = new Entity("PBR textured");
        pbrTexturedEntity.GetTransform().SetWorldPosition(new Vector3(1f, 0f, 0f));
        modelRenderer = pbrTexturedEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Sphere.obj"));
        modelRenderer.SetShaderInstance(shaderInstanceTextured);

        // Objects
        ShaderInstance shaderInstanceGround = new ShaderInstance(Shader.Create(Shader.DefaultType.PBRFlat));
        //ShaderInstance shaderInstanceGround = new ShaderInstance(Shader.Create("Shader/ShadowMappingDepthMap.vert", "Shader/ShadowMappingDepthMap.frag", false));
        shaderInstanceGround.SetInt("u_irradianceMap", 0);
        shaderInstanceGround.SetInt("u_prefilteredMap", 1);
        shaderInstanceGround.SetInt("u_brdfLut", 2);
        shaderInstanceGround.SetVec3("u_albedo", (Vector3)Colour.White);
        shaderInstanceGround.SetFloat("u_metallic", 0f);
        shaderInstanceGround.SetFloat("u_roughness", 1f);

        Entity cubeEntity1 = new Entity();
        cubeEntity1.GetTransform().SetLocalPosition(new Vector3(0f, -1f, 0f));
        cubeEntity1.GetTransform().SetLocalScale(new Vector3(10f, 0.1f, 10f));
        modelRenderer = cubeEntity1.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Cube.obj"));
        modelRenderer.SetShaderInstance(shaderInstanceGround);

        Entity cubeEntity2 = new Entity();
        cubeEntity2.GetTransform().SetLocalPosition(new Vector3(0f, 1.5f, 5f));
        cubeEntity2.GetTransform().SetLocalScale(new Vector3(10f, 5f, 0.1f));
        modelRenderer = cubeEntity2.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Cube.obj"));
        modelRenderer.SetShaderInstance(shaderInstanceGround);

        Entity cubeEntity3 = new Entity();
        cubeEntity3.GetTransform().SetLocalPosition(new Vector3(1f, 1.5f, -2f));
        modelRenderer = cubeEntity3.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Cube.obj"));
        modelRenderer.SetShaderInstance(shaderInstanceGround);

        // Camera
        Entity cameraEntity = new Entity("Camera");
        m_camera = cameraEntity.AddComponent<CameraComponent>();
        cameraEntity.GetTransform().Translate(new Vector3(0, 0, -5));

        // Lighting
        PointLightComponent[] pointLights = new PointLightComponent[3];
        ShaderInstance[] flatShaderInstances = new ShaderInstance[3];
        for (int i = 0; i < 3; i++)
        {
            m_pointLightEntites[i] = new Entity();
            pointLights[i] = m_pointLightEntites[i].AddComponent<PointLightComponent>();
            pointLights[i].GetEntity().GetTransform().SetLocalScale(Vector3.One * 0.2f);
            pointLights[i].SetRange(2f);

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

        Scene.GetActive().GetDirectionalLight().SetIntensity(1f);
        Scene.GetActive().GetSkyLight().SetIntensity(1f);
    }

    private void OnUpdate(double deltaTime)
    {
        float dt = (float)deltaTime;

        TransformComponent cameraTransform = m_camera.GetEntity().GetTransform();

        if (Input.IsMouseButtonDown(MouseButton.Right))
        {
            Vector2 mouseDelta = Input.GetMouseDelta() * 10f * dt;

            m_eulerAngles += new Vector3(mouseDelta.Y, mouseDelta.X, 0f);
            m_eulerAngles.X = Math.Clamp(m_eulerAngles.X, -89f, 89f);

            cameraTransform.SetLocalRotation(m_eulerAngles);
        }

        float cameraSpeed = 3f * dt;
        if (Input.IsKeyDown(Key.W)) cameraTransform.Translate(cameraTransform.GetForward() * cameraSpeed);
        if (Input.IsKeyDown(Key.A)) cameraTransform.Translate(-cameraTransform.GetRight() * cameraSpeed);
        if (Input.IsKeyDown(Key.S)) cameraTransform.Translate(-cameraTransform.GetForward() * cameraSpeed);
        if (Input.IsKeyDown(Key.D)) cameraTransform.Translate(cameraTransform.GetRight() * cameraSpeed);

        if (Input.WasKeyPressed(Key.AltLeft))
        {
            cameraTransform.SetLocalScale(Vector3.One);
            cameraTransform.SetWorldPosition(new Vector3(0f, 0f, -5f));
            cameraTransform.SetLocalRotation(Quaternion.Identity);
        }

        float piThird = (MathF.PI * 2f) / 3f;
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            m_pointLightEntites[i].GetTransform().SetWorldPosition(new Vector3(MathF.Sin((float)m_window.Time + piThird * i) * 4, 0, MathF.Cos((float)m_window.Time + piThird * i) * 4));
        }

        Scene.GetActive().Update(dt);
        Input.RefreshInputStates();
        GargabeCollection.Update(dt);
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
