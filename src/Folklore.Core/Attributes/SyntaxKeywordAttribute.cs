namespace Folklore.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SyntaxKeywordAttribute : Attribute
{
    public string Keyword { get; }
    public Type NodeType { get; set; }

    public SyntaxKeywordAttribute(string keyword, Type nodeType)
    {
        Keyword = keyword;
        NodeType = nodeType;
    }
}