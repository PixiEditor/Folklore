using System.Runtime.CompilerServices;

namespace PixiScript.Rules;

public class EndsWithEndOfLine : SyntaxRule
{
    public EndsWithEndOfLine()
    {
    }

    public override bool IsValid(List<Token> tokens, out string[]? errors)
    {
        if (tokens.Count == 0)
        {
            errors = new[] { $"Missing ';'" };
            return false;
        }

        if (tokens[^1].Kind != TokenKind.EndOfLine)
        {
            errors = new[] { $"{LineInfo(tokens[^1])}';' missing" };
            return false;
        }

        errors = null;
        return true;
    }
}