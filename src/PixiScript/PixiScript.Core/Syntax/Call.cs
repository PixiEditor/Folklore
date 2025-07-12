namespace PixiScript.Syntax;

public class Call : SyntaxNode
{
    public static readonly string[] BuiltInFunctions = new[]
    {
        "log"
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

    public override bool IsValid(out string[] errors)
    {
        if (BuiltInFunctions.Contains(FunctionName))
        {
            if (Tokens[1].Kind != TokenKind.OpenPassingScope)
            {
                errors = new[] { $"Function '{FunctionName}' expects an open passing scope '(', but found '{Tokens[1].Text}' at line: {Tokens[1].Line} column: {Tokens[1].Column}" };
                return false;
            }

            if (Tokens[^2].Kind != TokenKind.ClosePassingScope)
            {
                errors = new[] { $"Function '{FunctionName}' expects a close passing scope ')', but found '{Tokens[^2].Text}' at line: {Tokens[^2].Line} column: {Tokens[^2].Column}" };
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
                        errors = new[] { $"Expected separator ',' at line: {Tokens[i].Line} column: {Tokens[i].Column}, but found '{Tokens[i].Text}'" };
                        return false;
                    }

                    continue;
                }

                if (tokenKind != TokenKind.Identifier && tokenKind != TokenKind.Literal)
                {
                    errors = new[] { $"Expected identifier or literal at line: {Tokens[i].Line} column: {Tokens[i].Column}, but found '{Tokens[i].Text}'" };
                    return false;
                }
            }
        }

        errors = [];
        return true;
    }
}