namespace Nebula;

public struct Vector2 : IEquatable<Vector2>
{
    public float X;
    public float Y;

    public static readonly Vector2 Zero = new Vector2(0f, 0f);
    public static readonly Vector2 One = new Vector2(1f, 1f);
    public static readonly Vector2 Right = new Vector2(1f, 0f);
    public static readonly Vector2 Up = new Vector2(0f, 1f);

    public static readonly Vector2 PositiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
    public static readonly Vector2 NegativeInfinity = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    /* -------------------- Methods -------------------- */

    public float Magnitude()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }

    public float SqrMagnitude()
    {
        return X * X + Y * Y;
    }

    public void Normalise()
    {
        float coefficient = 1f / Magnitude();
        X *= coefficient;
        Y *= coefficient;
    }

    public Vector2 Normalised()
    {
        Vector2 result = this;
        result.Normalise();
        return result;
    }

    /* -------------------- Static Methods -------------------- */

    public static Vector2 Abs(Vector2 value)
    {
        value.X = MathF.Abs(value.X);
        value.Y = MathF.Abs(value.Y);
        return value;
    }

    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
    {
        value.X = System.Math.Clamp(value.X, min.X, max.X);
        value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
        return value;
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static float Distance(Vector2 a, Vector2 b)
    {
        a = b - a;
        return MathF.Sqrt(a.X * a.X + a.Y * a.Y);
    }

    public static float SqrDistance(Vector2 a, Vector2 b)
    {
        a = b - a;
        return a.X * a.X + a.Y * a.Y;
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        a.X = a.X + (b.X - a.X) * t;
        a.Y = a.Y + (b.Y - a.Y) * t;
        return a;
    }

    /* -------------------- Operators -------------------- */

    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
        left.X += right.X;
        left.Y += right.Y;
        return left;
    }

    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        return left;
    }

    // Negation
    public static Vector2 operator -(Vector2 self)
    {
        self.X = -self.X;
        self.Y = -self.Y;
        return self;
    }

    public static Vector2 operator *(Vector2 value, float scale)
    {
        value.X *= scale;
        value.Y *= scale;
        return value;
    }

    public static Vector2 operator *(float scale, Vector2 value)
    {
        value.X *= scale;
        value.Y *= scale;
        return value;
    }

    public static Vector2 operator *(Vector2 value, Vector2 scale)
    {
        value.X *= scale.X;
        value.Y *= scale.Y;
        return value;
    }

    public static Vector2 operator *(Quaternion quaternion, Vector2 value)
    {
        Quaternion result = quaternion * new Quaternion(value.X, value.Y, 0f, 0f);
        result = result * quaternion.Inverted();
        return new Vector2(result.X, result.Y);
    }

    public static Vector2 operator /(Vector2 vector, float scale)
    {
        vector.X /= scale;
        vector.Y /= scale;
        return vector;
    }

    public static Vector2 operator /(Vector2 value, Vector2 scale)
    {
        value.X /= scale.X;
        value.Y /= scale.Y;
        return value;
    }

    public static bool operator >(Vector2 left, Vector2 right)
    {
        return left.X > right.X && left.Y > right.Y;
    }

    public static bool operator <(Vector2 left, Vector2 right)
    {
        return left.X < right.X && left.Y < right.Y;
    }

    public static bool operator >=(Vector2 left, Vector2 right)
    {
        return left.X >= right.X && left.Y >= right.Y;
    }

    public static bool operator <=(Vector2 left, Vector2 right)
    {
        return left.X <= right.X && left.Y <= right.Y;
    }

    public static bool operator ==(Vector2 left, Vector2 right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Vector2 left, Vector2 right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Silk.NET.Maths.Vector2D<float>(Vector2 value)
    {
        return new Silk.NET.Maths.Vector2D<float>(value.X, value.Y);
    }

    public static implicit operator Vector2(Silk.NET.Maths.Vector2D<float> value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static implicit operator System.Numerics.Vector2(Vector2 value)
    {
        return new System.Numerics.Vector2(value.X, value.Y);
    }

    public static implicit operator Vector2(System.Numerics.Vector2 value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static explicit operator Vector3(Vector2 value)
    {
        return new Vector3(value.X, value.Y, 0f);
    }

    public static explicit operator Vector4(Vector2 value)
    {
        return new Vector4(value.X, value.Y, 0f, 0f);
    }

    public static explicit operator Vector2i(Vector2 value)
    {
        return new Vector2i((int)value.X, (int)value.Y);
    }

    public static explicit operator Vector3i(Vector2 value)
    {
        return new Vector3i((int)value.X, (int)value.Y, 0);
    }

    public static explicit operator Vector4i(Vector2 value)
    {
        return new Vector4i((int)value.X, (int)value.Y, 0, 0);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Vector2 other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2 other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"({X} | {Y})";
    }
}
