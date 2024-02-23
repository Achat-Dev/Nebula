namespace Nebula.Rendering;

internal struct UniformBufferLayout
{
    public readonly uint ByteSize;

    public UniformBufferLayout(params UniformBufferElement[] elements)
        : this(1, elements) { }

    public UniformBufferLayout(uint arraySize, params UniformBufferElement[] elements)
    {
        ByteSize = CalculateTotalByteSize(elements);

        // Structs are padded to a multiple of 16
        // (every layout is padded to 16 because the code is easier that way and those few extra bytes don't really matter)
        ByteSize += CalculatePadding(ByteSize, 16);
        ByteSize *= arraySize;
    }

    private uint CalculateTotalByteSize(UniformBufferElement[] elements)
    {
        uint byteSize = 0;
        uint elementSize = 0;
        uint elementOffset = 0;

        for (int i = 0; i < elements.Length; i++)
        {
            elementSize = elements[i].GetByteSize();
            elementOffset = elements[i].GetBaseOffset();
            byteSize += CalculatePadding(byteSize, elementOffset) + elementSize;
        }

        return byteSize;
    }

    private uint CalculatePadding(uint byteSize, uint offset)
    {
        uint padding = byteSize % offset;
        if (padding != 0)
        {
            padding = offset - padding;
        }
        return padding;
    }
}
