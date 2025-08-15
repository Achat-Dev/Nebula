using System.IO.Hashing;
using System.Text;

namespace Nebula;

internal struct UUID
{
    public readonly ulong ID;

    public UUID()
    {
        ID = 0;
    }

    public UUID(ulong id)
    {
        ID = id;
    }

    public UUID(string path)
    {
        ID = XxHash64.HashToUInt64(Encoding.UTF8.GetBytes(path), 0);
    }
}
