using Folklore.Logical;
using Folklore.Types;

namespace Folklore.Syntax;

public class Assignment : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }
    public Reference AssignTo { get; set; }
    public Literal? AssignedConstantLiteral { get; private set; }
    public Reference? AssignedReference { get; private set; }

    public Dictionary<string, Reference> ReferencesInScope { get; }

    public Assignment(Reference assignTo, Dictionary<string, Reference> referencesInScope)
    {
        AssignTo = assignTo;
        ReferencesInScope = referencesInScope;
    }

    protected override void OnTokenAdded(Token token)
    {
        if (Tokens.Any(x => x.Kind == TokenKind.EndOfLine)) return;
        if (token.Kind == TokenKind.Literal && AssignedConstantLiteral == null)
        {
            if (Literal.TryParse(token.Text, out var literal))
            {
                AssignedConstantLiteral = literal;
            }
        }
        else if (token.Kind == TokenKind.Identifier && AssignedReference == null)
        {
            var reference = ReferencesInScope[token.Text];
            AssignedReference = new Reference(token.Text, reference.Type);
        }
    }

    public override bool IsValid(out string[] errors)
    {
        bool hasVariable = AssignTo != null;

        if (!hasVariable)
        {
            errors = new[] { "Assignment must have a variable to assign to." };
            return false;
        }

        bool hasValue = AssignedConstantLiteral != null || AssignedReference != null;
        if (!hasValue)
        {
            errors = new[] { "Assignment must have a value to assign." };
            return false;
        }

        if (AssignedReference != null)
        {
            bool referenceExists = ReferencesInScope.ContainsKey(AssignedReference.Name);
            if (!referenceExists)
            {
                errors = new[] { $"Reference '{AssignedReference.Name}' does not exist in the current scope." };
                return false;
            }

            bool typesMatch = AssignTo!.Type?.Name.Equals(AssignedReference.Type?.Name, StringComparison.OrdinalIgnoreCase) ?? false;
            if (!typesMatch)
            {
                errors = new[]
                {
                    $"Type mismatch: Cannot assign reference of type '{AssignedReference.Type?.Name}' to variable of type '{AssignTo!.Type!.Name}'."
                };
                return false;
            }
        }
        else if (AssignedConstantLiteral != null)
        {
            bool typesMatch = AssignTo!.Type?.Name.Equals(AssignedConstantLiteral.Type.Name, StringComparison.OrdinalIgnoreCase) ?? false;
            if (!typesMatch)
            {
                errors = new[]
                {
                    $"Type mismatch: Cannot assign literal of type '{AssignedConstantLiteral.Type.Name}' to variable of type '{AssignTo!.Type!.Name}'."
                };
                return false;
            }
        }

        errors = [];
        return true;
    }
}