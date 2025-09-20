namespace Folklore.Tests;

public class LiteralTests
{
    [Fact]
    public void TestThatTextLiteralCantBeAssignedToNumber()
    {
        string code = """
                      number x = "hello";
                      """;
        var syntaxTree = Parser.Parse(code, out var errors);
        Assert.NotNull(errors);
        Assert.Contains(errors, e => e.Contains("Type mismatch"));
    }

    [Fact]
    public void TestThatNumberLiteralCantBeAssignedToText()
    {
        string code = """
                      text x = 123;
                      """;
        var syntaxTree = Parser.Parse(code, out var errors);
        Assert.NotNull(errors);
        Assert.Contains(errors, e => e.Contains("Type mismatch"));
    }

    [Fact]
    public void TestThatReferenceWithOtherTypeCantBeAssignedToText()
    {
        string code = """
                      number someNumber = 10;
                      text x = someNumber;
                      """;
        var syntaxTree = Parser.Parse(code, out var errors);
        Assert.NotNull(errors);
        Assert.Contains(errors, e => e.Contains("Type mismatch") && e.Contains("text") && e.Contains("number"));
    }

    [Fact]
    public void TestThatReferenceWithOtherTypeCantBeAssignedToNumber()
    {
        string code = """
                      text someText = "hello";
                      number x = someText;
                      """;
        var syntaxTree = Parser.Parse(code, out var errors);
        Assert.NotNull(errors);
        Assert.Contains(errors, e => e.Contains("Type mismatch") && e.Contains("text") && e.Contains("number"));
    }
}