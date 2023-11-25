namespace Nebula;

public struct Colour
{
    public float R = 0f;
    public float G = 0f;
    public float B = 0f;
    public float A = 1f;

    public static readonly Colour Red = new Colour(1, 0, 0);
    public static readonly Colour Green = new Colour(0, 1, 0);
    public static readonly Colour Blue = new Colour(0, 0, 1);

    public Colour(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Colour(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /* -------------------- Conversions -------------------- */

    public static implicit operator System.Numerics.Vector4(Colour colour)
    {
        return new System.Numerics.Vector4(colour.R, colour.G, colour.B, colour.A);
    }

    public static implicit operator Colour(System.Numerics.Vector4 colour)
    {
        return new Colour(colour.X, colour.Y, colour.Z, colour.W);
    }
}
