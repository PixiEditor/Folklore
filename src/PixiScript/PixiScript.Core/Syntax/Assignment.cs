using PixiScript.Logical;

namespace PixiScript.Syntax;

public class Assignment : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }
    public Reference AssignTo { get; set; }
    public string? AssignedConstantLiteral { get; private set; }
    public Reference? AssignedReference { get; private set; }

    public Assignment(Reference assignTo)
    {
        AssignTo = assignTo;
    }

    protected override void OnTokenAdded(Token token)
    {
        if (token.Kind == TokenKind.Literal)
        {
            AssignedConstantLiteral = token.Text;
        }
        else if (token.Kind == TokenKind.Identifier)
        {
            AssignedReference = new Reference(token.Text);
        }
    }

    public override bool IsValid()
    {
        bool hasVariable = AssignTo != null;
        return hasVariable && (
            (AssignedConstantLiteral != null && AssignedReference == null) ||
            (AssignedConstantLiteral == null && AssignedReference != null)
        );
    }
}