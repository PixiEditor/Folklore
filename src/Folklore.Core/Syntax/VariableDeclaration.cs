using System.Reflection;
using Folklore.Attributes;
using Folklore.Logical;
using Folklore.Rules;
using Folklore.Types;

namespace Folklore.Syntax;

public class VariableDeclaration : SyntaxNode
{
    public static readonly Type[] BuiltInTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
        t.IsSubclassOf(typeof(FolkloreType)) && t.GetCustomAttribute<SyntaxKeywordAttribute>() != null).ToArray();

    public static readonly IReadOnlyDictionary<string, Type> BuiltInTypeMap = BuiltInTypes.ToDictionary(
        t => t.GetCustomAttribute<SyntaxKeywordAttribute>()?.Keyword ?? string.Empty,
        t => t);

    public override List<SyntaxRule>? Rules { get; }

    public FolkloreType VariableType { get; private set; }
    public string VariableName => Tokens.Count > 1 ? Tokens[1].Text : string.Empty;

    private Assignment? potentialAssignmentNode;

    private Dictionary<string, Reference> referencesInScope;

    public VariableDeclaration(Dictionary<string, Reference> referencesInScope)
    {
        this.referencesInScope = referencesInScope ?? new Dictionary<string, Reference>();
        Rules = new List<SyntaxRule>
        {
            new EndsWithEndOfLine(),
            new DependsOn<Assignment>(() => potentialAssignmentNode).PassOnNull(true),

            new TokenSchema(
                TokenKind.Identifier | TokenKind.Keyword,
                TokenKind.Identifier,
                TokenKind.Assignment | TokenKind.EndOfLine)
                .AllowOverflow(2, last => last == TokenKind.Assignment),

            new WhenTokenIs(TokenKind.Keyword, 0)
                .Then(new TokenTextIsOneOf(BuiltInTypeMap.Keys.ToArray(), 0))
        };
    }

    protected override void OnTokenAdded(Token token)
    {
        if (TokenPosition == 1)
        {
            if (BuiltInTypeMap.TryGetValue(token.Text, out var type))
            {
                VariableType = (FolkloreType)Activator.CreateInstance(type)!;
            }
        }
        if (potentialAssignmentNode is not null)
        {
            potentialAssignmentNode.AddToken(token);
            if (potentialAssignmentNode.IsValid(out _))
            {
                Children.Add(potentialAssignmentNode);
                potentialAssignmentNode = null;
            }

            return;
        }

        if (token is { Kind: TokenKind.Assignment } && Tokens.Count == 3)
        {
            Assignment assignment = new Assignment(new Reference(Tokens[1].Text, VariableType), referencesInScope);
            potentialAssignmentNode = assignment;
        }
    }
}