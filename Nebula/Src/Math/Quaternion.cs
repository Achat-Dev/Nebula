namespace Nebula;

public struct Quaternion : IEquatable<Quaternion>
{
    public float X;
    public float Y;
    public float Z;
    public float W;

    public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);

    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /* -------------------- Methods -------------------- */

    internal Vector3 GetXyz()
    {
        return new Vector3(X, Y, Z);
    }

    public Vector3 GetEulerAngles()
    {
        Vector3 result;

        float a = 2f * (W * X + Y * Z);
        float b = 1f - 2f * (X * X + Y * Y);
        result.X = Utils.MathUtils.RadiansToDegrees(MathF.Atan2(a, b));

        a = MathF.Sqrt(1f + 2f * (W * Y - X * Z));
        b = MathF.Sqrt(1f - 2f * (W * Y - X * Z));
        result.Y = Utils.MathUtils.RadiansToDegrees(2f * MathF.Atan2(a, b) - MathF.PI * 0.5f);

        a = 2f * (W * Z + X * Y);
        b = 1f - 2f * (Y * Y + Z * Z);
        result.Z = Utils.MathUtils.RadiansToDegrees(MathF.Atan2(a, b));

        return result;
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

        angle = Utils.MathUtils.DegreesToRadians(angle);
        angle *= 0.5f;
        axis.Normalise();
        Vector3 xyz = axis * MathF.Sin(angle);

        return new Quaternion(xyz.X, xyz.Y, xyz.Z, MathF.Cos(angle));
    }

    public static Quaternion FromEulerAngles(Vector3 eulerAngles)
    {
        eulerAngles.X = Utils.MathUtils.DegreesToRadians(eulerAngles.X);
        eulerAngles.Y = Utils.MathUtils.DegreesToRadians(eulerAngles.Y);
        eulerAngles.Z = Utils.MathUtils.DegreesToRadians(eulerAngles.Z);

        float sinX = MathF.Sin(eulerAngles.X * 0.5f);
        float cosX = MathF.Cos(eulerAngles.X * 0.5f);
        float sinY = MathF.Sin(eulerAngles.Y * 0.5f);
        float cosY = MathF.Cos(eulerAngles.Y * 0.5f);
        float sinZ = MathF.Sin(eulerAngles.Z * 0.5f);
        float cosZ = MathF.Cos(eulerAngles.Z * 0.5f);

        Quaternion result;

        result.X = sinX * cosY * cosZ - cosX * sinY * sinZ;
        result.Y = cosX * sinY * cosZ + sinX * cosY * sinZ;
        result.Z = cosX * cosY * sinZ - sinX * sinY * cosZ;
        result.W = cosX * cosY * cosZ + sinX * sinY * sinZ;

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

    public static implicit operator Silk.NET.Maths.Quaternion<float>(Quaternion value)
    {
        return new Silk.NET.Maths.Quaternion<float>(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator Quaternion(Silk.NET.Maths.Quaternion<float> value)
    {
        return new Quaternion(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator System.Numerics.Quaternion(Quaternion value)
    {
        return new System.Numerics.Quaternion(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator Quaternion(System.Numerics.Quaternion value)
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
