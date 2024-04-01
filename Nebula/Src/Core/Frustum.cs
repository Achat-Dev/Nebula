namespace Nebula;

internal class Frustum
{
    private enum CornerType
    {
        BottomLeftNear = 0,
        TopLeftNear = 1,
        TopRightNear = 2,
        BottomRightNear = 3,

        BottomLeftFar = 4,
        TopLeftFar = 5,
        TopRightFar = 6,
        BottomRightFar = 7,
    }

    private Vector3 m_center;
    private Vector3[] m_corners = new Vector3[8];

    public Frustum(float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane, TransformComponent transform)
    {
        fov = MathHelper.DegreesToRadians(fov);

        // Setup
        Vector3 position = transform.GetWorldPosition();
        Vector3 forward = transform.GetForward();
        Vector3 up = transform.GetUp();
        Vector3 right = transform.GetRight();

        float heightNear = 2f * MathF.Tan(fov * 0.5f) * nearClippingPlane;
        float widthNear = heightNear * aspectRatio;
        Vector3 centerNear = position + forward * nearClippingPlane;
        Vector3 rightNear = right * (widthNear * 0.5f);
        Vector3 upNear = up * (heightNear * 0.5f);

        float heightFar = 2f * MathF.Tan(fov * 0.5f) * farClippingPlane;
        float widthFar = heightFar * aspectRatio;
        Vector3 centerFar = position + forward * farClippingPlane;
        Vector3 rightFar = right * (widthFar * 0.5f);
        Vector3 upFar = up * (heightFar * 0.5f);

        // Near face
        m_corners[(int)CornerType.BottomLeftNear] = centerNear - rightNear - upNear;
        m_corners[(int)CornerType.TopLeftNear] = centerNear - rightNear + upNear;
        m_corners[(int)CornerType.TopRightNear] = centerNear + rightNear + upNear;
        m_corners[(int)CornerType.BottomRightNear] = centerNear + rightNear - upNear;

        // Far face
        m_corners[(int)CornerType.BottomLeftFar] = centerFar - rightFar - upFar;
        m_corners[(int)CornerType.TopLeftFar] = centerFar - rightFar + upFar;
        m_corners[(int)CornerType.TopRightFar] = centerFar + rightFar + upFar;
        m_corners[(int)CornerType.BottomRightFar] = centerFar + rightFar - upFar;

        // Calculate center
        for (int i = 0; i < m_corners.Length; i++)
        {
            m_center += m_corners[i];
        }
        m_center /= m_corners.Length;
    }

    public Vector3 GetCenter()
    {
        return m_center;
    }

    public Vector3[] GetCorners()
    {
        return m_corners;
    }

    public Vector3 GetBottomLeftNear()
    {
        return m_corners[(int)CornerType.BottomLeftNear];
    }

    public Vector3 GetTopLeftNear()
    {
        return m_corners[(int)CornerType.TopLeftNear];
    }

    public Vector3 GetTopRightNear()
    {
        return m_corners[(int)CornerType.TopRightNear];
    }

    public Vector3 GetBottomRightNear()
    {
        return m_corners[(int)CornerType.BottomRightNear];
    }

    public Vector3 GetBottomLeftFar()
    {
        return m_corners[(int)CornerType.BottomLeftFar];
    }

    public Vector3 GetTopLeftFar()
    {
        return m_corners[(int)CornerType.TopLeftFar];
    }

    public Vector3 GetTopRightFar()
    {
        return m_corners[(int)CornerType.TopRightFar];
    }

    public Vector3 GetBottomRightFar()
    {
        return m_corners[(int)CornerType.BottomLeftFar];
    }
}
