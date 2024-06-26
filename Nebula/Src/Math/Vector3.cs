﻿namespace Nebula;

public struct Vector3 : IEquatable<Vector3>
{
    public float X;
    public float Y;
    public float Z;

    public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);
    public static readonly Vector3 One = new Vector3(1f, 1f, 1f);
    public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);
    public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);
    public static readonly Vector3 Forward = new Vector3(0f, 0f, 1f);

    public static readonly Vector3 PositiveInfinity = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    public static readonly Vector3 NegativeInfinity = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /* -------------------- Methods -------------------- */

    public float Magnitude()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z);
    }

    public float SqrMagnitude()
    {
        return X * X + Y * Y + Z * Z;
    }

    public void Normalise()
    {
        float coefficient = 1f / Magnitude();
        X *= coefficient;
        Y *= coefficient;
        Z *= coefficient;
    }

    public Vector3 Normalised()
    {
        Vector3 result = this;
        result.Normalise();
        return result;
    }

    /* -------------------- Static Methods -------------------- */

    public static Vector3 Abs(Vector3 value)
    {
        value.X = MathF.Abs(value.X);
        value.Y = MathF.Abs(value.Y);
        value.Z = MathF.Abs(value.Z);
        return value;
    }

    public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        value.X = System.Math.Clamp(value.X, min.X, max.X);
        value.Y = System.Math.Clamp(value.Y, min.Y, max.Y);
        value.Z = System.Math.Clamp(value.Z, min.Z, max.Z);
        return value;
    }

    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        Vector3 result;
        result.X = a.Y * b.Z - a.Z * b.Y;
        result.Y = a.Z * b.X - a.X * b.Z;
        result.Z = a.X * b.Y - a.Y * b.X;
        return result;
    }

    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        a = b - a;
        return MathF.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
    }

    public static float SqrDistance(Vector3 a, Vector3 b)
    {
        a = b - a;
        return a.X * a.X + a.Y * a.Y + a.Z * a.Z;
    }

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        a.X = a.X + (b.X - a.X) * t;
        a.Y = a.Y + (b.Y - a.Y) * t;
        a.Z = a.Z + (b.Z - a.Z) * t;
        return a;
    }

    /* -------------------- Operators -------------------- */

    public static Vector3 operator +(Vector3 left, Vector3 right)
    {
        left.X += right.X;
        left.Y += right.Y;
        left.Z += right.Z;
        return left;
    }

    public static Vector3 operator -(Vector3 left, Vector3 right)
    {
        left.X -= right.X;
        left.Y -= right.Y;
        left.Z -= right.Z;
        return left;
    }

    // Negation
    public static Vector3 operator -(Vector3 self)
    {
        self.X = -self.X;
        self.Y = -self.Y;
        self.Z = -self.Z;
        return self;
    }

    public static Vector3 operator *(Vector3 value, float scale)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        return value;
    }

    public static Vector3 operator *(float scale, Vector3 value)
    {
        value.X *= scale;
        value.Y *= scale;
        value.Z *= scale;
        return value;
    }

    public static Vector3 operator *(Vector3 value, Vector3 scale)
    {
        value.X *= scale.X;
        value.Y *= scale.Y;
        value.Z *= scale.Z;
        return value;
    }

    public static Vector3 operator *(Quaternion quaternion, Vector3 value)
    {
        Vector3 left = quaternion.GetXyz();
        Vector3 cross = Cross(left, value);
        cross += value * quaternion.W;
        cross = Cross(left, cross);
        cross *= 2f;
        value += cross;
        return value;
    }

    public static Vector3 operator *(Matrix3x3 transformation, Vector3 value)
    {
        Vector3 result;
        result.X = transformation.M11 * value.X + transformation.M12 * value.Y + transformation.M13 * value.Z;
        result.Y = transformation.M21 * value.X + transformation.M22 * value.Y + transformation.M23 * value.Z;
        result.Z = transformation.M31 * value.X + transformation.M32 * value.Y + transformation.M33 * value.Z;
        return result;
    }

    public static Vector3 operator /(Vector3 vector, float scale)
    {
        vector.X /= scale;
        vector.Y /= scale;
        vector.Z /= scale;
        return vector;
    }

    public static Vector3 operator /(Vector3 value, Vector3 scale)
    {
        value.X /= scale.X;
        value.Y /= scale.Y;
        value.Z /= scale.Z;
        return value;
    }

    public static Vector3 operator %(Vector3 left, Vector3 right)
    {
        return new Vector3(left.X % right.X, left.Y % right.Y, left.Z % right.Z);
    }

    public static bool operator >(Vector3 left, Vector3 right)
    {
        return left.X > right.X && left.Y > right.Y && left.Z > right.Z;
    }

    public static bool operator <(Vector3 left, Vector3 right)
    {
        return left.X < right.X && left.Y < right.Y && left.Z < right.Z;
    }

    public static bool operator >=(Vector3 left, Vector3 right)
    {
        return left.X >= right.X && left.Y >= right.Y && left.Z >= right.Z;
    }

    public static bool operator <=(Vector3 left, Vector3 right)
    {
        return left.X <= right.X && left.Y <= right.Y && left.Z <= right.Z;
    }

    public static bool operator ==(Vector3 left, Vector3 right)
    {
        return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
    }

    public static bool operator !=(Vector3 left, Vector3 right)
    {
        return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Silk.NET.Maths.Vector3D<float>(Vector3 value)
    {
        return new Silk.NET.Maths.Vector3D<float>(value.X, value.Y, value.Z);
    }

    public static implicit operator Vector3(Silk.NET.Maths.Vector3D<float> value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    public static implicit operator System.Numerics.Vector3(Vector3 value)
    {
        return new System.Numerics.Vector3(value.X, value.Y, value.Z);
    }

    public static implicit operator Vector3(System.Numerics.Vector3 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    public static explicit operator Vector2(Vector3 value)
    {
        return new Vector2(value.X, value.Y);
    }

    public static explicit operator Vector4(Vector3 value)
    {
        return new Vector4(value.X, value.Y, value.Z, 0f);
    }

    public static explicit operator Vector2i(Vector3 value)
    {
        return new Vector2i((int)value.X, (int)value.Y);
    }

    public static explicit operator Vector3i(Vector3 value)
    {
        return new Vector3i((int)value.X, (int)value.Y, (int)value.Z);
    }

    public static explicit operator Vector4i(Vector3 value)
    {
        return new Vector4i((int)value.X, (int)value.Y, (int)value.Z, 0);
    }

    public static explicit operator Colour(Vector3 value)
    {
        return new Colour(value.X, value.Y, value.Z);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Vector3 other)
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
