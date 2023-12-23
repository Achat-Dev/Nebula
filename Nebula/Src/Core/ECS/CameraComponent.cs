using System.Numerics;

namespace Nebula;

public class CameraComponent : Component
{
    private float m_aspectRatio = 16f / 9f;
    private float m_fov = MathHelper.DegreesToRadians(45f);
    private float m_nearClippingPlane = 0.001f;
    private float m_farClippingPlane = 500f;
    private Matrix4x4 m_projectionMatrix;

    public CameraComponent()
    {
        RecalculateProjectionMatrix();
    }

    public void SetClippingPlanes(float near, float far)
    {
        m_nearClippingPlane = near;
        m_farClippingPlane = far;
        RecalculateProjectionMatrix();
    }

    public void SetFov(float fov)
    {
        m_fov = fov;
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
}
