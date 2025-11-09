using Folklore.Logical;
using Folklore.Rules;
using Folklore.Types;
using Folklore.Types.Primitive;

namespace Folklore.Syntax;

public class MathExpression : ReturnValueSyntaxNode, IScopeReferences
{
    public override List<SyntaxRule>? Rules { get; }

    public MathTree OperationTree { get; } = new MathTree(new MathTreeNode());

    public readonly string[] ValidOperators =
    [
        "+", "-", "*", "/", "%", "^"
    ];

    public override FolkloreType ReturnType => new NumberType();

    public Dictionary<string, Reference> ReferencesInScope { get; set; }

    private MathTreeNode lastNode;

    public MathExpression()
    {
        Rules = new List<SyntaxRule>()
        {
            new EndsWithEndOfLine().Optional(),
        };

        lastNode = OperationTree.Root;
    }

    protected override void OnTokenAdded(Token newToken)
    {
        if (newToken.Kind == TokenKind.EndOfLine)
        {
            Stack<MathTreeNode> output = new();
            Stack<Operator> operators = new();

            for (var index = 0; index < Tokens.Count; index++)
            {
                var token = Tokens[index];
                if (token.Kind is TokenKind.Identifier or TokenKind.Keyword)
                {
                    if (ReferencesInScope.TryGetValue(token.Text, out var reference))
                    {
                        output.Push(new MathTreeNode
                        {
                            OperandValue = new Operand(new Reference(token.Text, reference.Type))
                        });
                    }
                }
                else if (token.Kind is TokenKind.Literal && Literal.TryParse(token.Text, out var literal))
                {
                    output.Push(new MathTreeNode
                    {
                        OperandValue = new Operand(literal)
                    });
                }
                else if (token.Kind is TokenKind.OpenPassingScope)
                {
                    // Handle function calls or nested expressions
                    var subExpression = new MathExpression();
                    subExpression.ReferencesInScope = ReferencesInScope;
                    int openedScopes = 1;
                    while (index + 1 < Tokens.Count)
                    {
                        if (Tokens[index + 1].Kind is TokenKind.OpenPassingScope)
                        {
                            openedScopes++;
                        }
                        else if (Tokens[index + 1].Kind is TokenKind.ClosePassingScope)
                        {
                            openedScopes--;
                            if (openedScopes == 0)
                            {
                                break;
                            }
                        }
                        
                        index++;
                        subExpression.AddToken(Tokens[index]);
                    }

                    subExpression.OnTokenAdded(new Token { Kind = TokenKind.EndOfLine, Text = ";" });
                    output.Push(subExpression.OperationTree.Root);
                }
                else if (token.Kind == TokenKind.Operator)
                {
                    var op = new Operator(token.Text[0]);
                    while (operators.Count > 0 && operators.Peek().Precedence >= op.Precedence)
                    {
                        var topOp = operators.Pop();
                        var right = output.Pop();
                        var left = output.Pop();
                        output.Push(new MathTreeNode
                        {
                            OperatorValue = topOp,
                            Left = left,
                            Right = right
                        });
                    }

                    operators.Push(op);
                }
            }

            while (operators.Count > 0)
            {
                var topOp = operators.Pop();
                var right = output.Pop();
                var left = output.Pop();
                output.Push(new MathTreeNode
                {
                    OperatorValue = topOp,
                    Left = left,
                    Right = right
                });
            }

            if (output.Count > 0)
            {
                OperationTree.Root = output.Pop();
            }
        }
    }

    public override bool IsValid(out string[]? errors)
    {
        if (!base.IsValid(out errors))
        {
            return false;
        }

        if (OperationTree.Root.IsEmpty)
        {
            errors = new[] { "Math expression must contain at least one operand." };
            return false;
        }

        var errorsInTree = new List<string>();
        OperationTree.Traverse(node =>
        {
            if (node.IsEmpty)
            {
                errorsInTree.Add("Math expression nodes must have either an operator or an operand.");
                return false;
            }

            if (!node.ChildrenFull && node.OperatorValue != null)
            {
                errorsInTree.Add("Math expression operators must have two operands.");
                return false;
            }

            return true;
        });

        if (errorsInTree.Count > 0)
        {
            errors = errorsInTree.ToArray();
            return false;
        }

        return true;
    }
}