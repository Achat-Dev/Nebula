namespace Nebula;

public struct Vector4 : IEquatable<Vector4>
{
    public float X;
    public float Y;
    public float Z;
    public float W;

    public static readonly Vector4 Zero = new Vector4(0f, 0f, 0f, 0f);
    public static readonly Vector4 One = new Vector4(1f, 1f, 1f, 1f);

    public Vector4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /* -------------------- Methods -------------------- */

    public float Magnitude()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
    }

    public float SqrMagnitude()
    {
        return X * X + Y * Y + Z * Z + W * W;
    }

    public void Normalise()
    {
        float coefficient = 1f / Magnitude();
        X *= coefficient;
        Y *= coefficient;
        Z *= coefficient;
        W *= coefficient;
    }

    public Vector4 Normalised()
    {
        Vector4 result = this;
        result.Normalise();
        return result;
    }

    /* -------------------- Static Methods -------------------- */

    public static Vector4 Abs(Vector4 value)
    {
        value.X = MathF.Abs(value.X);
        value.Y = MathF.Abs(value.Y);
        value.Z = MathF.Abs(value.Z);
        value.W = MathF.Abs(value.W);
        return value;
    }

    public static Vector4 Clamp(Vector4 value, Vector4 min, Vector4 max)
    {
        value.X = System.Math.Clamp(value.X, min.X, max.X);
        value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
        value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
        value.W = System.Math.Clamp(value.W, min.W, max.W);
        return value;
    }

    public static float Distance(Vector4 a, Vector4 b)
    {
        a = b - a;
        return MathF.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z + a.W * a.W);
    }

    public static float SqrDistance(Vector4 a, Vector4 b)
    {
        a = b - a;
        return a.X * a.X + a.Y * a.Y + a.Z * a.Z + a.W * a.W;
    }

    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
    {
        a.X = a.X + (b.X - a.X) * t;
        a.Y = a.Y + (b.Y - a.Y) * t;
        a.Z = a.Z + (b.Z - a.Z) * t;
        a.W = a.W + (b.W - a.W) * t;
        return a;
    }

    /* -------------------- Operators -------------------- */

    public static Vector4 operator +(Vector4 left, Vector4 right)
    {
        left.X += right.X;
        left.Y += right.Y;
        left.Z += right.Z;
        left.W += right.W;
        return left;
    }

    public static Vector4 operator -(Vector4 left, Vector4 right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        left.Z -= right.Z;
        left.W -= right.W;
        return left;
    }

    // Negation
    public static Vector4 operator -(Vector4 self)
    {
        self.X = -self.X;
        self.Y = -self.Y;
        self.Z = -self.Z;
        self.W = -self.W;
        return self;
    }

    public static Vector4 operator *(Vector4 value, float scale)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        value.W *= scale;
        return value;
    }

    public static Vector4 operator *(float scale, Vector4 value)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        value.W *= scale;
        return value;
    }

    public static Vector4 operator *(Vector4 value, Vector4 scale)
    {
        value.X *= scale.X;
        value.Y *= scale.Y;
        value.Z *= scale.Z;
        value.W *= scale.W;
        return value;
    }

    public static Vector4 operator /(Vector4 vector, float scale)
    {
        vector.X /= scale;
        vector.Y /= scale;
        vector.Z /= scale;
        vector.W /= scale;
        return vector;
    }

    public static Vector4 operator /(Vector4 value, Vector4 scale)
    {
        value.X /= scale.X;
        value.Y /= scale.Y;
        value.Z /= scale.Z;
        value.W /= scale.W;
        return value;
    }

    public static bool operator ==(Vector4 left, Vector4 right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
    }

    public static bool operator !=(Vector4 left, Vector4 right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Silk.NET.Maths.Vector4D<float>(Vector4 value)
    {
        return new Silk.NET.Maths.Vector4D<float>(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator Vector4(Silk.NET.Maths.Vector4D<float> value)
    {
        return new Vector4(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator System.Numerics.Vector4(Vector4 value)
    {
        return new System.Numerics.Vector4(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator Vector4(System.Numerics.Vector4 value)
    {
        return new Vector4(value.X, value.Y, value.Z, value.W);
    }

    public static explicit operator Vector2(Vector4 value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static explicit operator Vector3(Vector4 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    public static explicit operator Vector2i(Vector4 value)
    {
        return new Vector2i((int)value.X, (int)value.Y);
    }

    public static explicit operator Vector3i(Vector4 value)
    {
        return new Vector3i((int)value.X, (int)value.Y, (int)value.Z);
    }

    public static explicit operator Vector4i(Vector4 value)
    {
        return new Vector4i((int)value.X, (int)value.Y, (int)value.Z, (int)value.W);
    }

    public static explicit operator Colour(Vector4 value)
    {
        return new Colour(value.X, value.Y, value.Z, value.W);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Vector4 other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector4 other)
        {
            Equals(other);
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
