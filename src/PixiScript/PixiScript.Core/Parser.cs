namespace PixiScript;

public class Parser
{
    public static SyntaxTree? Parse(string input, out string[]? errors)
    {
        Tokenizer tokenizer = new Tokenizer(input);
        Lexer lexer = new Lexer(tokenizer);

        return lexer.Parse(out errors);
    }
}