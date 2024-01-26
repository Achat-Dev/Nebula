namespace Nebula.Rendering;

internal static class Lighting
{
    private static DirectionalLight s_directionalLight = new DirectionalLight();
    private static readonly List<PointLightComponent> s_pointLights = new List<PointLightComponent>();
    private static int s_pointLightCount = 0;

    public static DirectionalLight GetDirectionalLight()
    {
        return s_directionalLight;
    }

    public static void AddPointLight(PointLightComponent pointLight)
    {
        s_pointLights.Add(pointLight);
        s_pointLightCount++;
    }

    public static void RemovePointLight(PointLightComponent pointLight)
    {
        s_pointLights.Remove(pointLight);
        s_pointLightCount = Math.Max(0, s_pointLightCount - 1);
    }

    public static List<PointLightComponent> GetPointLights()
    {
        return s_pointLights;
    }

    public static int GetPointLightCount()
    {
        return s_pointLightCount;
    }
}
