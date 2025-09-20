using Folklore.Logical;

namespace Folklore.Syntax;

public class Root : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }

    public List<SyntaxNode> PotentialNodes { get; } = new List<SyntaxNode>();

    public Dictionary<string, Reference> DefinedReferences { get; } = new Dictionary<string, Reference>();

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
                    AddReferences(node);

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

        if (token is { Kind: TokenKind.Keyword })
        {
            if (IsBuiltInVarType(token.Text) && !IsInPotentialNodes(typeof(VariableDeclaration)))
            {
                VariableDeclaration variableDeclaration = new VariableDeclaration(DefinedReferences);
                variableDeclaration.AddToken(token);
                PotentialNodes.Add(variableDeclaration);
            }
            else if (IsBuiltInFunc(token.Text) && !IsInPotentialNodes(typeof(Call)))
            {
                Call call = new Call();
                call.AddToken(token);
                PotentialNodes.Add(call);
            }
        }
    }

    private void AddReferences(SyntaxNode node)
    {
        if (node is VariableDeclaration varDecl)
        {
            DefinedReferences[varDecl.VariableName] =
                new Reference(varDecl.VariableName, varDecl.VariableType);
        }
    }

    public override bool IsValid(out string[]? errors)
    {
        List<string> errorMessages = new List<string>();
        foreach (var node in PotentialNodes)
        {
            if (!node.IsValid(out string[] nodeErrors))
            {
                errorMessages.AddRange(nodeErrors);
            }
        }

        if (errorMessages.Count > 0)
        {
            errors = errorMessages.ToArray();
        }

        if (Children.Count == 0 && PotentialNodes.Count == 0 && Tokens.Count > 0)
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

        foreach (var child in Children)
        {
            if (!child.IsValid(out string[] nodeErrors))
            {
                errorMessages.AddRange(nodeErrors);
            }
        }


        errors = errorMessages.Count == 0 ? null : errorMessages.ToArray();
        return errorMessages.Count == 0;
    }

    private bool IsInPotentialNodes(Type nodeType)
    {
        return PotentialNodes.Any(node => node.GetType() == nodeType);
    }

    private static bool IsBuiltInVarType(string type)
    {
        return VariableDeclaration.BuiltInTypeMap.ContainsKey(type);
    }

    private static bool IsBuiltInFunc(string func)
    {
        return Call.BuiltInFunctions.Contains(func);
    }
}