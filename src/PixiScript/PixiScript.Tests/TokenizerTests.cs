namespace PixiScript.Tests;

public class TokenizerTests
{
    [Fact]
    public void TestBasicStructure()
    {
        string syntax = """
                        void execute()
                        {
                            
                        }
                        """;

        Tokenizer tokenizer = new Tokenizer(syntax);
        var tokens = tokenizer.Tokenize();

        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
        Assert.All(tokens, token => Assert.IsType<Token>(token));
        Assert.Equal(6, tokens.Count);
        Assert.Equal("void", tokens[0].Text);
        Assert.Equal(TokenKind.Keyword, tokens[0].Kind);

        Assert.Equal("execute", tokens[1].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);

        Assert.Equal("(", tokens[2].Text);
        Assert.Equal(TokenKind.OpenPassingScope, tokens[2].Kind);

        Assert.Equal(")", tokens[3].Text);
        Assert.Equal(TokenKind.ClosePassingScope, tokens[3].Kind);

        Assert.Equal("{", tokens[4].Text);
        Assert.Equal(TokenKind.OpenScope, tokens[4].Kind);

        Assert.Equal("}", tokens[5].Text);
        Assert.Equal(TokenKind.CloseScope, tokens[5].Kind);
    }

    [Fact]
    public void TestMultiVariableDeclaration()
    {
        string syntax = """
                        number someNumber;
                        number anotherNumber;
                        """;

        Tokenizer tokenizer = new Tokenizer(syntax);
        var tokens = tokenizer.Tokenize();

        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
        Assert.All(tokens, token => Assert.IsType<Token>(token));
        Assert.Equal(6, tokens.Count);
        Assert.Equal("number", tokens[0].Text);
        Assert.Equal(TokenKind.Keyword, tokens[0].Kind);

        Assert.Equal("someNumber", tokens[1].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);

        Assert.Equal(";", tokens[2].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[2].Kind);

        Assert.Equal("number", tokens[3].Text);
        Assert.Equal(TokenKind.Keyword, tokens[3].Kind);

        Assert.Equal("anotherNumber", tokens[4].Text);
        Assert.Equal(TokenKind.Identifier, tokens[4].Kind);

        Assert.Equal(";", tokens[5].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[5].Kind);
    }

    [Fact]
    public void TestVariableDeclarationAndAssignment()
    {
        string syntax = """
                        number someNumber = 42;
                        """;

        Tokenizer tokenizer = new Tokenizer(syntax);
        var tokens = tokenizer.Tokenize();

        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
        Assert.All(tokens, token => Assert.IsType<Token>(token));
        Assert.Equal(5, tokens.Count);
        Assert.Equal("number", tokens[0].Text);
        Assert.Equal(TokenKind.Keyword, tokens[0].Kind);

        Assert.Equal("someNumber", tokens[1].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);

        Assert.Equal("=", tokens[2].Text);
        Assert.Equal(TokenKind.Assignment, tokens[2].Kind);

        Assert.Equal("42", tokens[3].Text);
        Assert.Equal(TokenKind.Literal, tokens[3].Kind);

        Assert.Equal(";", tokens[4].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[4].Kind);
    }

    [Fact]
    public void TestMultipleVariableDeclarationAndAssignment()
    {
        string syntax = """
                        number someNumber = 1;
                        number anotherNumber = 1.1;
                        """;

        Tokenizer tokenizer = new Tokenizer(syntax);
        var tokens = tokenizer.Tokenize();

        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
        Assert.All(tokens, token => Assert.IsType<Token>(token));
        Assert.Equal(10, tokens.Count);
        Assert.Equal("number", tokens[0].Text);
        Assert.Equal(TokenKind.Keyword, tokens[0].Kind);

        Assert.Equal("someNumber", tokens[1].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);

        Assert.Equal("=", tokens[2].Text);
        Assert.Equal(TokenKind.Assignment, tokens[2].Kind);

        Assert.Equal("1", tokens[3].Text);
        Assert.Equal(TokenKind.Literal, tokens[3].Kind);

        Assert.Equal(";", tokens[4].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[4].Kind);

        Assert.Equal("number", tokens[5].Text);
        Assert.Equal(TokenKind.Keyword, tokens[5].Kind);

        Assert.Equal("anotherNumber", tokens[6].Text);
        Assert.Equal(TokenKind.Identifier, tokens[6].Kind);

        Assert.Equal("=", tokens[7].Text);
        Assert.Equal(TokenKind.Assignment, tokens[7].Kind);

        Assert.Equal("1.1", tokens[8].Text);
        Assert.Equal(TokenKind.Literal, tokens[8].Kind);

        Assert.Equal(";", tokens[9].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[9].Kind);
    }

    [Fact]
    public void TestFunctionCallTokenizer()
    {
        string syntax = """
                            log("Hello, World!");
                        """;

        Tokenizer tokenizer = new Tokenizer(syntax);
        var tokens = tokenizer.Tokenize();
        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
        Assert.All(tokens, token => Assert.IsType<Token>(token));
        Assert.Equal(5, tokens.Count);
        Assert.Equal("log", tokens[0].Text);
        Assert.Equal(TokenKind.Keyword, tokens[0].Kind);
        Assert.Equal("(", tokens[1].Text);
        Assert.Equal(TokenKind.OpenPassingScope, tokens[1].Kind);
        Assert.Equal("\"Hello, World!\"", tokens[2].Text);
        Assert.Equal(TokenKind.Literal, tokens[2].Kind);
        Assert.Equal(")", tokens[3].Text);
        Assert.Equal(TokenKind.ClosePassingScope, tokens[3].Kind);
        Assert.Equal(";", tokens[4].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[4].Kind);
    }

    [Fact]
    public void TestFunctionCallTokenizerWithVarDeclaration()
    {
        string syntax = """
                        text myText = "Hello, World!";
                        log(myText);
                        """;

        Tokenizer tokenizer = new Tokenizer(syntax);
        var tokens = tokenizer.Tokenize();
        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
        Assert.All(tokens, token => Assert.IsType<Token>(token));
        Assert.Equal(10, tokens.Count);
        Assert.Equal("text", tokens[0].Text);
        Assert.Equal(TokenKind.Keyword, tokens[0].Kind);
        Assert.Equal("myText", tokens[1].Text);
        Assert.Equal(TokenKind.Identifier, tokens[1].Kind);
        Assert.Equal("=", tokens[2].Text);
        Assert.Equal(TokenKind.Assignment, tokens[2].Kind);
        Assert.Equal("\"Hello, World!\"", tokens[3].Text);
        Assert.Equal(TokenKind.Literal, tokens[3].Kind);
        Assert.Equal(";", tokens[4].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[4].Kind);
        Assert.Equal("log", tokens[5].Text);
        Assert.Equal(TokenKind.Keyword, tokens[5].Kind);
        Assert.Equal("(", tokens[6].Text);
        Assert.Equal(TokenKind.OpenPassingScope, tokens[6].Kind);
        Assert.Equal("myText", tokens[7].Text);
        Assert.Equal(TokenKind.Identifier, tokens[7].Kind);
        Assert.Equal(")", tokens[8].Text);
        Assert.Equal(TokenKind.ClosePassingScope, tokens[8].Kind);
        Assert.Equal(";", tokens[9].Text);
        Assert.Equal(TokenKind.EndOfLine, tokens[9].Kind);
    }
}