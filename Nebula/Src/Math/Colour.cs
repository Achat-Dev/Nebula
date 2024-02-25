namespace Nebula;

public struct Colour : IEquatable<Colour>
{
    public float R;
    public float G;
    public float B;
    public float A;

    public static readonly Colour White = new Colour(1f, 1f, 1f);
    public static readonly Colour Black = new Colour(0f, 0f, 0f);
    public static readonly Colour Red = new Colour(1f, 0f, 0f);
    public static readonly Colour Green = new Colour(0f, 1f, 0f);
    public static readonly Colour Blue = new Colour(0f, 0f, 1f);
    public static readonly Colour Transparent = new Colour(1f, 1f, 1f, 0f);

    public Colour()
    {
        A = 1f;
    }

    public Colour(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
        A = 1f;
    }

    public Colour(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /* -------------------- Operators -------------------- */

    public static bool operator ==(Colour left, Colour right)
    {
        return left.R == right.R && left.G == right.G && left.B == right.B && left.A == right.A;
    }

    public static bool operator !=(Colour left, Colour right)
    {
        return left.R != right.R || left.G != right.G || left.B != right.B || left.A != right.A;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator System.Drawing.Color(Colour colour)
    {
        return System.Drawing.Color.FromArgb((int)(colour.A * 255), (int)(colour.R * 255), (int)(colour.G * 255), (int)(colour.B * 255));
    }

    public static implicit operator Colour(System.Drawing.Color colour)
    {
        return new Colour((float)colour.R / 255f, (float)colour.G / 255f, (float)colour.B / 255f, (float)colour.A / 255f);
    }

    public static explicit operator System.Numerics.Vector4(Colour colour)
    {
        return new System.Numerics.Vector4(colour.R, colour.G, colour.B, colour.A);
    }

    public static explicit operator Colour(System.Numerics.Vector4 colour)
    {
        return new Colour(colour.X, colour.Y, colour.Z, colour.W);
    }

    public static explicit operator Vector3(Colour colour)
    {
        return new Vector3(colour.R, colour.G, colour.B);
    }

    public static explicit operator Vector4(Colour colour)
    {
        return new Vector4(colour.R, colour.G, colour.B, colour.A);
    }

    /* -------------------- Overrides -------------------- */

    public bool Equals(Colour other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj is Colour other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    public override string ToString()
    {
        return $"({R} | {B} | {B} | {A})";
    }
}
