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
                if (node.IsValid(out _))
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

    public override bool IsValid(out string[] errors)
    {
        List<string> errorMessages = new List<string>();
        if (Children.Count == 0)
        {
            foreach (var node in PotentialNodes)
            {
                if (!node.IsValid(out string[] nodeErrors))
                {
                    errorMessages.AddRange(nodeErrors);
                }
            }

            if (PotentialNodes.Count == 0 && Tokens.Count > 0)
            {
                if (Tokens[0].Kind == TokenKind.Identifier)
                {
                    errorMessages.Add(
                        $"Unknown type '{Tokens[0].Text}' at line: {Tokens[0].Line} column: {Tokens[0].Column}");
                }
                else
                {
                    errorMessages.Add(
                        $"Unexpected token '{Tokens[0].Text}' at line: {Tokens[0].Line} column: {Tokens[0].Column}");
                }
            }

            errors = errorMessages.ToArray();
            return false;
        }

        bool valid = true;
        foreach (var child in Children)
        {
            if (!child.IsValid(out string[] nodeErrors))
            {
                errorMessages.AddRange(nodeErrors);
                valid = false;
            }
        }


        errors = errorMessages.ToArray();
        return valid;
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