using Nebula.Rendering;

namespace Nebula;

public class Scene
{
    private readonly Camera r_camera = new Camera();
    private readonly DirectionalLight r_directionalLight = new DirectionalLight();
    private readonly SkyLight r_skyLight = new SkyLight();

    private readonly List<Entity> r_entities = new List<Entity>();
    private readonly Stack<Entity> r_entityRemovalStack = new Stack<Entity>();

    private static Scene s_active;

    internal void Update(float deltaTime)
    {
        r_camera.Update();

        for (int i = r_entities.Count - 1; i >= 0; i--)
        {
            r_entities[i].Update(deltaTime);
        }

        int removalCount = r_entityRemovalStack.Count;
        for (int i = 0; i < removalCount; i++)
        {
            Entity entity = r_entityRemovalStack.Pop();
            entity.OnDestroy();
            r_entities.Remove(entity);
        }
    }

    internal void AddEntity(Entity entity)
    {
        r_entities.Add(entity);
    }

    internal void RemoveEntity(Entity entity)
    {
        r_entityRemovalStack.Push(entity);
    }

    public Camera GetCamera()
    {
        return r_camera;
    }

    public DirectionalLight GetDirectionalLight()
    {
        return r_directionalLight;
    }

    public SkyLight GetSkyLight()
    {
        return r_skyLight;
    }

    public static Scene Load()
    {
        if (s_active != null)
        {
            IDisposable disposable = s_active.r_camera;
            disposable.Dispose();
            s_active.r_entities.Clear();
            s_active.r_entities.TrimExcess();
            s_active.r_entityRemovalStack.Clear();
            s_active.r_entityRemovalStack.TrimExcess();
        }

        // Actually load scenes once scene files are implemented
        Scene scene = new Scene();
        scene.r_skyLight.SetSkybox(new Skybox("Art/Textures/Skybox_RuralRoad.hdr", SkyboxConfig.Defaults.Small(), true));

        s_active = scene;
        return scene;
    }

    public static Scene GetActive()
    {
        return s_active;
    }
}
