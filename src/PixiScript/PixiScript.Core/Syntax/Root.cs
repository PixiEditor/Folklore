namespace PixiScript.Syntax;

public class Root : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }

    public List<SyntaxNode> PotentialNodes { get; } = new List<SyntaxNode>();

    public Root()
    {
    }

    protected override void OnTokenAdded(Token token)
    {
        if (PotentialNodes.Count > 0)
        {
            bool foundValidNode = false;
            foreach (var node in PotentialNodes)
            {
                node.AddToken(token);
                if (node.IsValid())
                {
                    Children.Add(node);
                    foundValidNode = true;
                    break;
                }
            }

            if (foundValidNode)
            {
                PotentialNodes.Clear();
            }

            return;
        }

        // TODO: Variable type detection
        if (token is { Kind: TokenKind.Keyword })
        {
            if (IsBuiltInVarType(token.Text))
            {
                VariableDeclaration variableDeclaration = new VariableDeclaration();
                variableDeclaration.AddToken(token);
                PotentialNodes.Add(variableDeclaration);
            }
            else if (IsBuiltInFunc(token.Text))
            {
                Call call = new Call();
                call.AddToken(token);
                PotentialNodes.Add(call);
            }
        }
    }

    public override bool IsValid()
    {
        return Children.Count > 0 &&
               Children.All(child => child.IsValid());
    }

    private static bool IsBuiltInVarType(string type)
    {
        return VariableDeclaration.BuiltInTypes.Contains(type);
    }

    private static bool IsBuiltInFunc(string func)
    {
        return Call.BuiltInFunctions.Contains(func);
    }
}