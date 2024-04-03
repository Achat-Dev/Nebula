namespace Nebula;

public struct Vector4i : IEquatable<Vector4i>
{
    public int X;
    public int Y;
    public int Z;
    public int W;

    public static readonly Vector4i Zero = new Vector4i(0, 0, 0, 0);
    public static readonly Vector4i One = new Vector4i(1, 1, 1, 1);

    public Vector4i(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /* -------------------- Static Methods -------------------- */

    public static Vector4i Abs(Vector4i value)
    {
        value.X = System.Math.Abs(value.X);
        value.Y = System.Math.Abs(value.Y);
        value.Z = System.Math.Abs(value.Z);
        value.W = System.Math.Abs(value.W);
        return value;
    }

    public static Vector4i Clamp(Vector4i value, Vector4i min, Vector4i max)
    {
        value.X = System.Math.Clamp(value.X, min.X, max.X);
        value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
        value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
        value.W = System.Math.Clamp(value.W, min.W, max.W);
        return value;
    }

    /* -------------------- Operators -------------------- */

    public static Vector4i operator +(Vector4i left, Vector4i right)
    {
        left.X += right.X;
        left.Y += right.Y;
        left.Z += right.Z;
        left.W += right.W;
        return left;
    }

    public static Vector4i operator -(Vector4i left, Vector4i right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        left.Z -= right.Z;
        left.W -= right.W;
        return left;
    }

    // Negation
    public static Vector4i operator -(Vector4i self)
    {
        self.X = -self.X;
        self.Y = -self.Y;
        self.Z = -self.Z;
        self.W = -self.W;
        return self;
    }

    public static Vector4i operator *(Vector4i value, int scale)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        value.W *= scale;
        return value;
    }

    public static Vector4i operator *(int scale, Vector4i value)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        value.W *= scale;
        return value;
    }

    public static Vector4i operator *(Vector4i value, Vector4i scale)
    {
        value.X *= scale.X;
        value.Y *= scale.Y;
        value.Z *= scale.Z;
        value.W *= scale.W;
        return value;
    }

    public static Vector4i operator /(Vector4i vector, int scale)
    {
        vector.X /= scale;
        vector.Y /= scale;
        vector.Z /= scale;
        vector.W /= scale;
        return vector;
    }

    public static Vector4i operator /(Vector4i value, Vector4i scale)
    {
        value.X /= scale.X;
        value.Y /= scale.Y;
        value.Z /= scale.Z;
        value.W /= scale.W;
        return value;
    }

    public static bool operator >(Vector4i left, Vector4i right)
    {
        return left.X > right.X && left.Y > right.Y && left.Z > right.Z && left.W > right.W;
    }

    public static bool operator <(Vector4i left, Vector4i right)
    {
        return left.X < right.X && left.Y < right.Y && left.Z < right.Z && left.W < right.W;
    }

    public static bool operator >=(Vector4i left, Vector4i right)
    {
        return left.X >= right.X && left.Y >= right.Y && left.Z >= right.Z && left.W >= right.W;
    }

    public static bool operator <=(Vector4i left, Vector4i right)
    {
        return left.X <= right.X && left.Y <= right.Y && left.Z <= right.Z && left.W <= right.W;
    }

    public static bool operator ==(Vector4i left, Vector4i right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
    }

    public static bool operator !=(Vector4i left, Vector4i right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Silk.NET.Maths.Vector4D<int>(Vector4i value)
    {
        return new Silk.NET.Maths.Vector4D<int>(value.X, value.Y, value.Z, value.W);
    }

    public static implicit operator Vector4i(Silk.NET.Maths.Vector4D<int> value)
    {
        return new Vector4i(value.X, value.Y, value.Z, value.W);
    }

    public static explicit operator Vector2(Vector4i value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static explicit operator Vector3(Vector4i value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    public static explicit operator Vector4(Vector4i value)
    {
        return new Vector4(value.X, value.Y, value.Z, value.W);
    }

    public static explicit operator Vector2i(Vector4i value)
    {
        return new Vector2i(value.X, value.Y);
    }

    public static explicit operator Vector3i(Vector4i value)
    {
        return new Vector3i(value.X, value.Y, value.Z);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Vector4i other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector4i other)
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
