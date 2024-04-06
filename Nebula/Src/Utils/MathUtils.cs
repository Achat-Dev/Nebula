namespace Nebula.Utils;

public static class MathUtils
{
    public static float RadiansToDegrees(float radians)
    {
        return radians * (180f / MathF.PI);
    }

    public static float DegreesToRadians(float degrees)
    {
        return degrees * (MathF.PI / 180f);
    }
}
