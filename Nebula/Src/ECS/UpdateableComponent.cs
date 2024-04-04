namespace Nebula;

public abstract class UpdateableComponent : StartableComponent
{
    public abstract void OnUpdate(float deltaTime);
}
