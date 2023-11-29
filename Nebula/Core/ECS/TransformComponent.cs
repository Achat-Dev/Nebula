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

    public Matrix4x4 GetWorldMatrix()
    {
        return Matrix4x4.CreateFromQuaternion(m_localRotation) * Matrix4x4.CreateScale(m_localScale) * Matrix4x4.CreateTranslation(m_localPosition);
    }
}
