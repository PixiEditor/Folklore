using Folklore.Logical;
using Folklore.Rules;

namespace Folklore.Syntax;

public class VariableDeclaration : SyntaxNode
{
    public static readonly string[] BuiltInTypes = { "number", "text" };

    public override List<SyntaxRule>? Rules { get; }

    public string VariableType => Tokens.Count > 0 ? Tokens[0].Text : string.Empty;
    public string VariableName => Tokens.Count > 1 ? Tokens[1].Text : string.Empty;

    private Assignment? potentialAssignmentNode;

    public VariableDeclaration()
    {
        Rules = new List<SyntaxRule>
        {
            new EndsWithEndOfLine(),
            new DependsOn<Assignment>(() => potentialAssignmentNode).PassOnNull(true),

            new TokenSchema(
                TokenKind.Identifier | TokenKind.Keyword,
                TokenKind.Identifier,
                TokenKind.Assignment | TokenKind.EndOfLine)
                .AllowOverflow(2, last => last == TokenKind.Assignment),

            new WhenTokenIs(TokenKind.Keyword, 0)
                .Then(new TokenTextIsOneOf(BuiltInTypes, 0))
        };
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
}