namespace PixiScript.Syntax;

public class Call : SyntaxNode
{
    public static readonly string[] BuiltInFunctions = new[]
    {
        "print"
    };

    public override List<SyntaxRule>? Rules { get; }

    public string FunctionName => Tokens.Count > 0 ? Tokens[0].Text : string.Empty;
    public List<string> Arguments { get; } = new List<string>();

    protected override void OnTokenAdded(Token token)
    {
        if (Tokens.Count > 1)
        {
            if (token.Kind is TokenKind.Identifier or TokenKind.Literal)
            {
                Arguments.Add(token.Text);
            }
        }
    }

    public override bool IsValid()
    {
        if (Tokens.Count < 1)
            return false;

        if (BuiltInFunctions.Contains(FunctionName))
        {
            if (Tokens.Count < 4 || Tokens.Last().Kind != TokenKind.EndOfLine)
                return false;

            if (Tokens[1].Kind != TokenKind.OpenPassingScope || Tokens[^2].Kind != TokenKind.ClosePassingScope)
            {
                return false;
            }

            for (int i = 2; i < Tokens.Count - 2; i++)
            {
                var tokenKind = Tokens[i].Kind;
                bool separatorExpected = i % 2 != 0;
                if (separatorExpected)
                {
                    if (tokenKind != TokenKind.Separator)
                    {
                        return false;
                    }

                    continue;
                }

                if (tokenKind != TokenKind.Identifier && tokenKind != TokenKind.Literal)
                {
                    return false;
                }
            }
        }

        return true;
    }
}