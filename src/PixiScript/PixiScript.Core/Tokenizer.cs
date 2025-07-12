using System.Text;
using PixiScript.Attributes;
using PixiScript.Syntax;

namespace PixiScript;

public class Tokenizer
{
    public string Input { get; }
    public int Position { get; private set; }
    public int LineNumber { get; private set; } = 1;
    public int ColumnNumber => Position - Input.LastIndexOf('\n', Position - 1) - 1;

    public Token? CurrentToken { get; private set; }

    private StringBuilder tokenBuilder = new StringBuilder();

    private static List<string> keywords = new List<string>();

    static Tokenizer()
    {
        keywords.AddRange(VariableDeclaration.BuiltInTypes);
        keywords.AddRange(Call.BuiltInFunctions);
    }

    public Tokenizer(string input)
    {
        Input = input;
        Position = 0;
    }

    public void Reset()
    {
        Position = 0;
        tokenBuilder.Clear();
        LineNumber = 1;
    }

    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();
        Reset();

        while (Position < Input.Length)
        {
            Token? token = MoveNext();
            if (token != null)
            {
                tokens.Add(token);
            }
        }

        return tokens;
    }

    public Token? MoveNext()
    {
        if (Position >= Input.Length)
            return null;

        tokenBuilder.Clear();
        Token? identifiedToken = null;

        while (Position < Input.Length && identifiedToken == null)
        {
            do
            {
                char currentChar = Input[Position];

                if (currentChar == '\n')
                {
                    LineNumber++;
                }

                if (tokenBuilder.Length == 0 && char.IsWhiteSpace(currentChar))
                {
                    Position++;
                    continue;
                }

                Position++;
                tokenBuilder.Append(currentChar);

                if (currentChar is '(' or ')') break;

            } while (Position < Input.Length && (char.IsLetterOrDigit(Input[Position]) || Input[Position] == '.' || Input[Position] == '_'));

            if (tokenBuilder.Length == 0 && Position >= Input.Length)
            {
                return null;
            }

            identifiedToken = ParseToken(tokenBuilder);
        }

        CurrentToken = identifiedToken;
        return identifiedToken;
    }

    private Token? ParseToken(StringBuilder tokenBuilder)
    {
        string tokenText = tokenBuilder.ToString();

        if (string.IsNullOrWhiteSpace(tokenText))
            return null;

        TokenKind kind;

        if (tokenBuilder.Length == 1)
        {
            if (char.IsPunctuation(tokenText[0]) || char.IsSymbol(tokenText[0]))
            {
                kind = tokenText[0] switch
                {
                    '(' => TokenKind.OpenPassingScope,
                    ')' => TokenKind.ClosePassingScope,
                    '{' => TokenKind.OpenScope,
                    '}' => TokenKind.CloseScope,
                    ';' => TokenKind.EndOfLine,
                    ',' => TokenKind.Separator,
                    '=' => TokenKind.Assignment,
                    '+' => TokenKind.Operator,
                    '-' => TokenKind.Operator,
                    '*' => TokenKind.Operator,
                    '/' => TokenKind.Operator,
                    '%' => TokenKind.Operator,
                    '!' => TokenKind.Operator,
                    '<' => TokenKind.Operator,
                    '>' => TokenKind.Operator,
                    '&' => TokenKind.Operator,
                    '|' => TokenKind.Operator,
                    '^' => TokenKind.Operator,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else if (char.IsLetter(tokenText[0]) || tokenText[0] == '_')
            {
                kind = TokenKind.Identifier;
            }
            else if (char.IsDigit(tokenText[0]))
            {
                kind = TokenKind.Literal;
            }
            else
            {
                return null; // Invalid single character token
            }
        }
        else if (keywords.Contains(tokenText))
        {
            kind = TokenKind.Keyword;
        }
        else if (char.IsLetter(tokenText[0]) || tokenText[0] == '_')
        {
            kind = TokenKind.Identifier;
        }
        else if (tokenText.All(char.IsDigit) || IsFloatingNumber(tokenText) || IsStringLiteral(tokenText))
        {
            kind = TokenKind.Literal;
        }
        else
        {
            return null; // Invalid token
        }

        Token token = new Token
        {
            Text = tokenText,
            Kind = kind,
            StartPosition = Position - tokenBuilder.Length,
            EndPosition = Position,
            Line = LineNumber,
            Column = ColumnNumber
        };

        return token;
    }

    private static bool IsStringLiteral(string tokenText)
    {
        return (tokenText[0] == '"' && tokenText[^1] == '"' && tokenText.Length > 1);
    }

    private static bool IsFloatingNumber(string tokenText)
    {
        return tokenText.Count(c => c == '.') <= 1 &&
               tokenText.Count(c => c == '-') <= 1 &&
               (tokenText[0] == '-' || char.IsDigit(tokenText[0])) &&
               tokenText.All(c => char.IsDigit(c) || c == '.' || c == '-');
    }

    /*private Token? ParseToken(StringBuilder tokenBuilder, Scope currentScope)
    {
        List<Type> allowedNodes = currentScope.AllowedNodes;
        if (allowedNodes.Count == 0)
            return null;

        var node = FindByReservedKeyword(tokenBuilder, allowedNodes);
        if (node != null)
        {
            return node;
        }

        node = FindByRule(tokenBuilder, allowedNodes);

        return null;
    }*/

    /*private static Token? FindByReservedKeyword(StringBuilder tokenBuilder, List<Type> allowedNodes)
    {
        List<SyntaxKeywordAttribute> allowedNodesAttributes = allowedNodes
            .SelectMany(nodeType => (SyntaxKeywordAttribute[])nodeType.GetCustomAttributes(typeof(SyntaxKeywordAttribute), true))
            .ToList();

        var firstMatchingNode = allowedNodesAttributes.FirstOrDefault(attr => tokenBuilder.ToString() == attr.Keyword);
        if (firstMatchingNode != null)
        {
            Token token = (Token)Activator.CreateInstance(firstMatchingNode.NodeType);
            return token;
        }

        return null;
    }

    private static Token? FindByRule(StringBuilder tokenBuilder, List<Type> allowedNodes)
    {
        foreach (var nodeType in allowedNodes)
        {
            var syntaxRuleAttribute = (SyntaxRuleAttribute)nodeType.GetCustomAttributes(typeof(SyntaxRuleAttribute), true).FirstOrDefault();
            if (syntaxRuleAttribute != null && syntaxRuleAttribute.Regex.IsMatch(tokenBuilder.ToString()))
            {
                Token token = (Token)Activator.CreateInstance(nodeType);
                return token;
            }
        }

        return null;
    }*/
}