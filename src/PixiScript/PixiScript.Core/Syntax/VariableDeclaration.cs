using PixiScript.Logical;

namespace PixiScript.Syntax;

public class VariableDeclaration : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }

    public string VariableType => Tokens.Count > 0 ? Tokens[0].Text : string.Empty;
    public string VariableName => Tokens.Count > 1 ? Tokens[1].Text : string.Empty;

    private SyntaxNode? potentialAssignmentNode;

    public VariableDeclaration()
    {
    }

    protected override void OnTokenAdded(Token token)
    {
        if (potentialAssignmentNode is not null)
        {
            potentialAssignmentNode.AddToken(token);
            if (potentialAssignmentNode.IsValid())
            {
                Children.Add(potentialAssignmentNode);
                potentialAssignmentNode = null;
            }

            return;
        }

        if (token is { Kind: TokenKind.Assignment } && Tokens.Count == 3)
        {
            Assignment assignment = new Assignment(new Reference(Tokens[1].Text));
            potentialAssignmentNode = assignment;
        }
    }

    public override bool IsValid()
    {
        if(Tokens.Count < 3)
            return false;

        if (Tokens[0].Kind != TokenKind.Keyword)
            return false;

        if(Tokens[1].Kind != TokenKind.Identifier)
            return false;

        return (potentialAssignmentNode?.IsValid() ?? true) &&
               Tokens.Last().Kind == TokenKind.EndOfLine;
    }
}