namespace Folklore;

public class Parser
{
    public static SyntaxTree? Parse(string input, out string[]? errors)
    {
        Tokenizer tokenizer = new Tokenizer(input);
        Syntaxer syntaxer = new Syntaxer(tokenizer);

        return syntaxer.Parse(out errors);
    }
}