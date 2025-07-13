using PixiScript.Syntax;

namespace PixiScript.Tests;

public class ParserTests
{
    [Fact]
    public void TestBasicVariableDeclaration()
    {
        string syntax = """
                        number someNumber;
                        number anotherNumber;
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.NotNull(parsed);
        Assert.Null(errors);
        Assert.IsType<SyntaxTree>(parsed);
        Assert.NotNull(parsed.Root);
        Assert.Equal(2, parsed.Root.Children.Count);

        Assert.IsType<VariableDeclaration>(parsed.Root.Children[0]);
        Assert.IsType<VariableDeclaration>(parsed.Root.Children[1]);
    }

    [Fact]
    public void TestBasicVariableAssignment()
    {
        string syntax = """
                        number someNumber = 10;
                        number anotherNumber = -50;
                        number thirdNumber = 3.14;
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.NotNull(parsed);
        Assert.Null(errors);
        Assert.IsType<SyntaxTree>(parsed);
        Assert.NotNull(parsed.Root);
        Assert.Equal(3, parsed.Root.Children.Count);

        Assert.IsType<VariableDeclaration>(parsed.Root.Children[0]);
        Assert.IsType<VariableDeclaration>(parsed.Root.Children[1]);
        Assert.IsType<VariableDeclaration>(parsed.Root.Children[2]);

        var firstVariable = (VariableDeclaration)parsed.Root.Children[0];
        Assert.Single(firstVariable.Children);
        Assert.IsType<Assignment>(firstVariable.Children[0]);
        var firstAssignment = (Assignment)firstVariable.Children[0];
        Assert.Equal("someNumber", firstAssignment.AssignTo.Name);
        Assert.Equal("10", firstAssignment.AssignedConstantLiteral);

        var secondVariable = (VariableDeclaration)parsed.Root.Children[1];
        Assert.Single(secondVariable.Children);
        Assert.IsType<Assignment>(secondVariable.Children[0]);
        var secondAssignment = (Assignment)secondVariable.Children[0];
        Assert.Equal("anotherNumber", secondAssignment.AssignTo.Name);
        Assert.Equal("-50", secondAssignment.AssignedConstantLiteral);

        var thirdVariable = (VariableDeclaration)parsed.Root.Children[2];
        Assert.Single(thirdVariable.Children);
        Assert.IsType<Assignment>(thirdVariable.Children[0]);
        var thirdAssignment = (Assignment)thirdVariable.Children[0];
        Assert.Equal("thirdNumber", thirdAssignment.AssignTo.Name);
        Assert.Equal("3.14", thirdAssignment.AssignedConstantLiteral);
    }

    [Fact]
    public void TestLog()
    {
        string syntax = """
                        number someNumber = 10;
                        log(someNumber);
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.NotNull(parsed);
        Assert.Null(errors);
        Assert.IsType<SyntaxTree>(parsed);
        Assert.NotNull(parsed.Root);
        Assert.Equal(2, parsed.Root.Children.Count);

        Assert.IsType<VariableDeclaration>(parsed.Root.Children[0]);
        Assert.IsType<Call>(parsed.Root.Children[1]);

        var firstVariable = (VariableDeclaration)parsed.Root.Children[0];
        Assert.Single(firstVariable.Children);
        Assert.IsType<Assignment>(firstVariable.Children[0]);
        var firstAssignment = (Assignment)firstVariable.Children[0];
        Assert.Equal("someNumber", firstAssignment.AssignTo.Name);
        Assert.Equal("10", firstAssignment.AssignedConstantLiteral);

        var printCall = (Call)parsed.Root.Children[1];
        Assert.Equal("log", printCall.FunctionName);
        Assert.Single(printCall.Arguments);
        Assert.Equal("someNumber", printCall.Arguments[0]);
    }

    [Fact]
    public void TestMissingSemicolon()
    {
        string syntax = """
                        number someNumber = 10
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.Null(parsed);
        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("Line 1, Column 22: ';' missing", errors[0]);
    }

    [Fact]
    public void TestNotFullAssignment()
    {
        string syntax = """
                        number someNumber = ;
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.Null(parsed);
        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("Assignment must have a value to assign.", errors[0]);
    }

    [Fact]
    public void TestMultiLineNotFullAssignment()
    {
        string syntax = """
                        number someNumber1 = 2;
                        number someNumber2 = ;
                        number someNumber3 = 4.2;
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.Null(parsed);
        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("Assignment must have a value to assign.", errors[0]);
    }

    [Fact]
    public void TestInvalidVariableType()
    {
        string syntax = """
                        invalidType someNumber = 10;
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.Null(parsed);
        Assert.NotNull(errors);
        Assert.Single(errors);
        Assert.Equal("Unknown type 'invalidType' at line: 1 column: 11", errors[0]);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("\"Hello World\"")]
    public void TestLiteralLog(string literal)
    {
        string syntax = $"""
                        log({literal});
                        """;

        var parsed = Parser.Parse(syntax, out string[]? errors);
        Assert.NotNull(parsed);
        Assert.Null(errors);
        Assert.IsType<SyntaxTree>(parsed);
        Assert.NotNull(parsed.Root);
        Assert.Single(parsed.Root.Children);

        Assert.IsType<Call>(parsed.Root.Children[0]);
        var printCall = (Call)parsed.Root.Children[0];
        Assert.Equal("log", printCall.FunctionName);
        Assert.Single(printCall.Arguments);
        Assert.Equal(literal, printCall.Arguments[0]);
    }
}