using Nebula;

namespace Sandbox;

public class Program
{
    public static void Main(string[] args)
    {
        Game game = new Game("Sandbox", new Vector2i(1280, 720), true);

        Entity entity = new Entity();
        entity.AddComponent<TestComponent>();
        entity.AddComponent<TestStartableComponent>();
        entity.AddComponent<TestUpdateableComponent>();

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
    public override void OnCreate()
    {
        Logger.Info("Creating test updateble component");
    }

    public override void OnUpdate(float deltaTime)
    {
        if (Input.IsKeyPressed(Key.Escape))
        {
            Game.Close();
        }
    }
}
