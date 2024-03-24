namespace Nebula.Utils;

public static class CubemapUtils
{
    public static Matrix4x4[] GetViewMatrices(Vector3 position)
    {
        return new Matrix4x4[]
        {
            Matrix4x4.CreateLookAt(position, position + Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(position, position - Vector3.Right, -Vector3.Up),
            Matrix4x4.CreateLookAt(position, position + Vector3.Up, Vector3.Forward),
            Matrix4x4.CreateLookAt(position, position - Vector3.Up, -Vector3.Forward),
            Matrix4x4.CreateLookAt(position, position + Vector3.Forward, -Vector3.Up),
            Matrix4x4.CreateLookAt(position, position - Vector3.Forward, -Vector3.Up),
        };
    }
}
