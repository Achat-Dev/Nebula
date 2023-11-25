using Nebula;

namespace Sandbox;

public class Program
{
    static void Main(string[] args)
    {
        Game game = new Game("Sandbox", new Vector2i(1280, 720), true);
        game.Start();
    }
}
