namespace Nebula;

public class Camera : IDisposable
{
    private TransformComponent m_transform = new TransformComponent();

    private float m_aspectRatio = 16f / 9f;
    private float m_fov = 45f;
    private float m_nearClippingPlane = 0.001f;
    private float m_farClippingPlane = 100f;

    private Matrix4x4 m_viewMatrix = Matrix4x4.Identity;
    private Matrix4x4 m_projectionMatrix = Matrix4x4.Identity;
    private Matrix4x4 m_viewProjectionMatrix = Matrix4x4.Identity;

    internal Camera()
    {
        Game.Resizing += OnResize;
        OnResize(Game.GetWindowSize());
    }

    internal void Update()
    {
        Vector3 position = m_transform.GetWorldPosition();
        m_viewMatrix = Matrix4x4.CreateLookAtLeftHanded(position, position + m_transform.GetForward(), m_transform.GetUp());
        m_viewProjectionMatrix = m_viewMatrix * m_projectionMatrix;
    }

    private void OnResize(Vector2i size)
    {
        m_aspectRatio = (float)size.X / (float)size.Y;
        RecalculateProjectionMatrix();
    }

    private void RecalculateProjectionMatrix()
    {
        m_projectionMatrix = Matrix4x4.CreatePerspectiveLeftHanded(m_fov, m_aspectRatio, m_nearClippingPlane, m_farClippingPlane);
    }

    internal Frustum GetFrustum(float nearClippingPlane, float farClippingPlane)
    {
        nearClippingPlane = nearClippingPlane < m_nearClippingPlane ? m_nearClippingPlane : nearClippingPlane;
        farClippingPlane = farClippingPlane > m_farClippingPlane ? m_farClippingPlane : farClippingPlane;
        return Frustum.FromPerspective(m_transform, m_fov, m_aspectRatio, nearClippingPlane, farClippingPlane);
    }

    public TransformComponent GetTransform()
    {
        return m_transform;
    }

    public float GetNearClippingPlane()
    {
        return m_nearClippingPlane;
    }

    public float GetFarClippingPlane()
    {
        return m_farClippingPlane;
    }

    public void SetClippingPlanes(float near, float far)
    {
        m_nearClippingPlane = near;
        m_farClippingPlane = far;
        RecalculateProjectionMatrix();
    }

    public float GetFov()
    {
        return m_fov;
    }

    public void SetFov(float fov)
    {
        m_fov = fov;
        RecalculateProjectionMatrix();
    }

    internal Matrix4x4 GetViewMatrix()
    {
        return m_viewMatrix;
    }

    internal Matrix4x4 GetProjectionMatrix()
    {
        return m_projectionMatrix;
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        return m_viewProjectionMatrix;
    }

    void IDisposable.Dispose()
    {
        Game.Resizing -= OnResize;
    }
}
