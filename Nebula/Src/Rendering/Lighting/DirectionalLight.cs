namespace Nebula.Rendering;

public class DirectionalLight
{
    private Vector3 m_direction;
    private Colour m_colour = Colour.White;
    private float m_intensity = 1f;

    internal float m_shadowDistance = 20f;

    internal DirectionalLight()
    {
        SetDirection(new Vector3(50f, -30f, 0f));
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        Frustum frustum = Scene.GetActive().GetCamera().GetFrustum(m_shadowDistance);

        Quaternion rotation = Quaternion.FromEulerAngles(m_direction);
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(frustum.GetCenter() - (rotation * Vector3.Forward), frustum.GetCenter(), rotation * Vector3.Up);

        // Calculate min and max coordinates of the camera frustum
        Vector4 point;
        float minX = float.PositiveInfinity;
        float maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity;
        float maxY = float.NegativeInfinity;
        float minZ = float.PositiveInfinity;
        float maxZ = float.NegativeInfinity;
        Vector3[] corners = frustum.GetCorners();
        for (int i = 0; i < corners.Length; i++)
        {
            point = viewMatrix * new Vector4(corners[i].X, corners[i].Y, corners[i].Z, 1f);
            minX = MathF.Min(minX, point.X);
            maxX = MathF.Max(maxX, point.X);
            minY = MathF.Min(minY, point.Y);
            maxY = MathF.Max(maxY, point.Y);
            minZ = MathF.Min(minZ, point.Z);
            maxZ = MathF.Max(maxZ, point.Z);
        }

        // Make frustum an aspect ratio of 1:1
        float distanceX = maxX - minX;
        float distanceY = maxY - minY;
        float distanceZ = maxZ - minZ;

        float maxDistance = MathF.Max(MathF.Max(distanceX, distanceY), distanceZ);

        float padding = 2f;

        float halfX = (maxDistance - distanceX) / 2f;
        minX -= halfX + padding;
        maxX += halfX + padding;

        float halfY = (maxDistance - distanceY) / 2f;
        minY -= halfY + padding;
        maxY += halfY + padding;

        float halfZ = (maxDistance - distanceZ) / 2f;
        minZ -= halfZ + padding;
        maxZ += halfZ + padding;

        // Scale near and far clipping plane
        float zScale = 5f;
        if (minZ < 0)
        {
            minZ *= zScale;
        }
        else
        {
            minZ /= zScale;
        }

        if (maxZ < 0)
        {
            maxZ /= zScale;
        }
        else
        {
            maxZ *= zScale;
        }

        Matrix4x4 projectionMatrix = Matrix4x4.CreateOrthographic(minX, maxX, minY, maxY, minZ, maxZ);

        return viewMatrix * projectionMatrix;
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
}
