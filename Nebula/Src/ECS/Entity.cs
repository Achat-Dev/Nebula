namespace Nebula;

public class Entity
{
    private string m_name;
    private TransformComponent m_transform;

    private readonly List<Component> r_components = new List<Component>();
    private readonly List<UpdateableComponent> r_updateableComponents = new List<UpdateableComponent>();
    private readonly Stack<Component> r_componentRemovalStack = new Stack<Component>();

    public Entity(string name = "New entity")
    {
        m_name = name;
        m_transform = new TransformComponent();
        Scene scene = Scene.GetActive();
        if (scene == null)
        {
            Logger.EngineError("Trying to create an entity without an active scene. Created entity will not be updated");
        }
        else
        {
            scene.AddEntity(this);
        }
    }

    internal void Update(float deltaTime)
    {
        for (int i = r_updateableComponents.Count - 1; i >= 0; i--)
        {
            r_updateableComponents[i].OnUpdate(deltaTime);
        }

        int removalCount = r_componentRemovalStack.Count;
        for (int i = 0; i < removalCount; i++)
        {
            Component component = r_componentRemovalStack.Pop();
            r_components.Remove(component);
            if (component is UpdateableComponent updateableComponent)
            {
                r_updateableComponents.Remove(updateableComponent);
            }
            component.OnDestroy();
        }
    }

    internal void RemoveComponent(Component component)
    {
        r_componentRemovalStack.Push(component);
    }

    internal void OnDestroy()
    {
        for (int i = 0; i < r_components.Count; i++)
        {
            r_components[i].OnDestroy();
        }
        r_components.Clear();
        r_components.TrimExcess();
        r_updateableComponents.Clear();
        r_updateableComponents.TrimExcess();
        r_componentRemovalStack.Clear();
    }

    public void Destroy()
    {
        Scene.GetActive().RemoveEntity(this);
    }

    public T AddComponent<T>() where T : Component
    {
        Component component = (Component)Activator.CreateInstance(typeof(T));
        component.SetEntity(this);
        r_components.Add(component);

        if (component is UpdateableComponent updateableComponent)
        {
            updateableComponent.OnCreate();
            r_updateableComponents.Add(updateableComponent);
        }
        else if (component is StartableComponent startableComponent)
        {
            startableComponent.OnCreate();
        }

        return (T)component;
    }

    public T GetComponent<T>() where T : Component
    {
        Type type = typeof(T);
        for (int i = 0; i < r_components.Count; i++)
        {
            if (r_components[i].GetType() == type)
            {
                return (T)r_components[i];
            }
        }

        Logger.EngineWarn("Entity doesn't have a component of type {0}", type);
        return null;
    }

    public bool HasComponent<T>(out T component) where T : Component
    {
        Type type = typeof(T);
        for (int i = 0; i < r_components.Count; i++)
        {
            if (r_components[i].GetType() == type)
            {
                component = (T)r_components[i];
                return true;
            }
        }

        component = null;
        return false;
    }

    public string GetName()
    {
        return m_name;
    }

    public void SetName(string name)
    {
        m_name = name;
    }

    public TransformComponent GetTransform()
    {
        return m_transform;
    }
}
