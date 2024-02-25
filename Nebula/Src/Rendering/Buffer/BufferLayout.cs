namespace Nebula.Rendering;

internal class BufferLayout
{
    private readonly uint r_byteSize;
    private readonly BufferElement[] r_elements;

    public BufferLayout(params BufferElement[] elements)
    {
        r_elements = elements;
        r_byteSize = 0;
        for (int i = 0; i < r_elements.Length; i++)
        {
            r_elements[i].Offset = r_byteSize;
            r_byteSize += r_elements[i].GetByteSize();
        }
    }

    public uint GetByteSize()
    {
        return r_byteSize;
    }

    public BufferElement[] GetElements()
    {
        return r_elements;
    }
}
