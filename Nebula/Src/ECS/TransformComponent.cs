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

    public void Scale(float scale)
    {
        m_localScale *= scale;
    }

    public void Scale(Vector3 scale)
    {
        m_localScale *= scale;
    }

    public Vector3 GetLocalPosition()
    {
        return m_localPosition;
    }

    public void SetLocalPosition(Vector3 position)
    {
        m_localPosition = position;
    }

    public Quaternion GetLocalRotation()
    {
        return m_localRotation;
    }

    public void SetLocalRotation(Vector3 eulerAngles)
    {
        m_localRotation = Quaternion.FromEulerAngles(eulerAngles);
    }

    public void SetLocalRotation(Quaternion rotation)
    {
        m_localRotation = rotation;
    }

    public Vector3 GetLocalScale()
    {
        return m_localScale;
    }

    public void SetLocalScale(Vector3 scale)
    {
        m_localScale = scale;
    }

    public Vector3 GetWorldPosition()
    {
        // Properly implement once parenting is implemented
        return m_localPosition;
    }

    public void SetWorldPosition(Vector3 position)
    {
        // Properly implement once parenting is implemented
        m_localPosition = position;
    }

    public Quaternion GetWorldRotation()
    {
        // Properly implement once parenting is implemented
        return m_localRotation;
    }

    public void SetWorldRotation(Vector3 eulerAngles)
    {
        // Properly implement once parenting is implemented
        m_localRotation = Quaternion.FromEulerAngles(eulerAngles);
    }

    public void SetWorldRotation(Quaternion rotation)
    {
        // Properly implement once parenting is implemented
        m_localRotation = rotation;
    }

    public Vector3 GetWorldScale()
    {
        // Properly implement once parenting is implemented
        return m_localScale;
    }

    public void SetWorldScale(Vector3 scale)
    {
        // Properly implement once parenting is implemented
        m_localScale = scale;
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
        return Matrix4x4.CreateScale(m_localScale) * Matrix4x4.CreateRotation(m_localRotation) * Matrix4x4.CreateTranslation(m_localPosition);
    }
}
