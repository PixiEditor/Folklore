using Folklore.Types;

namespace Folklore.Syntax;

public abstract class ReturnValueSyntaxNode : SyntaxNode
{
    public abstract FolkloreType ReturnType { get; }
}