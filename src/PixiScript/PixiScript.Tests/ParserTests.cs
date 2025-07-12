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
    public void TestPrint()
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
}