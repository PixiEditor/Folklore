using Folklore.Syntax;

namespace Folklore;

public class Syntaxer
{
    public Tokenizer Tokenizer { get; private set; }
    public int Position { get; private set; } = 0;

    public Syntaxer(Tokenizer tokenizer)
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

        if (rootNode.IsValid(out errors))
        {
            return new SyntaxTree(rootNode);
        }

        return null;

    }
}