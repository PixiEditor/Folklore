namespace PixiScript.Syntax;

public abstract class SyntaxNode
{
    public List<SyntaxNode> Children { get; protected set; } = new List<SyntaxNode>();
    public abstract List<SyntaxRule>? Rules { get; }

    public List<Token> Tokens { get; protected set; } = new List<Token>();
    public int TokenPosition { get; set; } = 0;

    public abstract bool IsValid(out string[] errors);

    public void AddToken(Token token)
    {
        Tokens.Add(token);
        TokenPosition++;
        OnTokenAdded(token);
    }

    protected virtual void OnTokenAdded(Token token)
    {
    }
}