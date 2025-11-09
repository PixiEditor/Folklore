using Folklore.NetILTranspiler;

namespace Folklore.Tests;

public class MathTests
{
    [Theory]
    [InlineData("2 + 2", 4)]
    [InlineData("5 - 3", 2)]
    [InlineData("4 * 2", 8)]
    [InlineData("8 / 4", 2)]
    [InlineData("10 % 3", 1)]
    [InlineData("2 ^ 3", 8)]
    [InlineData("3 + 5 * 2", 13)]
    [InlineData("(3 + 5) * 2", 16)]
    [InlineData("10 - 2 ^ 3", 2)]
    [InlineData("18 / (3 + 3)", 3)]
    [InlineData("2 * (2 * (4 + 4))", 32)]
    [InlineData("(10 / (8 / (3 + 1) - 1))", 10)]
    public void TestLiteralMath(string expression, double expected)
    {
        string code = $"""
                      number n = {expression};
                      log(n);
                      """;

        DotNetILCompiler compiler = new();
        var parsed = Parser.Parse(code, out string[] errors);
        Assert.Null(errors);
        var fn = compiler.DynamicCompile(parsed);

        using var sw = new StringWriter();
        Console.SetOut(sw);
        fn();
        
        var output = sw.ToString().Trim();
        Assert.Equal(expected.ToString(), output);
    }
    
    [Theory]
    [InlineData("a + b", 6)]
    [InlineData("a - b", 2)]
    [InlineData("a * b", 8)]
    [InlineData("a / b", 2)]
    [InlineData("a % b", 0)]
    [InlineData("a ^ b", 16)]
    [InlineData("b + a * b", 10)]
    [InlineData("(b + a) * b", 12)]
    [InlineData("a - b ^ b", 0)]
    [InlineData("a / (a / b)", 2)]
    [InlineData("b * (b * (a + a))", 32)]
    [InlineData("(a / (b / (b + b) - b))", -2.6666666666666665)]
    public void TestReferenceMath(string expression, double expected)
    {
        string code = $"""
                      number a = 4;
                      number b = 2;
                      number n = {expression};
                      log(n);
                      """;

        DotNetILCompiler compiler = new();
        var parsed = Parser.Parse(code, out string[] errors);
        Assert.Null(errors);
        var fn = compiler.DynamicCompile(parsed);

        using var sw = new StringWriter();
        Console.SetOut(sw);
        fn();
        
        var output = sw.ToString().Trim();
        Assert.Equal(expected.ToString(), output);
    }
}