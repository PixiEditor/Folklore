namespace Folklore.Types;

public abstract class FolkloreType
{
    public static readonly FolkloreType Number = new Primitive.NumberType();
    public static readonly FolkloreType Text = new Primitive.TextType();
    public static readonly FolkloreType Void = new VoidType();
    public string Name { get; set; }

    public FolkloreType(string name)
    {
        Name = name;
    }
}