using PixiScript.Syntax;

namespace PixiScript;

public class SyntaxTree
{
    public Root Root { get; private set; }
    public SyntaxTree(Root root)
    {
        Root = root;
    }

    public void Traverse(Action<SyntaxNode> action)
    {
        if (Root == null)
        {
            return;
        }

        TraverseNode(Root, action);
    }

    private void TraverseNode(SyntaxNode node, Action<SyntaxNode> action)
    {
        action(node);
        foreach (var child in node.Children)
        {
            TraverseNode(child, action);
        }
    }
}