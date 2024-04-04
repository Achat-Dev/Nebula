namespace Nebula;

public abstract class Component
{
    protected Entity m_entity;

    public Entity GetEntity()
    {
        return m_entity;
    }

    internal void SetEntity(Entity entity)
    {
        m_entity = entity;
    }

    public void Destroy()
    {
        m_entity.RemoveComponent(this);
    }

    public virtual void OnDestroy()
    {
        m_entity = null;
    }
}
