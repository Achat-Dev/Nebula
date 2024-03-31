namespace Nebula.Rendering;

public class SkyboxConfig
{
    public Vector2i EnvironmentMapSize;
    public Vector2i IrradianceMapSize;
    public Vector2i PrefilteredMapSize;

    public class Defaults
    {
        public static SkyboxConfig Small()
        {
            return new SkyboxConfig(new Vector2i(512, 512), new Vector2i(32, 32), new Vector2i(128, 128));
        }

        public static SkyboxConfig Medium()
        {
            return new SkyboxConfig(new Vector2i(1024, 1024), new Vector2i(64, 64), new Vector2i(256, 256));
        }

        public static SkyboxConfig High()
        {
            return new SkyboxConfig(new Vector2i(2048, 2048), new Vector2i(128, 128), new Vector2i(512, 512));
        }
    }

    public SkyboxConfig(Vector2i environmentMapSize, Vector2i irradianceMapSize, Vector2i prefilteredMapSize)
    {
        EnvironmentMapSize = environmentMapSize;
        IrradianceMapSize = irradianceMapSize;
        PrefilteredMapSize = prefilteredMapSize;
    }
}
