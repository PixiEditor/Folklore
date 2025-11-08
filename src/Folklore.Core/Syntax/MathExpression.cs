using Folklore.Logical;
using Folklore.Rules;
using Folklore.Types;
using Folklore.Types.Primitive;

namespace Folklore.Syntax;

public class MathExpression : ReturnValueSyntaxNode, IScopeReferences
{
    public override List<SyntaxRule>? Rules { get; }
    
    public Operand? LeftOperand { get; private set; }
    public Token? Operator { get; private set; }
    public Operand? RightOperand { get; private set; }
    
    public readonly string[] ValidOperators = new[]
    {
        "+",/* "-", "*", "/", "%", "^"*/
    };
    
    public override FolkloreType ReturnType => new NumberType();
    
    public Dictionary<string, Reference> ReferencesInScope { get; set; }
 
    public MathExpression()
    {
        Rules = new List<SyntaxRule>()
        {
            new TokenAtIndexIs(TokenKind.Identifier | TokenKind.Literal, 0),
            new TokenAtIndexIs(TokenKind.Operator, 1),
            new TokenAtIndexIs(TokenKind.Identifier | TokenKind.Literal, 2),
            new EndsWithEndOfLine().Optional(),
        };
    }

    protected override void OnTokenAdded(Token token)
    {
        int position = Tokens.Count - 1;
        if (position == 0)
        {
            LeftOperand = CreateOperand(token);
        }
        else if (position == 1)
        {
            Operator = token;
        }
        else if (position == 2)
        {
            RightOperand = CreateOperand(token);
        }
    }

    public override bool IsValid(out string[]? errors)
    {
        if (!base.IsValid(out errors))
        {
            return false;
        }

        if (LeftOperand == null)
        {
            errors = ["Left operand is missing."];
            return false;
        }
        
        if (Operator == null)
        {
            errors = ["Operator is missing."];
            return false;
        }
        
        if (RightOperand == null)
        {
            errors = ["Right operand is missing."];
            return false;
        }
        
        if (!ValidOperators.Contains(Operator.Text))
        {
            errors = [$"Invalid operator '{Operator.Text}' at line: {Operator.Line} column: {Operator.Column}."];
            return false;
        }
        
        return true;
    }
    
    private Operand? CreateOperand(Token token)
    {
        if(token.Kind == TokenKind.Literal && Literal.TryParse(token.Text, out var literal))
        {
            return new Operand(literal);
        }

        if(token.Kind is TokenKind.Identifier or TokenKind.Keyword && ReferencesInScope.TryGetValue(token.Text, out var reference))
        {
            return new Operand(new Reference(token.Text, reference.Type));
        }

        return null;
    }
}