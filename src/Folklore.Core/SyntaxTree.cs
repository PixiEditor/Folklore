using Folklore.Syntax;

namespace Folklore;

public class SyntaxTree
{
    public Root Root { get; private set; }
    public SyntaxTree(Root root)
    {
        Root = root;
    }

    public void Traverse(Action<SyntaxNode?, SyntaxNode> action)
    {
        if (Root == null)
        {
            return;
        }

        TraverseNode(null, Root, action);
    }

    private void TraverseNode(SyntaxNode? previous, SyntaxNode node, Action<SyntaxNode?, SyntaxNode> action)
    {
        action(previous, node);
        foreach (var child in node.Children)
        {
            TraverseNode(node, child, action);
        }
    }
}