using Folklore.Rules;

namespace Folklore.Syntax;

public class Call : SyntaxNode
{
    public static readonly string[] BuiltInFunctions = new[]
    {
        "log"
    };

    public override List<SyntaxRule>? Rules { get; }

    public string FunctionName => Tokens.Count > 0 ? Tokens[0].Text : string.Empty;
    public List<string> Arguments { get; } = new List<string>();

    public Call()
    {
        // Define rules for the Call syntax node
        Rules = new List<SyntaxRule>
        {
            new EndsWithEndOfLine(),
            new TokenTextIsOneOf(BuiltInFunctions, 0),
            new TokenAtIndexIs(TokenKind.OpenPassingScope, 1),
            new TokenAtIndexIs(TokenKind.ClosePassingScope, -2),
        };
    }

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

    public override bool IsValid(out string[]? errors)
    {
        if (!base.IsValid(out errors))
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
                    errors = new[]
                    {
                        $"Expected separator ',' at line: {Tokens[i].Line} column: {Tokens[i].Column}, but found '{Tokens[i].Text}'"
                    };
                    return false;
                }

                continue;
            }

            if (tokenKind != TokenKind.Identifier && tokenKind != TokenKind.Literal)
            {
                errors = new[]
                {
                    $"Expected identifier or literal at line: {Tokens[i].Line} column: {Tokens[i].Column}, but found '{Tokens[i].Text}'"
                };
                return false;
            }
        }

        errors = null;
        return true;
    }
}