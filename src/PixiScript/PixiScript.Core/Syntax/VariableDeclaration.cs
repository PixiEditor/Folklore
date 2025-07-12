using PixiScript.Logical;

namespace PixiScript.Syntax;

public class VariableDeclaration : SyntaxNode
{
    public static readonly string[] BuiltInTypes = { "number", "text" };

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
            if (potentialAssignmentNode.IsValid(out _))
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

    public override bool IsValid(out string[] errors)
    {
        if (Tokens.Count < 3)
        {
            errors = new[] { Tokens.Count == 1 ? $"Expected variable name at line: {Tokens[0].Line} column: {Tokens[0].Column}"
                : $"Missing ';' at line: {Tokens.Last().Line} column: {Tokens.Last().Column + 1}" };
            return false;
        }

        if (Tokens[0].Kind != TokenKind.Keyword)
        {
            errors = new[] { $"Expected variable type at line: {Tokens[0].Line} column: {Tokens[0].Column}" };
            return false;
        }

        if (Tokens[1].Kind != TokenKind.Identifier)
        {
            errors = new[] { $"Unexpected token '{Tokens[1].Text}' at line: {Tokens[1].Line} column: {Tokens[1].Column}" };
            return false;
        }

        if(potentialAssignmentNode != null && !potentialAssignmentNode.IsValid(out errors))
        {
            return false;
        }

        bool eolPresent = Tokens.Last().Kind == TokenKind.EndOfLine;
        if (!eolPresent)
        {
            errors = new[] { $"Missing ';' at line: {Tokens.Last().Line} column: {Tokens.Last().Column + 1}" };
            return false;
        }

        if (!BuiltInTypes.Contains(VariableType))
        {
            errors = new[] { $"Unknown variable type '{VariableType}' at line: {Tokens[0].Line} column: {Tokens[0].Column}" };
            return false;
        }

        errors = [];
        return true;
    }
}