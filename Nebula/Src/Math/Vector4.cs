namespace Nebula;

public struct Vector4 : IEquatable<Vector4>
{
    public float X = 0f;
    public float Y = 0f;
    public float Z = 0f;
    public float W = 0f;

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

    public float SqrMagnitude()
    {
        return X * X + Y * Y + Z * Z + W * W;
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

    public static explicit operator Vector3(Vector4 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    public static explicit operator Colour(Vector4 colour)
    {
        return new Colour(colour.X, colour.Y, colour.Z, colour.W);
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
