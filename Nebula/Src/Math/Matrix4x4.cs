using Silk.NET.Assimp;

namespace Nebula;

public struct Matrix4x4 : IEquatable<Matrix4x4>
{
    public float M11;
    public float M12;
    public float M13;
    public float M14;

    public float M21;
    public float M22;
    public float M23;
    public float M24;

    public float M31;
    public float M32;
    public float M33;
    public float M34;

    public float M41;
    public float M42;
    public float M43;
    public float M44;

    public static readonly Matrix4x4 Identity = new Matrix4x4(1f, 0f, 0f, 0f,
        0f, 1f, 0f, 0f,
        0f, 0f, 1f, 0f,
        0f, 0f, 0f, 1f);

    public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;

        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;

        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;

        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;
    }

    /* -------------------- Methods -------------------- */

    public float GetDeterminant()
    {
        float value1 = M33 * M44 - M34 * M43;
        float value2 = M32 * M44 - M34 * M42;
        float value3 = M32 * M43 - M33 * M42;
        float value4 = M31 * M44 - M34 * M41;
        float value5 = M31 * M43 - M33 * M41;
        float value6 = M31 * M42 - M32 * M41;

        float a1 = M22 * value1 - M23 * value2 + M24 * value3;
        float a2 = M21 * value1 - M23 * value4 + M24 * value5;
        float b1 = M21 * value2 - M22 * value4 + M24 * value6;
        float b2 = M21 * value3 - M22 * value5 + M23 * value6;

        float c1 = M11 * a1 - M12 * a2;
        float c2 = M13 * b1 - M14 * b2;

        return c1 + c2;
    }

    // Implementation taken from the Microsoft .Net Framework source refercene
    // | https://raw.githubusercontent.com/microsoft/referencesource/master/System.Numerics/System/Numerics/Matrix4x4.cs
    //
    // Possible optimisation:
    // | Since this is a few years old there may be a more efficient way to compute this
    // | -> .Net 8 System.Numerics.Matrix4x4.Invert returns slightly different results
    public void Invert()
    {
        float m11 = M11;
        float m12 = M12;
        float m13 = M13;
        float m14 = M14;

        float m21 = M21;
        float m22 = M22;
        float m23 = M23;
        float m24 = M24;

        float m31 = M31;
        float m32 = M32;
        float m33 = M33;
        float m34 = M34;

        float m41 = M41;
        float m42 = M42;
        float m43 = M43;
        float m44 = M44;

        // Calculate determinant
        float value1 = m33 * m44 - m34 * m43;
        float value2 = m32 * m44 - m34 * m42;
        float value3 = m32 * m43 - m33 * m42;
        float value4 = m31 * m44 - m34 * m41;
        float value5 = m31 * m43 - m33 * m41;
        float value6 = m31 * m42 - m32 * m41;

        float a1 = m22 * value1 - m23 * value2 + m24 * value3;
        float a2 = m21 * value1 - m23 * value4 + m24 * value5;
        float b1 = m21 * value2 - m22 * value4 + m24 * value6;
        float b2 = m21 * value3 - m22 * value5 + m23 * value6;

        float c1 = m11 * a1 - m12 * a2;
        float c2 = m13 * b1 - m14 * b2;

        float determinant = c1 + c2;

        if (Math.Abs(determinant) < float.Epsilon)
        {
            Logger.EngineError("Matrix is not invertible");
            return;
        }

        float inverseDeterminant = 1.0f / determinant;

        M11 = a1 * inverseDeterminant;
        M21 = -a2 * inverseDeterminant;
        M31 = b1 * inverseDeterminant;
        M41 = -b2 * inverseDeterminant;

        M12 = -(m12 * value1 - m13 * value2 + m14 * value3) * inverseDeterminant;
        M22 = (m11 * value1 - m13 * value4 + m14 * value5) * inverseDeterminant;
        M32 = -(m11 * value2 - m12 * value4 + m14 * value6) * inverseDeterminant;
        M42 = (m11 * value3 - m12 * value5 + m13 * value6) * inverseDeterminant;

        value1 = m23 * m44 - m24 * m43;
        value2 = m22 * m44 - m24 * m42;
        value3 = m22 * m43 - m23 * m42;
        value4 = m21 * m44 - m24 * m41;
        value5 = m21 * m43 - m23 * m41;
        value6 = m21 * m42 - m22 * m41;

        M13 = (m12 * value1 - m13 * value2 + m14 * value3) * inverseDeterminant;
        M23 = -(m11 * value1 - m13 * value4 + m14 * value5) * inverseDeterminant;
        M33 = (m11 * value2 - m12 * value4 + m14 * value6) * inverseDeterminant;
        M43 = -(m11 * value3 - m12 * value5 + m13 * value6) * inverseDeterminant;

        value1 = m23 * m34 - m24 * m33;
        value2 = m22 * m34 - m24 * m32;
        value3 = m22 * m33 - m23 * m32;
        value4 = m21 * m34 - m24 * m31;
        value5 = m21 * m33 - m23 * m31;
        value6 = m21 * m32 - m22 * m31;

        M14 = -(m12 * value1 - m13 * value2 + m14 * value3) * inverseDeterminant;
        M24 = (m11 * value1 - m13 * value4 + m14 * value5) * inverseDeterminant;
        M34 = -(m11 * value2 - m12 * value4 + m14 * value6) * inverseDeterminant;
        M44 = (m11 * value3 - m12 * value5 + m13 * value6) * inverseDeterminant;
    }

    public Matrix4x4 Inverted()
    {
        Matrix4x4 result = this;
        result.Invert();
        return result;
    }

    public void Transpose()
    {
        Matrix4x4 backup = this;
        M12 = backup.M21;
        M21 = backup.M12;

        M13 = backup.M31;
        M31 = backup.M13;

        M14 = backup.M41;
        M41 = backup.M14;

        M23 = backup.M32;
        M32 = backup.M23;

        M24 = backup.M42;
        M42 = backup.M24;

        M34 = backup.M43;
        M43 = backup.M34;
    }

    public Matrix4x4 Transposed()
    {
        Matrix4x4 result = this;
        result.M12 = M21;
        result.M21 = M12;

        result.M13 = M31;
        result.M31 = M13;

        result.M14 = M41;
        result.M41 = M14;

        result.M23 = M32;
        result.M32 = M23;

        result.M24 = M42;
        result.M42 = M24;

        result.M34 = M43;
        result.M43 = M34;

        return result;
    }

    /* -------------------- Static Methods -------------------- */

    public static Matrix4x4 CreateTranslation(Vector3 position)
    {
        return new Matrix4x4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
            position.X, position.Y, position.Z, 1f);
    }

    public static Matrix4x4 CreateScale(Vector3 scale)
    {
        return new Matrix4x4(
            scale.X, 0f, 0f, 0f,
            0f, scale.Y, 0f, 0f,
            0f, 0f, scale.Z, 0f,
            0f, 0f, 0f, 1f);
    }

    public static Matrix4x4 CreateRotation(Quaternion rotation)
    {
        Matrix4x4 result = Identity;

        float xx = rotation.X * rotation.X;
        float yy = rotation.Y * rotation.Y;
        float zz = rotation.Z * rotation.Z;

        float xy = rotation.X * rotation.Y;
        float wz = rotation.Z * rotation.W;
        float xz = rotation.Z * rotation.X;
        float wy = rotation.Y * rotation.W;
        float yz = rotation.Y * rotation.Z;
        float wx = rotation.X * rotation.W;

        result.M11 = 1f - 2f * (yy + zz);
        result.M12 = 2f * (xy + wz);
        result.M13 = 2f * (xz - wy);
        result.M21 = 2f * (xy - wz);
        result.M22 = 1f - 2f * (zz + xx);
        result.M23 = 2f * (yz + wx);
        result.M31 = 2f * (xz + wy);
        result.M32 = 2f * (yz - wx);
        result.M33 = 1f - 2f * (yy + xx);

        return result;
    }

    public static Matrix4x4 CreateLookAt(Vector3 position, Vector3 target, Vector3 up)
    {
        return System.Numerics.Matrix4x4.CreateLookAt(position, target, up);
    }

    public static Matrix4x4 CreateLookAtLeftHanded(Vector3 position, Vector3 target, Vector3 up)
    {
        return System.Numerics.Matrix4x4.CreateLookAtLeftHanded(position, target, up);
    }

    public static Matrix4x4 CreatePerspectiveFieldOfView(float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane)
    {
        fov = MathHelper.DegreesToRadians(fov);
        return System.Numerics.Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClippingPlane, farClippingPlane);
    }

    public static Matrix4x4 CreatePerspectiveFieldOfViewLeftHanded(float fov, float aspectRatio, float nearClippingPlane, float farClippingPlane)
    {
        fov = MathHelper.DegreesToRadians(fov);
        return System.Numerics.Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(fov, aspectRatio, nearClippingPlane, farClippingPlane);
    }

    public static Matrix4x4 CreateOrthographicFieldOfView(float width, float height, float nearClippingPlane, float farClippingPlane)
    {
        return System.Numerics.Matrix4x4.CreateOrthographic(width, height, nearClippingPlane, farClippingPlane);
    }

    public static Matrix4x4 CreateOrthographicFieldOfViewLeftHanded(float width, float height, float nearClippingPlane, float farClippingPlane)
    {
        return System.Numerics.Matrix4x4.CreateOrthographicLeftHanded(width, height, nearClippingPlane, farClippingPlane);
    }

    /* -------------------- Operators -------------------- */

    public static Matrix4x4 operator +(Matrix4x4 left, Matrix4x4 right)
    {
        left.M11 += right.M11;
        left.M12 += right.M12;
        left.M13 += right.M13;
        left.M14 += right.M14;

        left.M21 += right.M21;
        left.M22 += right.M22;
        left.M23 += right.M23;
        left.M24 += right.M24;

        left.M31 += right.M31;
        left.M32 += right.M32;
        left.M33 += right.M33;
        left.M34 += right.M34;

        left.M41 += right.M41;
        left.M42 += right.M42;
        left.M43 += right.M43;
        left.M44 += right.M44;
        return left;
    }

    public static Matrix4x4 operator -(Matrix4x4 left, Matrix4x4 right)
    {
        left.M11 -= right.M11;
        left.M12 -= right.M12;
        left.M13 -= right.M13;
        left.M14 -= right.M14;

        left.M21 -= right.M21;
        left.M22 -= right.M22;
        left.M23 -= right.M23;
        left.M24 -= right.M24;

        left.M31 -= right.M31;
        left.M32 -= right.M32;
        left.M33 -= right.M33;
        left.M34 -= right.M34;

        left.M41 -= right.M41;
        left.M42 -= right.M42;
        left.M43 -= right.M43;
        left.M44 -= right.M44;
        return left;
    }

    // Negation
    public static Matrix4x4 operator -(Matrix4x4 self)
    {
        self.M11 = -self.M11;
        self.M12 = -self.M12;
        self.M13 = -self.M13;
        self.M14 = -self.M14;

        self.M21 = -self.M21;
        self.M22 = -self.M22;
        self.M23 = -self.M23;
        self.M24 = -self.M24;

        self.M31 = -self.M31;
        self.M32 = -self.M32;
        self.M33 = -self.M33;
        self.M34 = -self.M34;

        self.M41 = -self.M41;
        self.M42 = -self.M42;
        self.M43 = -self.M43;
        self.M44 = -self.M44;
        return self;
    }

    public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
    {
        float m11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
        float m12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
        float m13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
        float m14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

        float m21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
        float m22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
        float m23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
        float m24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

        float m31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
        float m32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
        float m33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
        float m34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

        float m41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
        float m42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
        float m43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
        float m44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

        return new Matrix4x4(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
    }

    public static Matrix4x4 operator *(Matrix4x4 value, float scale)
    {
        value.M11 *= scale;
        value.M12 *= scale;
        value.M13 *= scale;
        value.M14 *= scale;

        value.M21 *= scale;
        value.M22 *= scale;
        value.M23 *= scale;
        value.M24 *= scale;

        value.M31 *= scale;
        value.M32 *= scale;
        value.M33 *= scale;
        value.M34 *= scale;

        value.M41 *= scale;
        value.M42 *= scale;
        value.M43 *= scale;
        value.M44 *= scale;

        return value;
    }

    public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
    {
        return left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14
            && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24
            && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34
            && left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 && left.M44 == right.M44;
    }

    public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
    {
        return left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14
            || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24
            || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34
            || left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 || left.M44 != right.M44;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator Matrix4x4(System.Numerics.Matrix4x4 value)
    {
        return new Matrix4x4(value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44);
    }

    public static implicit operator System.Numerics.Matrix4x4(Matrix4x4 value)
    {
        return new System.Numerics.Matrix4x4(value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44);
    }

    public static explicit operator Matrix3x3(Matrix4x4 value)
    {
        return new Matrix3x3(value.M11, value.M12, value.M13,
            value.M21, value.M22, value.M23,
            value.M31, value.M32, value.M33);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Matrix4x4 other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Matrix4x4 other)
        {
            Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        HashCode hashCode = default;

        hashCode.Add(M11);
        hashCode.Add(M12);
        hashCode.Add(M13);
        hashCode.Add(M14);

        hashCode.Add(M21);
        hashCode.Add(M22);
        hashCode.Add(M23);
        hashCode.Add(M24);

        hashCode.Add(M31);
        hashCode.Add(M32);
        hashCode.Add(M33);
        hashCode.Add(M34);

        hashCode.Add(M41);
        hashCode.Add(M42);
        hashCode.Add(M43);
        hashCode.Add(M44);

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        return $"({M11} | {M12} | {M13} | {M14})\n({M21} | {M22} | {M23} | {M24})\n({M31} | {M32} | {M33} | {M34})\n({M41} | {M42} | {M43} | {M44})";
    }
}
