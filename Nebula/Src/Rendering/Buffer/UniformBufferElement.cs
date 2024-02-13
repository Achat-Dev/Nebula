namespace Nebula.Rendering;

internal struct UniformBufferElement
{
    private readonly Shader.DataType r_shaderDataType;

    public static readonly UniformBufferElement Float = new UniformBufferElement(Shader.DataType.Float);
    public static readonly UniformBufferElement Float2 = new UniformBufferElement(Shader.DataType.Float2);
    public static readonly UniformBufferElement Float3 = new UniformBufferElement(Shader.DataType.Float3);
    public static readonly UniformBufferElement Float4 = new UniformBufferElement(Shader.DataType.Float4);
    public static readonly UniformBufferElement Int = new UniformBufferElement(Shader.DataType.Int);
    public static readonly UniformBufferElement Int2 = new UniformBufferElement(Shader.DataType.Int2);
    public static readonly UniformBufferElement Int3 = new UniformBufferElement(Shader.DataType.Int3);
    public static readonly UniformBufferElement Int4 = new UniformBufferElement(Shader.DataType.Int4);
    public static readonly UniformBufferElement Mat3 = new UniformBufferElement(Shader.DataType.Mat3);
    public static readonly UniformBufferElement Mat4 = new UniformBufferElement(Shader.DataType.Mat4);
    public static readonly UniformBufferElement Bool = new UniformBufferElement(Shader.DataType.Bool);

    private UniformBufferElement(Shader.DataType shaderDataType)
    {
        r_shaderDataType = shaderDataType;
    }

    public uint GetByteSize()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return 4;
            case Shader.DataType.Float2: return 8;
            case Shader.DataType.Float3: return 16;
            case Shader.DataType.Float4: return 16;
            case Shader.DataType.Int: return 4;
            case Shader.DataType.Int2: return 8;
            case Shader.DataType.Int3: return 16;
            case Shader.DataType.Int4: return 16;
            case Shader.DataType.Mat3: return 48;
            case Shader.DataType.Mat4: return 64;
            case Shader.DataType.Bool: return 4;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }

    public uint GetBaseOffset()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return 4;
            case Shader.DataType.Float2: return 8;
            case Shader.DataType.Float3: return 16;
            case Shader.DataType.Float4: return 16;
            case Shader.DataType.Int: return 4;
            case Shader.DataType.Int2: return 8;
            case Shader.DataType.Int3: return 16;
            case Shader.DataType.Int4: return 16;
            case Shader.DataType.Mat3: return 16;
            case Shader.DataType.Mat4: return 16;
            case Shader.DataType.Bool: return 4;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }
}
