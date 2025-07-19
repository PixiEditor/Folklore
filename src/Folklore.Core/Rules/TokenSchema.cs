namespace Folklore.Rules;

public class TokenSchema : SyntaxRule
{
    public Func<TokenKind, bool> CanOverflowAction { get; private set; } = _ => false;
    public TokenKind[] Schema { get; }

    public int TokenIndex { get; private set; } = -1;

    public TokenSchema(params TokenKind[] schema)
    {
        Schema = schema;
    }

    public override bool IsValid(List<Token> tokens, out string[]? errors)
    {
        for (int i = 0; i < Schema.Length; i++)
        {
            if (i >= tokens.Count || !Schema[i].HasFlag(tokens[i].Kind))
            {
                errors = [$"Expected {Schema[i]} at position {i}, but found {tokens.ElementAtOrDefault(i)}."];
                return false;
            }
        }

        if (!CanOverflowAction(tokens[TokenIndex].Kind) && tokens.Count > Schema.Length)
        {
            errors = new[]
            {
                $"Unexpected tokens found after schema match: {string.Join(", ", tokens.Skip(Schema.Length).Select(t => t.Text))}."
            };
            return false;
        }

        errors = null;
        return true;
    }

    public SyntaxRule AllowOverflow(int tokenIndex, Func<TokenKind, bool> canOverflowAction)
    {
        TokenIndex = tokenIndex;
        CanOverflowAction = canOverflowAction;
        return this;
    }
}