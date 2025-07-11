namespace PixiScript.Syntax;

public class Root : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }

    public List<Type> AllowedNodeTypes { get; } = new()
    {
        typeof(VariableDeclaration),
        typeof(Call)
    };

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
                if(node.IsValid())
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
        if (token is { Kind: TokenKind.Keyword, Text: "number" })
        {
            VariableDeclaration variableDeclaration = new VariableDeclaration();
            variableDeclaration.AddToken(token);
            PotentialNodes.Add(variableDeclaration);
        }
        else if (token.Kind == TokenKind.Keyword && Call.BuiltInFunctions.Contains(token.Text))
        {
            Call call = new Call();
            call.AddToken(token);
            PotentialNodes.Add(call);
        }
    }

    public override bool IsValid()
    {
        return Children.Count > 0 &&
               Children.All(child => child.IsValid());
    }
}