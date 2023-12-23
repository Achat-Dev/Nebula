using Silk.NET.Maths;

namespace Nebula;

public struct Vector2i : IEquatable<Vector2i>
{
    public int X = 0;
    public int Y = 0;

    public static readonly Vector2i Zero = new Vector2i(0, 0);
    public static readonly Vector2i One = new Vector2i(1, 1);
    public static readonly Vector2i Right = new Vector2i(1, 0);
    public static readonly Vector2i Up = new Vector2i(0, 1);

    public Vector2i(int x, int y)
    {
        X = x;
        Y = y;
    }

    /* -------------------- Static Methods -------------------- */

    public static Vector2i Abs(Vector2i value)
    {
        value.X = System.Math.Abs(value.X);
        value.Y = System.Math.Abs(value.Y);
        return value;
    }

    public static Vector2i Clamp(Vector2i value, Vector2i min, Vector2i max)
    {
        value.X = System.Math.Clamp(value.X, min.X, max.X);
        value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
        return value;
    }

    /* -------------------- Operators -------------------- */

    public static Vector2i operator +(Vector2i left, Vector2i right)
    {
        left.X += right.X;
        left.Y += right.Y;
        return left;
    }

    public static Vector2i operator -(Vector2i left, Vector2i right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        return left;
    }

    // Negation
    public static Vector2i operator -(Vector2i self)
    {
        self.X = -self.X;
        self.Y = -self.Y;
        return self;
    }

    public static Vector2i operator *(Vector2i value, int scale)
    {
        value.X *= scale;
        value.Y *= scale;
        return value;
    }

    public static Vector2i operator *(int scale, Vector2i value)
    {
        value.X *= scale;
        value.Y *= scale;
        return value;
    }

    public static Vector2i operator *(Vector2i value, Vector2i scale)
    {
        value.X *= scale.X;
        value.Y *= scale.Y;
        return value;
    }

    public static Vector2i operator /(Vector2i vector, int scale)
    {
        vector.X /= scale;
        vector.Y /= scale;
        return vector;
    }

    public static Vector2i operator /(Vector2i value, Vector2i scale)
    {
        value.X /= scale.X;
        value.Y /= scale.Y;
        return value;
    }

    public static bool operator ==(Vector2i left, Vector2i right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Vector2i left, Vector2i right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Vector2D<int>(Vector2i value)
    {
        return new Vector2D<int>(value.X, value.Y);
    }

    public static implicit operator Vector2i(Vector2D<int> value)
    {
        return new Vector2i(value.X, value.Y);
    }

    public static explicit operator Vector2(Vector2i value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static explicit operator Vector3i(Vector2i value)
    {
        return new Vector3i(value.X, value.Y, 0);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Vector2i other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2i other)
        {
            Equals(other);
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
