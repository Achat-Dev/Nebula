namespace Nebula;

// This is later probably going to be replaced with a config file
public static class Settings
{
    public static class Lighting
    {
        public static uint CascadeCount = 1;

        // Cascade distances are relative to the cameras far clipping plane
        public static float[] CascadeDistances = new float[0];
    }
}
