namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;
    private float m_shadowMapPadding = 1f;

    internal DirectionalLight()
    {
        SetDirection(new Vector3(50f, -30f, 0f));
    }

    internal float[] GetCascadeDistances()
    {
        float[] cascadeDistances = Settings.Lighting.CascadeDistances;
        float farClippingPlane = Scene.GetActive().GetCamera().GetFarClippingPlane();

        float[] result = new float[Settings.Lighting.CascadeCount * 4];

        for (int i = 0; i < result.Length; i += 4)
        {
            result[i] = cascadeDistances[i / 4] * farClippingPlane;
        }

        return result;
    }

    internal Matrix4x4[] GetViewProjectionMatrices()
    {
        int cascadeCount = (int)Settings.Lighting.CascadeCount;
        Matrix4x4[] result = new Matrix4x4[cascadeCount];

        Camera camera = Scene.GetActive().GetCamera();
        float nearClippingPlane = camera.GetNearClippingPlane();
        float farClippingPlane = camera.GetFarClippingPlane();
        float[] cascadeDistances = Settings.Lighting.CascadeDistances;

        for (int i = 0; i < cascadeCount; i++)
        {
            float cascadeDistance = cascadeDistances[i] * farClippingPlane;

            BoundingSphere frustumBoundingSphere;
            if (i == 0)
            {
                frustumBoundingSphere = Scene.GetActive().GetCamera().GetFrustum(nearClippingPlane, cascadeDistance).GetBoundingSphere();
            }
            else
            {
                frustumBoundingSphere = Scene.GetActive().GetCamera().GetFrustum(cascadeDistances[i - 1] * farClippingPlane, cascadeDistance).GetBoundingSphere();
            }

            float csmTexelSize = 2f * (i + 1) / (float)Lighting.GetDirectionalShadowMapSize();

            Quaternion rotation = Quaternion.FromEulerAngles(m_direction);
            Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(frustumBoundingSphere.Center - rotation * Vector3.Forward, frustumBoundingSphere.Center, rotation * Vector3.Up);

            // Make sure that the texture always has the same size and an aspect ratio of 1:1
            // | This fixes shadow shimmering when the camera rotates
            // | (as long as the fov and aspect ratio of the camera don't change)
            float frustumSize = MathF.Floor(frustumBoundingSphere.Radius / csmTexelSize) * csmTexelSize;
            frustumSize += m_shadowMapPadding;

            Matrix4x4 viewProjection = viewMatrix * Matrix4x4.CreateOrthographic(frustumSize, frustumSize, -frustumSize, frustumSize);

            // Snap translation to texel grid
            // | This fixes shadow shimmering when the camera moves
            viewProjection.M41 -= viewProjection.M41 % csmTexelSize;
            viewProjection.M42 -= viewProjection.M42 % csmTexelSize;
            viewProjection.M43 -= viewProjection.M43 % csmTexelSize;

            result[i] = viewProjection;
        }

        return result;
    }

    public Vector3 GetDirection()
    {
        return m_direction;
    }

    public void SetDirection(Vector3 eulerAngles)
    {
        m_direction = eulerAngles;
    }

    public Colour GetColour()
    {
        return m_colour;
    }

    public void SetColour(Colour colour)
    {
        m_colour = colour;
    }

    public float GetIntensity()
    {
        return m_intensity;
    }

    public void SetIntensity(float intensity)
    {
        m_intensity = intensity;
    }

    public float GetShadowMapPadding()
    {
        return m_shadowMapPadding;
    }

    public void SetShadowMapPadding(float padding)
    {
        m_shadowMapPadding = padding;
    }
}
