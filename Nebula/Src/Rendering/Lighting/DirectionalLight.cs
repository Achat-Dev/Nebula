namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;
    private float m_shadowDistance = 20f;
    private float m_shadowMapPadding = 2f;

    private Matrix4x4 m_viewProjection = Matrix4x4.Identity;

    internal DirectionalLight()
    {
        SetDirection(new Vector3(50f, -30f, 0f));
    }

    internal void Update()
    {
        BoundingSphere frustumBoundingSphere = Scene.GetActive().GetCamera().GetFrustum(m_shadowDistance).GetBoundingSphere();

        // Replace this with the cascade level
        int csmLevel = 1;
        float csmTexelSize = 2f * csmLevel / (float)Lighting.GetDirectionalShadowMapSize();

        Quaternion rotation = Quaternion.FromEulerAngles(m_direction);
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(frustumBoundingSphere.Center - rotation * Vector3.Forward, frustumBoundingSphere.Center, rotation * Vector3.Up);

        // Make sure that the texture always has the same size and an aspect ratio of 1:1
        // | This fixes shadow shimmering when the camera rotates
        // | (as long as the fov and aspect ratio of the camera don't change)
        float frustumSize = MathF.Floor(frustumBoundingSphere.Radius / csmTexelSize) * csmTexelSize;
        frustumSize += m_shadowMapPadding;

        m_viewProjection = viewMatrix * Matrix4x4.CreateOrthographic(frustumSize, frustumSize, -frustumSize, frustumSize);

        // Snap translation to texel grid
        // | This fixes shadow shimmering when the camera moves
        m_viewProjection.M41 -= m_viewProjection.M41 % csmTexelSize;
        m_viewProjection.M42 -= m_viewProjection.M42 % csmTexelSize;
        m_viewProjection.M43 -= m_viewProjection.M43 % csmTexelSize;
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        return m_viewProjection;
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

    public float GetShadowDistance()
    {
        return m_shadowDistance;
    }

    public void SetShadowDistance(float shadowDistance)
    {
        m_shadowDistance = shadowDistance;
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
