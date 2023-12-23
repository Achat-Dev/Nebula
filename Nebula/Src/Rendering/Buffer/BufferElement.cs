using Silk.NET.OpenGL;

namespace Nebula.Rendering;

internal struct BufferElement
{
    public uint Offset;
    private readonly ShaderDataType r_shaderDataType;

    public static readonly BufferElement Float = new BufferElement(ShaderDataType.Float);
    public static readonly BufferElement Float2 = new BufferElement(ShaderDataType.Float2);
    public static readonly BufferElement Float3 = new BufferElement(ShaderDataType.Float3);
    public static readonly BufferElement Float4 = new BufferElement(ShaderDataType.Float4);
    public static readonly BufferElement Int = new BufferElement(ShaderDataType.Int);
    public static readonly BufferElement Int2 = new BufferElement(ShaderDataType.Int2);
    public static readonly BufferElement Int3 = new BufferElement(ShaderDataType.Int3);
    public static readonly BufferElement Int4 = new BufferElement(ShaderDataType.Int4);
    public static readonly BufferElement Mat3 = new BufferElement(ShaderDataType.Mat3);
    public static readonly BufferElement Mat4 = new BufferElement(ShaderDataType.Mat4);
    public static readonly BufferElement Bool = new BufferElement(ShaderDataType.Bool);

    private BufferElement(ShaderDataType dataType)
    {
        r_shaderDataType = dataType;
    }

    public uint GetByteSize()
    {
        switch (r_shaderDataType)
        {
            case ShaderDataType.Float: return 4;
            case ShaderDataType.Float2: return 8;
            case ShaderDataType.Float3: return 12;
            case ShaderDataType.Float4: return 16;
            case ShaderDataType.Int: return 4;
            case ShaderDataType.Int2: return 8;
            case ShaderDataType.Int3: return 12;
            case ShaderDataType.Int4: return 16;
            case ShaderDataType.Mat3: return 36;
            case ShaderDataType.Mat4: return 64;
            case ShaderDataType.Bool: return 1;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }

    public int GetCount()
    {
        switch (r_shaderDataType)
        {
            case ShaderDataType.Float: return 1;
            case ShaderDataType.Float2: return 2;
            case ShaderDataType.Float3: return 3;
            case ShaderDataType.Float4: return 4;
            case ShaderDataType.Int: return 1;
            case ShaderDataType.Int2: return 2;
            case ShaderDataType.Int3: return 3;
            case ShaderDataType.Int4: return 4;
            case ShaderDataType.Mat3: return 9;
            case ShaderDataType.Mat4: return 16;
            case ShaderDataType.Bool: return 1;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }

    public VertexAttribPointerType GetGLType()
    {
        switch (r_shaderDataType)
        {
            case ShaderDataType.Float: return VertexAttribPointerType.Float;
            case ShaderDataType.Float2: return VertexAttribPointerType.Float;
            case ShaderDataType.Float3: return VertexAttribPointerType.Float;
            case ShaderDataType.Float4: return VertexAttribPointerType.Float;
            case ShaderDataType.Int: return VertexAttribPointerType.Int;
            case ShaderDataType.Int2: return VertexAttribPointerType.Int;
            case ShaderDataType.Int3: return VertexAttribPointerType.Int;
            case ShaderDataType.Int4: return VertexAttribPointerType.Int;
            case ShaderDataType.Mat3: return VertexAttribPointerType.Float;
            case ShaderDataType.Mat4: return VertexAttribPointerType.Float;
            case ShaderDataType.Bool: return VertexAttribPointerType.Byte;
            default:
                Logger.EngineAssert(false, "Unknown shader data type");
                return 0;
        }
    }
}
