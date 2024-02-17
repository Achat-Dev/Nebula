namespace Nebula;

public struct Matrix3x3 : IEquatable<Matrix3x3>
{
    public float M11;
    public float M12;
    public float M13;

    public float M21;
    public float M22;
    public float M23;

    public float M31;
    public float M32;
    public float M33;

    public static Matrix3x3 Identity = new Matrix3x3(1f, 0f, 0f,
        0f, 1f, 0f,
        0f, 0f, 1f);

    public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;

        M21 = m21;
        M22 = m22;
        M23 = m23;

        M31 = m31;
        M32 = m32;
        M33 = m33;
    }

    /* -------------------- Methods -------------------- */

    public float GetDeterminant()
    {
        float a1 = M22 * M33 - M23 * M32;
        float a2 = M21 * M33 - M23 * M31;
        float b1 = M21 * M32 - M22 * M31;

        float c1 = M11 * a1 - M12 * a2;
        float c2 = M13 * b1;

        return c1 + c2;
    }

    public void Invert()
    {
        float m11 = M11;
        float m12 = M12;
        float m13 = M13;

        float m21 = M21;
        float m22 = M22;
        float m23 = M23;

        float m31 = M31;
        float m32 = M32;
        float m33 = M33;

        // Calculate determinant
        float a1 = m22 * m33 - m23 * m32;
        float a2 = m21 * m33 - m23 * m31;
        float b1 = m21 * m32 - m22 * m31;

        float c1 = m11 * a1 - m12 * a2;
        float c2 = m13 * b1;

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

        M12 = -(m12 * m33 - m13 * m32) * inverseDeterminant;
        M22 = (m11 * m33 - m13 * m31) * inverseDeterminant;
        M32 = -(m11 * m32 - m12 * m31) * inverseDeterminant;

        M13 = (m12 * m23 - m13 * m22) * inverseDeterminant;
        M23 = -(m11 * m23 - m13 * m21) * inverseDeterminant;
        M33 = (m11 * m22 - m12 * m21) * inverseDeterminant;
    }

    public Matrix3x3 Inverted()
    {
        Matrix3x3 result = this;
        result.Invert();
        return result;
    }

    public void Transpose()
    {
        Matrix3x3 backup = this;

        M12 = backup.M21;
        M21 = backup.M12;

        M13 = backup.M31;
        M31 = backup.M13;

        M23 = backup.M32;
        M32 = backup.M23;
    }

    public Matrix3x3 Transposed()
    {
        Matrix3x3 result = this;

        result.M12 = M21;
        result.M21 = M12;

        result.M13 = M31;
        result.M31 = M13;

        result.M23 = M32;
        result.M32 = M23;

        return result;
    }

    /* -------------------- Operators -------------------- */

    public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right)
    {
        left.M11 += right.M11;
        left.M12 += right.M12;
        left.M13 += right.M13;

        left.M21 += right.M21;
        left.M22 += right.M22;
        left.M23 += right.M23;

        left.M31 += right.M31;
        left.M32 += right.M32;
        left.M33 += right.M33;

        return left;
    }

    public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right)
    {
        left.M11 -= right.M11;
        left.M12 -= right.M12;
        left.M13 -= right.M13;

        left.M21 -= right.M21;
        left.M22 -= right.M22;
        left.M23 -= right.M23;

        left.M31 -= right.M31;
        left.M32 -= right.M32;
        left.M33 -= right.M33;

        return left;
    }

    // Negation
    public static Matrix3x3 operator -(Matrix3x3 self)
    {
        self.M11 = -self.M11;
        self.M12 = -self.M12;
        self.M13 = -self.M13;

        self.M21 = -self.M21;
        self.M22 = -self.M22;
        self.M23 = -self.M23;

        self.M31 = -self.M31;
        self.M32 = -self.M32;
        self.M33 = -self.M33;

        return self;
    }

    public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right)
    {
        float m11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31;
        float m12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32;
        float m13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33;

        float m21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31;
        float m22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32;
        float m23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33;

        float m31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31;
        float m32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32;
        float m33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33;

        return new Matrix3x3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
    }

    public static Matrix3x3 operator *(Matrix3x3 value, float scale)
    {
        value.M11 *= scale;
        value.M12 *= scale;
        value.M13 *= scale;

        value.M21 *= scale;
        value.M22 *= scale;
        value.M23 *= scale;

        value.M31 *= scale;
        value.M32 *= scale;
        value.M33 *= scale;

        return value;
    }

    public static bool operator ==(Matrix3x3 left, Matrix3x3 right)
    {
        return left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13
            && left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23
            && left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33;
    }

    public static bool operator !=(Matrix3x3 left, Matrix3x3 right)
    {
        return left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13
            || left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23
            || left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33;
    }

    /* -------------------- Conversions -------------------- */

    public static explicit operator Matrix4x4(Matrix3x3 value)
    {
        return new Matrix4x4(value.M11, value.M12, value.M13, 0f,
            value.M21, value.M22, value.M23, 0f,
            value.M31, value.M32, value.M33, 0f,
            0f, 0f, 0f, 1f);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Matrix3x3 other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Matrix3x3 other)
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

        hashCode.Add(M21);
        hashCode.Add(M22);
        hashCode.Add(M23);

        hashCode.Add(M31);
        hashCode.Add(M32);
        hashCode.Add(M33);

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        return $"({M11} | {M12} | {M13})\n({M21} | {M22} | {M23})\n({M31} | {M32} | {M33})";
    }
}
