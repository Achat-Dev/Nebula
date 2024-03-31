namespace Nebula.Rendering;

public struct UniformBufferElement
{
    private readonly Shader.DataType r_shaderDataType;

    public static readonly UniformBufferElement Float = new UniformBufferElement(Shader.DataType.Float);
    public static readonly UniformBufferElement Vec2 = new UniformBufferElement(Shader.DataType.Vec2);
    public static readonly UniformBufferElement Vec3 = new UniformBufferElement(Shader.DataType.Vec3);
    public static readonly UniformBufferElement Vec4 = new UniformBufferElement(Shader.DataType.Vec4);
    public static readonly UniformBufferElement Int = new UniformBufferElement(Shader.DataType.Int);
    public static readonly UniformBufferElement IVec2 = new UniformBufferElement(Shader.DataType.IVec2);
    public static readonly UniformBufferElement IVec3 = new UniformBufferElement(Shader.DataType.IVec3);
    public static readonly UniformBufferElement IVec4 = new UniformBufferElement(Shader.DataType.IVec4);
    public static readonly UniformBufferElement Mat3 = new UniformBufferElement(Shader.DataType.Mat3);
    public static readonly UniformBufferElement Mat4 = new UniformBufferElement(Shader.DataType.Mat4);
    public static readonly UniformBufferElement Bool = new UniformBufferElement(Shader.DataType.Bool);

    private UniformBufferElement(Shader.DataType shaderDataType)
    {
        r_shaderDataType = shaderDataType;
    }

    internal uint GetByteSize()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return 4;
            case Shader.DataType.Vec2: return 8;
            case Shader.DataType.Vec3: return 16;
            case Shader.DataType.Vec4: return 16;
            case Shader.DataType.Int: return 4;
            case Shader.DataType.IVec2: return 8;
            case Shader.DataType.IVec3: return 16;
            case Shader.DataType.IVec4: return 16;
            case Shader.DataType.Mat3: return 48;
            case Shader.DataType.Mat4: return 64;
            case Shader.DataType.Bool: return 4;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }

    internal uint GetBaseOffset()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return 4;
            case Shader.DataType.Vec2: return 8;
            case Shader.DataType.Vec3: return 16;
            case Shader.DataType.Vec4: return 16;
            case Shader.DataType.Int: return 4;
            case Shader.DataType.IVec2: return 8;
            case Shader.DataType.IVec3: return 16;
            case Shader.DataType.IVec4: return 16;
            case Shader.DataType.Mat3: return 16;
            case Shader.DataType.Mat4: return 16;
            case Shader.DataType.Bool: return 4;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }
}
