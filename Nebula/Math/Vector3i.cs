using Silk.NET.Maths;

namespace Nebula;

public struct Vector3i : IEquatable<Vector3i>
{
    public int X = 0;
    public int Y = 0;
    public int Z = 0;

    public static readonly Vector3i Zero = new Vector3i(0, 0, 0);
    public static readonly Vector3i One = new Vector3i(1, 1, 1);
    public static readonly Vector3i Right = new Vector3i(1, 0, 0);
    public static readonly Vector3i Up = new Vector3i(0, 1, 0);
    public static readonly Vector3i Backward = new Vector3i(0, 0, 1);

    public Vector3i(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /* -------------------- Static Methods -------------------- */

    public static Vector3i Abs(Vector3i value)
    {
        value.X = System.Math.Abs(value.X);
        value.Y = System.Math.Abs(value.Y);
        value.Z = System.Math.Abs(value.Z);
        return value;
    }

    public static Vector3i Clamp(Vector3i value, Vector3i min, Vector3i max)
    {
        value.X = System.Math.Clamp(value.X, min.X, max.X);
        value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
        value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
        return value;
    }

    /* -------------------- Operators -------------------- */

    public static Vector3i operator +(Vector3i left, Vector3i right)
    {
        left.X += right.X;
        left.Y += right.Y;
        left.Z += right.Z;
        return left;
    }

    public static Vector3i operator -(Vector3i left, Vector3i right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        left.Z -= right.Z;
        return left;
    }

    // Negation
    public static Vector3i operator -(Vector3i self)
    {
        self.X = -self.X;
        self.Y = -self.Y;
        self.Z = -self.Z;
        return self;
    }

    public static Vector3i operator *(Vector3i value, int scale)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        return value;
    }

    public static Vector3i operator *(int scale, Vector3i value)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        return value;
    }

    public static Vector3i operator *(Vector3i value, Vector3i scale)
    {
        value.X *= scale.X;
        value.Y *= scale.Y;
        value.Z *= scale.Z;
        return value;
    }

    public static Vector3i operator /(Vector3i vector, int scale)
    {
        vector.X /= scale;
        vector.Y /= scale;
        vector.Z /= scale;
        return vector;
    }

    public static Vector3i operator /(Vector3i value, Vector3i scale)
    {
        value.X /= scale.X;
        value.Y /= scale.Y;
        value.Z /= scale.Z;
        return value;
    }

    public static bool operator ==(Vector3i left, Vector3i right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
    }

    public static bool operator !=(Vector3i left, Vector3i right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Vector3D<int>(Vector3i value)
    {
        return new Vector3D<int>(value.X, value.Y, value.Z);
    }

    public static implicit operator Vector3i(Vector3D<int> value)
    {
        return new Vector3i(value.X, value.Y, value.Z);
    }

    public static explicit operator Vector3(Vector3i value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    public static explicit operator Vector2i(Vector3i value)
    {
        return new Vector2i(value.X, value.Y);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Vector3i other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector3 other)
        {
            Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override string ToString()
    {
        return $"({X} | {Y} | {Z})";
    }
}
