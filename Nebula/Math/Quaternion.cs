using Silk.NET.Maths;

namespace Nebula;

public struct Quaternion : IEquatable<Quaternion>
{
    public float X = 0f;
    public float Y = 0f;
    public float Z = 0f;
    public float W = 1f;

    public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);

    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /* -------------------- Methods -------------------- */

    public Vector3 GetXyz()
    {
        return new Vector3(X, Y, Z);
    }

    public void Invert()
    {
        float sqrMagnitude = SqrMagnitude();
        if (sqrMagnitude != 0.0f)
        {
            float coefficient = 1f / sqrMagnitude;
            X *= coefficient;
            Y *= coefficient;
            Z *= coefficient;
            W *= coefficient;
        }
    }

    public Quaternion Inverted()
    {
        Quaternion quaternion = this;
        quaternion.Invert();
        return quaternion;
    }

    public float Magnitude()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
    }

    public float SqrMagnitude()
    {
        return X * X + Y * Y + Z * Z + W * W;
    }

    /* -------------------- Static Methods -------------------- */

    public static Quaternion FromAxisAngle(Vector3 axis, float angle)
    {
        if (axis == Vector3.Zero)
        {
            Logger.EngineWarn("Illegal operation: Trying to create a quaternion via FromAxisAngle with Vector3.Zero as the axis");
            return Identity;
        }

        angle = MathHelper.DegreesToRadians(angle);
        angle *= 0.5f;
        axis.Normalise();
        Vector3 xyz = axis * MathF.Sin(angle);

        Quaternion result = Identity;
        result.X = xyz.X;
        result.Y = xyz.Y;
        result.Z = xyz.Z;
        result.W = MathF.Cos(angle);

        return result;
    }

    /* -------------------- Operators -------------------- */

    public static Quaternion operator +(Quaternion left, Quaternion right)
    {
        left.X += right.X;
        left.Y += right.Y;
        left.Z += right.Z;
        left.W += right.W;
        return left;
    }

    public static Quaternion operator -(Quaternion left, Quaternion right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        left.Z -= right.Z;
        left.W -= right.W;
        return left;
    }

    public static Quaternion operator *(Quaternion value, float scale)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        value.W *= scale;
        return value;
    }

    public static Quaternion operator *(float scale, Quaternion value)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        value.W *= scale;
        return value;
    }

    public static Quaternion operator *(Quaternion left, Quaternion right)
    {
        Vector3 leftXyz = left.GetXyz();
        Vector3 rightXyz = right.GetXyz();

        Vector3 xyz = right.W * leftXyz + rightXyz * left.W;
        xyz += Vector3.Cross(leftXyz, rightXyz);

        float w = left.W * right.W;
        w -= Vector3.Dot(leftXyz, rightXyz);

        return new Quaternion(xyz.X, xyz.Y, xyz.Z, w);
    }

    public static bool operator ==(Quaternion left, Quaternion right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
    }

    public static bool operator !=(Quaternion left, Quaternion right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Quaternion<float>(Quaternion value)
    {
        return new Quaternion<float>(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator Quaternion(Quaternion<float> value)
    {
        return new Quaternion(value.X, value.Y, value.Z, value.W);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Quaternion other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Quaternion other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z, W);
    }

    public override string ToString()
    {
        return $"({X} | {Y} | {Z} | {W})";
    }
}
