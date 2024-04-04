using Nebula.Rendering;

namespace Nebula;

public class Camera : IDisposable
{
    private TransformComponent m_transform = new TransformComponent();

    private float m_aspectRatio = 16f / 9f;
    private float m_fov = 45f;
    private float m_nearClippingPlane = 0.001f;
    private float m_farClippingPlane = 100f;
    private Matrix4x4 m_projectionMatrix;
    private Framebuffer m_framebuffer;

    internal Camera()
    {
        Window.Resizing += OnResize;
        Vector2i windowSize = Game.GetWindowSize();
        m_framebuffer = new Framebuffer(windowSize, FramebufferAttachmentConfig.Defaults.Colour(), FramebufferAttachmentConfig.Defaults.DepthStencil());
        OnResize(windowSize);
    }

    private void OnResize(Vector2i size)
    {
        m_aspectRatio = (float)size.X / (float)size.Y;
        m_framebuffer.Resize(size);
        RecalculateProjectionMatrix();
    }

    private void RecalculateProjectionMatrix()
    {
        m_projectionMatrix = Matrix4x4.CreatePerspectiveLeftHanded(m_fov, m_aspectRatio, m_nearClippingPlane, m_farClippingPlane);
    }

    internal Frustum GetFrustum(float maxDistance)
    {
        float farClippingPlane = maxDistance > m_farClippingPlane ? m_farClippingPlane : maxDistance + m_nearClippingPlane;
        return Frustum.FromPerspective(m_transform, m_fov, m_aspectRatio, m_nearClippingPlane, farClippingPlane);
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        Vector3 position = m_transform.GetWorldPosition();
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAtLeftHanded(position, position + m_transform.GetForward(), m_transform.GetUp());
        return viewMatrix * m_projectionMatrix;
    }

    internal Framebuffer GetFramebuffer()
    {
        return m_framebuffer;
    }

    public TransformComponent GetTransform()
    {
        return m_transform;
    }

    public Vector2 GetClippingPlanes()
    {
        return new Vector2(m_nearClippingPlane, m_farClippingPlane);
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

    void IDisposable.Dispose()
    {
        IDisposable disposable = m_framebuffer;
        disposable.Dispose();
        Window.Resizing -= OnResize;
    }
}
