namespace Folklore.Rules;

public class TokenAtIndexIs : SyntaxRule
{
    public TokenKind Kind { get; set; }
    public int Index { get; set; }

    public TokenAtIndexIs(TokenKind kind, int index)
    {
        Kind = kind;
        Index = index;
    }

    public override bool IsValid(List<Token> tokens, out string[]? errors)
    {
        if (tokens.Count <= Index)
        {
            errors = new[] { $"Insufficient number of tokens. Expected at least {Index + 1}, but got {tokens.Count}." };
            return false;
        }

        System.Index i = Index < 0 ? ^(-this.Index) : this.Index;
        var token = tokens[i];
        if (token.Kind != Kind)
        {
            errors = [$"{LineInfo(token)}'Expected {Kind} but found '{token.Text}' instead."];
            return false;
        }

        errors = null;
        return true;
    }
}