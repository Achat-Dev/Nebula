namespace Nebula;

internal class Frustum
{
    private struct CornerType
    {
        public const int NearBottomLeft = 0;
        public const int NearTopLeft = 1;
        public const int NearTopRight = 2;
        public const int NearBottomRight = 3;

        public const int FarBottomLeft = 4;
        public const int FarTopLeft = 5;
        public const int FarTopRight = 6;
        public const int FarBottomRight = 7;
    }

    private Vector3 m_center;
    private Vector3[] m_corners = new Vector3[8];

    private Frustum(TransformComponent transform, float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane)
    {
        fov = Utils.MathUtils.DegreesToRadians(fov);

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
        m_corners[CornerType.NearBottomLeft] = centerNear - rightNear - upNear;
        m_corners[CornerType.NearTopLeft] = centerNear - rightNear + upNear;
        m_corners[CornerType.NearTopRight] = centerNear + rightNear + upNear;
        m_corners[CornerType.NearBottomRight] = centerNear + rightNear - upNear;

        // Far face
        m_corners[CornerType.FarBottomLeft] = centerFar - rightFar - upFar;
        m_corners[CornerType.FarTopLeft] = centerFar - rightFar + upFar;
        m_corners[CornerType.FarTopRight] = centerFar + rightFar + upFar;
        m_corners[CornerType.FarBottomRight] = centerFar + rightFar - upFar;

        // Center
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
        m_corners[CornerType.NearBottomLeft] = centerNear - distanceRight - distanceUp;
        m_corners[CornerType.NearTopLeft] = centerNear - distanceRight + distanceUp;
        m_corners[CornerType.NearTopRight] = centerNear + distanceRight + distanceUp;
        m_corners[CornerType.NearBottomRight] = centerNear + distanceRight - distanceUp;

        // Far face
        m_corners[CornerType.FarBottomLeft] = centerFar - distanceRight - distanceUp;
        m_corners[CornerType.FarTopLeft] = centerFar - distanceRight + distanceUp;
        m_corners[CornerType.FarTopRight] = centerFar + distanceRight + distanceUp;
        m_corners[CornerType.FarBottomRight] = centerFar + distanceRight - distanceUp;
    }

    public static Frustum FromPerspective(TransformComponent transform, float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane)
    {
        return new Frustum(transform, fov, aspectRatio, nearClippingPlane, farClippingPlane);
    }

    public static Frustum FromOrthographic(Vector3 center, Quaternion rotation, float distanceX, float distanceY, float distanceZ)
    {
        return new Frustum(center, rotation, distanceX, distanceY, distanceZ);
    }

    public BoundingSphere GetBoundingSphere()
    {
        return new BoundingSphere(this);
    }

    public Vector3 GetBottomLeftNear()
    {
        return m_corners[CornerType.NearBottomLeft];
    }

    public Vector3 GetTopLeftNear()
    {
        return m_corners[CornerType.NearTopLeft];
    }

    public Vector3 GetTopRightNear()
    {
        return m_corners[CornerType.NearTopRight];
    }

    public Vector3 GetBottomRightNear()
    {
        return m_corners[CornerType.NearBottomRight];
    }

    public Vector3 GetBottomLeftFar()
    {
        return m_corners[CornerType.FarBottomLeft];
    }

    public Vector3 GetTopLeftFar()
    {
        return m_corners[CornerType.FarTopLeft];
    }

    public Vector3 GetTopRightFar()
    {
        return m_corners[CornerType.FarTopRight];
    }

    public Vector3 GetBottomRightFar()
    {
        return m_corners[CornerType.FarBottomLeft];
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
