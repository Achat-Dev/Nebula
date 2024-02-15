using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal struct BufferElement
{
    public uint Offset;
    private readonly Shader.DataType r_shaderDataType;

    public static readonly BufferElement Float = new BufferElement(Shader.DataType.Float);
    public static readonly BufferElement Vec2 = new BufferElement(Shader.DataType.Vec2);
    public static readonly BufferElement Vec3 = new BufferElement(Shader.DataType.Vec3);
    public static readonly BufferElement Vec4 = new BufferElement(Shader.DataType.Vec4);
    public static readonly BufferElement Int = new BufferElement(Shader.DataType.Int);
    public static readonly BufferElement IVec2 = new BufferElement(Shader.DataType.IVec2);
    public static readonly BufferElement IVec3 = new BufferElement(Shader.DataType.IVec3);
    public static readonly BufferElement IVec4 = new BufferElement(Shader.DataType.IVec4);
    public static readonly BufferElement Mat3 = new BufferElement(Shader.DataType.Mat3);
    public static readonly BufferElement Mat4 = new BufferElement(Shader.DataType.Mat4);
    public static readonly BufferElement Bool = new BufferElement(Shader.DataType.Bool);

    private BufferElement(Shader.DataType shaderDataType)
    {
        r_shaderDataType = shaderDataType;
    }

    public uint GetByteSize()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return 4;
            case Shader.DataType.Vec2: return 8;
            case Shader.DataType.Vec3: return 12;
            case Shader.DataType.Vec4: return 16;
            case Shader.DataType.Int: return 4;
            case Shader.DataType.IVec2: return 8;
            case Shader.DataType.IVec3: return 12;
            case Shader.DataType.IVec4: return 16;
            case Shader.DataType.Mat3: return 36;
            case Shader.DataType.Mat4: return 64;
            case Shader.DataType.Bool: return 1;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }

    public int GetCount()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return 1;
            case Shader.DataType.Vec2: return 2;
            case Shader.DataType.Vec3: return 3;
            case Shader.DataType.Vec4: return 4;
            case Shader.DataType.Int: return 1;
            case Shader.DataType.IVec2: return 2;
            case Shader.DataType.IVec3: return 3;
            case Shader.DataType.IVec4: return 4;
            case Shader.DataType.Mat3: return 9;
            case Shader.DataType.Mat4: return 16;
            case Shader.DataType.Bool: return 1;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }

    public VertexAttribPointerType GetGLType()
    {
        switch (r_shaderDataType)
        {
            case Shader.DataType.Float: return VertexAttribPointerType.Float;
            case Shader.DataType.Vec2: return VertexAttribPointerType.Float;
            case Shader.DataType.Vec3: return VertexAttribPointerType.Float;
            case Shader.DataType.Vec4: return VertexAttribPointerType.Float;
            case Shader.DataType.Int: return VertexAttribPointerType.Int;
            case Shader.DataType.IVec2: return VertexAttribPointerType.Int;
            case Shader.DataType.IVec3: return VertexAttribPointerType.Int;
            case Shader.DataType.IVec4: return VertexAttribPointerType.Int;
            case Shader.DataType.Mat3: return VertexAttribPointerType.Float;
            case Shader.DataType.Mat4: return VertexAttribPointerType.Float;
            case Shader.DataType.Bool: return VertexAttribPointerType.Byte;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }
}
