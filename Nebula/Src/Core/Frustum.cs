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

    private Frustum(TransformComponent transform, float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane)
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

    private Frustum(Vector3 center, Quaternion rotation, float distanceX, float distanceY, float distanceZ)
    {
        m_center = center;

        Vector3 forward = rotation * Vector3.Forward;
        Vector3 up = rotation * Vector3.Up;
        Vector3 right = rotation * Vector3.Right;

        Vector3 distanceRight = right * (distanceX * 0.5f);
        Vector3 distanceUp = up * (distanceY * 0.5f);
        Vector3 distanceForward = forward * (distanceZ * 0.5f);

        Vector3 centerNear = center - distanceForward;
        Vector3 centerFar = center + distanceForward;

        // Near face
        m_corners[(int)CornerType.BottomLeftNear] = centerNear - distanceRight - distanceUp;
        m_corners[(int)CornerType.TopLeftNear] = centerNear - distanceRight + distanceUp;
        m_corners[(int)CornerType.TopRightNear] = centerNear + distanceRight + distanceUp;
        m_corners[(int)CornerType.BottomRightNear] = centerNear + distanceRight - distanceUp;

        // Far face
        m_corners[(int)CornerType.BottomLeftFar] = centerFar - distanceRight - distanceUp;
        m_corners[(int)CornerType.TopLeftFar] = centerFar - distanceRight + distanceUp;
        m_corners[(int)CornerType.TopRightFar] = centerFar + distanceRight + distanceUp;
        m_corners[(int)CornerType.BottomRightFar] = centerFar + distanceRight - distanceUp;
    }

    public static Frustum FromPerspective(TransformComponent transform, float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane)
    {
        return new Frustum(transform, fov, aspectRatio, nearClippingPlane, farClippingPlane);
    }

    // The position of the transform is treated as the center of the frustum
    public static Frustum FromOrthographic(Vector3 center, Quaternion rotation, float distanceX, float distanceY, float distanceZ)
    {
        return new Frustum(center, rotation, distanceX, distanceY, distanceZ);
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

    public Vector3 GetCenter()
    {
        return m_center;
    }

    public Vector3[] GetCorners()
    {
        return m_corners;
    }
}
