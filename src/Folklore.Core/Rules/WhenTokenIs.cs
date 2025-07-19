namespace Folklore.Rules;

public class WhenTokenIs : SyntaxRule
{
    public TokenKind Kind { get; set; }
    public int Position { get; set; }
    public SyntaxRule Rule { get; set; }

    public WhenTokenIs(TokenKind kind, int atPosition)
    {
        Kind = kind;
        Position = atPosition;
    }

    public WhenTokenIs Then(SyntaxRule rule)
    {
        Rule = rule;
        return this;
    }

    public override bool IsValid(List<Token> tokens, out string[]? errors)
    {
        if (tokens.Count <= Position)
        {
            errors = null;
            return true;
        }

        var token = tokens[Position];
        if (token.Kind != Kind)
        {
            errors = null;
            return true;
        }

        if (!Rule.IsValid(tokens, out errors))
        {
            return false;
        }

        return true;
    }
}