using Nebula;

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
    public override void OnCreate()
    {
        Logger.Info("Creating test updateble component");
    }

    public override void OnUpdate(float deltaTime)
    {
        if (Input.WasKeyPressed(Key.Escape))
        {
            Game.Close();
        }
    }
}
