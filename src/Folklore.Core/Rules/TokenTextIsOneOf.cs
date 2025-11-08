namespace Folklore.Rules;

public class TokenTextIsOneOf : SyntaxRule
{
    public IEnumerable<string> OneOf { get; }
    public int Index { get; set; }

    public TokenTextIsOneOf(IEnumerable<string> oneOf, int indexOfToken)
    {
        OneOf = oneOf;
        Index = indexOfToken;
    }

    public override bool IsValid(List<Token> tokens, out string[]? errors)
    {
        if (tokens.Count <= Index)
        {
            errors = new[] { $"Invalid amount of tokens. Expected at least {Index + 1}, but got {tokens.Count}." };
            return false;
        }

        var token = tokens[Index];
        if (!OneOf.Contains(token.Text))
        {
            errors = new[] { $"{token.Kind} '{token.Text}' at position {Index} is not one of the expected values: {string.Join(", ", OneOf)}." };
            return false;
        }

        errors = null;
        return true;
    }
}