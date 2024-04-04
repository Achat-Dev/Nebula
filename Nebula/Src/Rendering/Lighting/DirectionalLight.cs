namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;
    private float m_shadowDistance = 20f;
    private float m_shadowMapPadding = 2f;
    private float m_projectionZScale = 5f;

    private Matrix4x4 m_viewProjection = Matrix4x4.Identity;

    internal DirectionalLight()
    {
        SetDirection(new Vector3(50f, -30f, 0f));
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        Frustum frustum = Scene.GetActive().GetCamera().GetFrustum(m_shadowDistance);

        float shadowMapSize = (float)Lighting.GetDirectionalShadowMapSize();
        float texelSize = 1f / shadowMapSize;
        int csmSplit = 1;
        float csmTexelSize = 2f * csmSplit / shadowMapSize;

        Vector3 position = frustum.GetCenter();

        Quaternion rotation = Quaternion.FromEulerAngles(m_direction);
        Vector3 forward = rotation * Vector3.Forward;
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(position - forward, position, rotation * Vector3.Up);

        // Calculate min and max coordinates of the camera frustum
        Vector4 point;
        float left = float.PositiveInfinity;
        float right = float.NegativeInfinity;
        float bottom = float.PositiveInfinity;
        float top = float.NegativeInfinity;
        float near = float.PositiveInfinity;
        float far = float.NegativeInfinity;
        Vector3[] corners = frustum.GetCorners();
        for (int i = 0; i < corners.Length; i++)
        {
            point = viewMatrix * new Vector4(corners[i].X, corners[i].Y, corners[i].Z, 1f);
            left = MathF.Min(left, point.X);
            right = MathF.Max(right, point.X);
            bottom = MathF.Min(bottom, point.Y);
            top = MathF.Max(top, point.Y);
            near = MathF.Min(near, point.Z);
            far = MathF.Max(far, point.Z);
        }

        // Make frustum an aspect ratio of 1:1
        float distanceX = right - left;
        float distanceY = top - bottom;
        float distanceZ = far - near;

        //float maxDistance = MathF.Max(MathF.Max(distanceX, distanceY), distanceZ);
        float maxDistance = MathF.Max(distanceX, distanceY);

        float halfX = (maxDistance - distanceX) / 2f;
        //left -= halfX;
        //right += halfX;

        float halfY = (maxDistance - distanceY) / 2f;
        //bottom -= halfY;
        //top += halfY;

        float halfZ = (maxDistance - distanceZ) / 2f;
        //near -= halfZ + m_shadowMapPadding;
        //far += halfZ + m_shadowMapPadding;

        // Scale near and far clipping plane
        if (near < 0)
        {
            near *= m_projectionZScale;
        }
        else
        {
            near /= m_projectionZScale;
        }

        if (far < 0)
        {
            far /= m_projectionZScale;
        }
        else
        {
            far *= m_projectionZScale;
        }

        float width = right - left;
        float height = top - bottom;

        m_viewProjection = viewMatrix * Matrix4x4.CreateOrthographic(width, height, near, far);

        m_viewProjection.M41 -= m_viewProjection.M41 % csmTexelSize;
        m_viewProjection.M42 -= m_viewProjection.M42 % csmTexelSize;
        m_viewProjection.M43 -= m_viewProjection.M43 % csmTexelSize;

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

    public float GetProjectionZScale()
    {
        return m_projectionZScale;
    }

    public void SetProjectionZScale(float scale)
    {
        m_projectionZScale = scale;
    }
}
