using Folklore.Types;

namespace Folklore.Logical;

public class Reference
{
    public string Name { get; }
    public FolkloreType? Type { get; set; }

    public Reference(string name, FolkloreType? type)
    {
        Type = type;
        Name = name;
    }
}