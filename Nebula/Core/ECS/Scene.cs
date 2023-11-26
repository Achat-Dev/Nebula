namespace Nebula;

public sealed class Scene
{
    private readonly List<Entity> r_entities = new List<Entity>();
    private readonly Stack<Entity> r_entityRemovalStack = new Stack<Entity>();

    private static Scene s_active;

    private Scene() { }

    internal void Update()
    {
        for (int i = r_entities.Count - 1; i >= 0; i--)
        {
            r_entities[i].Update();
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

    public static Scene Load()
    {
        if (s_active != null)
        {
            s_active.r_entities.Clear();
            s_active.r_entityRemovalStack.Clear();
        }
        Scene scene = new Scene();
        s_active = scene;
        return scene;
    }

    public static Scene GetActive()
    {
        return s_active;
    }
}
