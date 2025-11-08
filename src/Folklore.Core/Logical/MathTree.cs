namespace Folklore.Logical;

public class MathTree
{
    public MathTreeNode Root { get; set; }

    public MathTree(MathTreeNode root)
    {
        Root = root;
    }

    public void Traverse(Func<MathTreeNode, bool> func)
    {
        if (Root == null)
        {
            return;
        }

        TraverseNode(Root, func);
    }

    private void TraverseNode(MathTreeNode node, Func<MathTreeNode, bool> func)
    {
        if (node == null || !func(node))
        {
            return;
        }

        TraverseNode(node.Left, func);
        TraverseNode(node.Right, func);
    }
}

public class MathTreeNode
{
    public MathTreeNode? Parent { get; set; }
    public MathTreeNode? Left { get; set; }
    public MathTreeNode? Right { get; set; }

    public bool ChildrenFull => Left != null && Right != null;

    public Operand? OperandValue { get; set; }
    public Operator? OperatorValue { get; set; }

    public bool IsEmpty => OperandValue == null && OperatorValue == null;

    public MathTreeNode()
    {
    }

    public MathTreeNode(Operand operand)
    {
        OperandValue = operand;
    }

    public void AddChild(MathTreeNode child)
    {
        child.Parent = this;
        if (Left == null)
        {
            Left = child;
        }
        else if (Right == null)
        {
            Right = child;
        }
        else
        {
            throw new InvalidOperationException("Cannot add more than two children to a node.");
        }
    }
}