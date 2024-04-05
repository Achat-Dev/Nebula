namespace Nebula.Rendering;

public struct ShaderCompileReplacement
{
    public readonly string Name;
    public readonly object Value;

    public ShaderCompileReplacement(string name, object value)
    {
        Name = name;
        Value = value;
    }
}
