using PixiScript.Syntax;

namespace PixiScript.Rules;

public class DependsOn<T> : SyntaxRule where T : SyntaxNode
{
    public bool ValidOnNull { get; set; } = false;
    public Func<T?> NodeGetter { get; protected set; }

    public DependsOn(Func<T?> nodeGetter)
    {
        NodeGetter = nodeGetter;
    }

    public override bool IsValid(List<Token> tokens, out string[]? errors)
    {
        var node = NodeGetter.Invoke();
        if (node is null)
        {
            if (ValidOnNull)
            {
                errors = null;
                return true;
            }

            errors = new[] { $"{GetType().Name} is missing" };
            return false;
        }

        if (!node.IsValid(out errors))
        {
            return false;
        }

        errors = null;
        return true;
    }

    public DependsOn<T> PassOnNull(bool validOnNull = false)
    {
        ValidOnNull = validOnNull;
        return this;
    }
}