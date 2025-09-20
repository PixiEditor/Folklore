namespace Folklore.Types;

public abstract class FolkloreType
{
    public string Name { get; set; }

    public FolkloreType(string name)
    {
        Name = name;
    }
}