namespace PixiScript.Syntax;

public abstract class SyntaxNode
{
    public List<SyntaxNode> Children { get; protected set; } = new List<SyntaxNode>();
    public abstract List<SyntaxRule>? Rules { get; }

    public List<Token> Tokens { get; protected set; } = new List<Token>();
    public int TokenPosition { get; set; } = 0;

    public virtual bool IsValid(out string[]? errors)
    {
        if (Rules == null || Rules.Count == 0)
        {
            errors = null;
            return true;
        }

        foreach (var rule in Rules)
        {
            if (!rule.IsValid(Tokens, out errors))
            {
                return false;
            }
        }

        errors = null;
        return true;
    }

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