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
    public void TestMath(string expression, double expected)
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
}