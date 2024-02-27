using Nebula.Rendering;

namespace Nebula;

public class CameraComponent : StartableComponent
{
    private float m_aspectRatio = 16f / 9f;
    private float m_fov = MathHelper.DegreesToRadians(45f);
    private float m_nearClippingPlane = 0.001f;
    private float m_farClippingPlane = 500f;
    private Matrix4x4 m_projectionMatrix;
    private Framebuffer m_framebuffer;

    public override void OnCreate()
    {
        Window.Resizing += OnResize;

        Vector2i windowSize = Game.GetWindowSize();

        FramebufferAttachment colourAttachment = new FramebufferAttachment(windowSize, FramebufferAttachment.AttachmentType.Colour, FramebufferAttachment.ReadWriteMode.Readable);
        FramebufferAttachment depthAttachment = new FramebufferAttachment(windowSize, FramebufferAttachment.AttachmentType.Depth, FramebufferAttachment.ReadWriteMode.Writeonly);
        m_framebuffer = new Framebuffer(windowSize, colourAttachment, depthAttachment);

        OnResize(windowSize);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        m_framebuffer.Dispose();
        Window.Resizing -= OnResize;
    }

    private void OnResize(Vector2i size)
    {
        m_aspectRatio = (float)size.X / (float)size.Y;
        m_framebuffer.Resize(size);
        RecalculateProjectionMatrix();
    }

    private void RecalculateProjectionMatrix()
    {
        m_projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(m_fov, m_aspectRatio, m_nearClippingPlane, m_farClippingPlane);
    }

    internal Matrix4x4 GetViewProjectionMatrix()
    {
        TransformComponent transform = m_entity.GetTransform();
        Vector3 position = transform.GetWorldPosition();
        Matrix4x4 viewMatrix = Matrix4x4.CreateLookAtLeftHanded(position, position + transform.GetForward(), transform.GetUp());
        return viewMatrix * m_projectionMatrix;
    }

    internal Framebuffer GetFramebuffer()
    {
        return m_framebuffer;
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
}
