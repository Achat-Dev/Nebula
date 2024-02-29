namespace Nebula.Rendering;

[Flags]
public enum VertexFlags
{
    Position    = 0b0001,
    Normal      = 0b0010,
    Tangent     = 0b0100,
    UV          = 0b1000,
}

internal static class VertexFlagsExtensions
{
    public static int GetElementCount(this VertexFlags vertexFlags)
    {
        int result = 0;
        if (vertexFlags.HasFlag(VertexFlags.Position)) result += 3;
        if (vertexFlags.HasFlag(VertexFlags.Normal)) result += 3;
        if (vertexFlags.HasFlag(VertexFlags.Tangent)) result += 3;
        if (vertexFlags.HasFlag(VertexFlags.UV)) result += 2;
        return result;
    }

    public static BufferLayout GenerateBufferLayout(this VertexFlags vertexFlags)
    {
        int bitCount = 0;
        VertexFlags v = vertexFlags;
        while (v != 0)
        {
            v = v & (v - 1);
            bitCount++;
        }

        Logger.EngineInfo(bitCount);

        int i = 0;
        BufferElement[] bufferElements = new BufferElement[bitCount];
        if (vertexFlags.HasFlag(VertexFlags.Position)) bufferElements[i++] = BufferElement.Vec3;
        if (vertexFlags.HasFlag(VertexFlags.Normal)) bufferElements[i++] = BufferElement.Vec3;
        if (vertexFlags.HasFlag(VertexFlags.Tangent)) bufferElements[i++] = BufferElement.Vec3;
        if (vertexFlags.HasFlag(VertexFlags.UV)) bufferElements[i++] = BufferElement.Vec2;

        return new BufferLayout(bufferElements);
    }
}
