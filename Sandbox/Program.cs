using Nebula;
using Nebula.Rendering;

namespace Sandbox;

public class Program
{
    public static void Main(string[] args)
    {
        Settings.Lighting.CascadeCount = 5;
        Settings.Lighting.CascadeDistances = new float[]
        {
            0.1f, 0.2f, 0.4f, 0.7f, 1f
        };

        Game game = new Game("Sandbox", new Vector2i(1280, 720), true);

        Game.Initialised += () =>
        {
            Entity entity = new Entity();
            entity.AddComponent<TestComponent>();
            entity.AddComponent<TestStartableComponent>();
            entity.AddComponent<TestUpdateableComponent>();
        };

        game.Start();
    }
}

public class TestComponent : Component
{

}

public class TestStartableComponent : StartableComponent
{
    public override void OnCreate()
    {
        Logger.Info("Creating test startable component");
    }
}

public class TestUpdateableComponent : UpdateableComponent
{
    private float m_time;
    private Vector3 m_cameraRotation;
    private int m_pointLightCount = 4;
    private Entity[] m_pointLightEntites;

    public override void OnCreate()
    {
        Logger.Info("Creating test updateble component");

        // PBR Flat
        ShaderInstance shaderInstanceFlat = new ShaderInstance(Shader.Defaults.PBRFlat);
        shaderInstanceFlat.SetInt("u_irradianceMap", 0);
        shaderInstanceFlat.SetInt("u_prefilteredMap", 1);
        shaderInstanceFlat.SetInt("u_brdfLut", 2);
        shaderInstanceFlat.SetInt("u_directionalShadowMap", 3);
        shaderInstanceFlat.SetInt("u_pointShadowMaps", 4);
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

        ShaderInstance shaderInstanceTextured = new ShaderInstance(Shader.Defaults.PBRTextured);
        shaderInstanceTextured.SetInt("u_irradianceMap", 0);
        shaderInstanceTextured.SetInt("u_prefilteredMap", 1);
        shaderInstanceTextured.SetInt("u_brdfLut", 2);
        shaderInstanceTextured.SetInt("u_directionalShadowMap", 3);
        shaderInstanceTextured.SetInt("u_pointShadowMaps", 4);
        shaderInstanceTextured.SetTexture("u_albedoMap", albedoMap, Texture.Unit.Texture5);
        shaderInstanceTextured.SetTexture("u_normalMap", normalMap, Texture.Unit.Texture6);
        shaderInstanceTextured.SetTexture("u_metallicMap", metallicMap, Texture.Unit.Texture7);
        shaderInstanceTextured.SetTexture("u_roughnessMap", roughnessMap, Texture.Unit.Texture8);

        Entity pbrTexturedEntity = new Entity("PBR textured");
        pbrTexturedEntity.GetTransform().SetWorldPosition(new Vector3(1f, 0f, 0f));
        modelRenderer = pbrTexturedEntity.AddComponent<ModelRendererComponent>();
        modelRenderer.SetModel(Model.Load("Art/Models/Sphere.obj"));
        modelRenderer.SetShaderInstance(shaderInstanceTextured);

        // Objects
        ShaderInstance shaderInstanceGround = new ShaderInstance(Shader.Defaults.PBRFlat);
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
        Scene.GetActive().GetCamera().GetTransform().Translate(new Vector3(0f, 0f, -5f));

        // Lighting
        m_pointLightEntites = new Entity[m_pointLightCount];
        PointLightComponent[] pointLights = new PointLightComponent[m_pointLightCount];
        ShaderInstance[] flatShaderInstances = new ShaderInstance[m_pointLightCount];
        for (int i = 0; i < m_pointLightCount; i++)
        {
            m_pointLightEntites[i] = new Entity();
            pointLights[i] = m_pointLightEntites[i].AddComponent<PointLightComponent>();
            pointLights[i].GetEntity().GetTransform().SetLocalScale(Vector3.One * 0.2f);
            pointLights[i].SetIntensity(1f);
            pointLights[i].SetRange(10f);
            float lightValue = Nebula.Utils.MathUtils.Lerp(0.1f, 1f, (float)i / (float)m_pointLightCount);
            Colour lightColour = new Colour(lightValue, lightValue, lightValue, 1f);
            pointLights[i].SetColour(lightColour);

            modelRenderer = m_pointLightEntites[i].AddComponent<ModelRendererComponent>();
            modelRenderer.SetModel(Model.Load("Art/Models/Cube.obj"));
            flatShaderInstances[i] = new ShaderInstance(Shader.Defaults.UnlitFlat);
            flatShaderInstances[i].SetVec3("u_colour", (Vector3)lightColour);
            modelRenderer.SetShaderInstance(flatShaderInstances[i]);
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        m_time += deltaTime;

        if (Input.WasKeyPressed(Key.Escape))
        {
            Game.Close();
        }

        TransformComponent cameraTransform = Scene.GetActive().GetCamera().GetTransform();

        if (Input.IsMouseButtonDown(MouseButton.Right))
        {
            Vector2 mouseDelta = Input.GetMouseDelta() * 10f * deltaTime;

            m_cameraRotation += new Vector3(mouseDelta.Y, mouseDelta.X, 0f);
            m_cameraRotation.X = Math.Clamp(m_cameraRotation.X, -89f, 89f);

            cameraTransform.SetLocalRotation(m_cameraRotation);
        }

        float cameraSpeed = 3f * deltaTime;
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

        float piThird = (MathF.PI * 2f) / (float)m_pointLightCount;
        for (int i = 0; i < m_pointLightEntites.Length; i++)
        {
            m_pointLightEntites[i].GetTransform().SetWorldPosition(new Vector3(MathF.Sin(m_time + piThird * i) * 4, 0, MathF.Cos(m_time + piThird * i) * 4));
        }
    }
}
