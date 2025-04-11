using MemoryPack;

namespace BlazorApp1.UserData.DataParts;

[MemoryPackable]
public partial class DataCounter
{
    public int Number { get; set; }
}
