namespace Nebula.Rendering;

internal class BufferLayout
{
    private readonly uint r_stride;
    private readonly BufferElement[] r_elements;

    public BufferLayout(params BufferElement[] elements)
    {
        r_elements = elements;
        r_stride = 0;
        for (int i = 0; i < r_elements.Length; i++)
        {
            r_elements[i].Offset = r_stride;
            r_stride += r_elements[i].GetByteSize();
        }
    }

    public uint GetStride()
    {
        return r_stride;
    }

    public BufferElement[] GetElements()
    {
        return r_elements;
    }
}
