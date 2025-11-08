using System.Reflection;
using Folklore.Logical;
using Folklore.Types;

namespace Folklore.Syntax;

public class Assignment : SyntaxNode
{
    public override List<SyntaxRule>? Rules { get; }
    public Reference AssignTo { get; set; }
    public Literal? AssignedConstantLiteral { get; private set; }
    public Reference? AssignedReference { get; private set; }

    public ReturnValueSyntaxNode AssignedExpression { get; private set; }
    public Dictionary<string, Reference> ReferencesInScope { get; }

    private static Type[] AllReturnValueNodesTypes = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsSubclassOf(typeof(ReturnValueSyntaxNode)) && !t.IsAbstract)
        .ToArray();


    public Assignment(Reference assignTo, Dictionary<string, Reference> referencesInScope)
    {
        AssignTo = assignTo;
        ReferencesInScope = referencesInScope;
    }

    protected override void OnTokenAdded(Token token)
    {
        if (token.Kind == TokenKind.EndOfLine)
        {
            if (Tokens.Count == 2)
            {
                Token valueToken = Tokens[0];
                if (valueToken.Kind == TokenKind.Literal && Literal.TryParse(valueToken.Text, out var literal))
                {
                    AssignedConstantLiteral = literal;
                }
                else if (valueToken.Kind == TokenKind.Identifier)
                {
                    AssignedReference = new Reference(valueToken.Text, ReferencesInScope[valueToken.Text].Type);
                }
            }
            else
            {
                var potentialNodes = AllReturnValueNodesTypes.Select(t =>
                    (ReturnValueSyntaxNode)Activator.CreateInstance(t)!).ToArray();

                foreach (var node in potentialNodes)
                {
                    if (node is IScopeReferences scopeNode)
                    {
                        scopeNode.ReferencesInScope = ReferencesInScope;
                    }

                    foreach (var tokenToAdd in Tokens)
                    {
                        node.AddToken(tokenToAdd);
                    }

                    if (node.IsValid(out _))
                    {
                        Children.Add(node);

                        AssignedExpression = (ReturnValueSyntaxNode)node;
                        break;
                    }
                }
            }
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

        bool hasValue = AssignedConstantLiteral != null || AssignedReference != null || AssignedExpression != null;
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

            bool typesMatch =
                AssignTo!.Type?.Name.Equals(AssignedReference.Type?.Name, StringComparison.OrdinalIgnoreCase) ?? false;
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
            bool typesMatch =
                AssignTo!.Type?.Name.Equals(AssignedConstantLiteral.Type.Name, StringComparison.OrdinalIgnoreCase) ??
                false;
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