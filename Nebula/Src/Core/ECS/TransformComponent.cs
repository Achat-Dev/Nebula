using System.Numerics;

namespace Nebula;

public class TransformComponent : Component
{
    private Vector3 m_localPosition = Vector3.Zero;
    private Vector3 m_localScale = Vector3.One;
    private Quaternion m_localRotation = Quaternion.Identity;

    public void Translate(Vector3 translation)
    {
        m_localPosition += translation;
    }

    public void Rotate(Quaternion rotation)
    {
        m_localRotation *= rotation;
    }

    public void SetLocalScale(Vector3 scale)
    {
        m_localScale = scale;
    }

    public Vector3 GetWorldPosition()
    {
        // Rework once parenting is introduced
        return m_localPosition;
    }

    public void SetWorldPosition(Vector3 position)
    {
        // Rework once parenting is introduced
        m_localPosition = position;
    }

    public Quaternion GetWorldRotation()
    {
        // Rework once parenting is introduced
        return m_localRotation;
    }

    public Vector3 GetRight()
    {
        return GetWorldRotation() * Vector3.Right;
    }

    public Vector3 GetUp()
    {
        return GetWorldRotation() * Vector3.Up;
    }

    public Vector3 GetForward()
    {
        return GetWorldRotation() * Vector3.Forward;
    }

    public Matrix4x4 GetWorldMatrix()
    {
        return Matrix4x4.CreateFromQuaternion(m_localRotation) * Matrix4x4.CreateScale(m_localScale) * Matrix4x4.CreateTranslation(m_localPosition);
    }
}
