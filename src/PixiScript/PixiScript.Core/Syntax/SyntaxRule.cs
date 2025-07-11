using PixiScript.Syntax;

namespace PixiScript;

public abstract class SyntaxRule
{
    public bool IsRequired { get; set; } = true;


    public SyntaxRule Optional()
    {
        IsRequired = false;
        return this;
    }

    public abstract bool IsValid(SyntaxNode node);
}