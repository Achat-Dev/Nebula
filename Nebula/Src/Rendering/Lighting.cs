using System.Runtime.InteropServices;

namespace Nebula.Rendering;

public static class Lighting
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PointLightData
    {
        public float Range;
        private float _;
        private float __;
        private float ___;
        public Vector3 Position;
        private float ____;
        public Vector3 Colour;
        private float _____;
    }

    private static DirectionalLight s_directionalLight = new DirectionalLight();
    private static readonly List<PointLightComponent> s_pointLights = new List<PointLightComponent>();
    private static int s_pointLightCount = 0;

    internal static void AddPointLight(PointLightComponent pointLight)
    {
        s_pointLights.Add(pointLight);
        s_pointLightCount++;
    }

    internal static void RemovePointLight(PointLightComponent pointLight)
    {
        s_pointLights.Remove(pointLight);
        s_pointLightCount = Math.Max(0, s_pointLightCount - 1);
    }

    internal static PointLightData[] GetPointLightData()
    {
        PointLightData[] data = new PointLightData[s_pointLightCount];
        for (int i = 0; i < s_pointLightCount; i++)
        {
            data[i].Position = s_pointLights[i].GetEntity().GetTransform().GetWorldPosition();
            data[i].Colour = ((Vector3)s_pointLights[i].GetColour()) * s_pointLights[i].GetIntensity();
            data[i].Range = s_pointLights[i].GetRange();
        }
        return data;
    }

    public static DirectionalLight GetDirectionalLight()
    {
        return s_directionalLight;
    }

    public static int GetPointLightCount()
    {
        return s_pointLightCount;
    }
}
