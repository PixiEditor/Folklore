using PixiScript.Syntax;

namespace PixiScript;

public class Lexer
{
    public Tokenizer Tokenizer { get; private set; }
    public int Position { get; private set; } = 0;

    public Lexer(Tokenizer tokenizer)
    {
        Tokenizer = tokenizer;
        Position = 0;
    }

    public void Reset()
    {
        Position = 0;
        Tokenizer.Reset();
    }

    public SyntaxTree? Parse(out string[]? errors)
    {
        Reset();
        Root rootNode = new Root();
        Tokenizer.MoveNext();
        do
        {
            if (Tokenizer.CurrentToken == null)
            {
                break;
            }

            rootNode.AddToken(Tokenizer.CurrentToken);
        } while (Tokenizer.MoveNext() != null);

        if (rootNode.IsValid())
        {
            errors = null;
            return new SyntaxTree(rootNode);
        }

        errors = new[] { "Syntax error in the input." };
        return null;

    }
}