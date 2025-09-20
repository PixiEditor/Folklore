using Folklore.Syntax;
using Folklore.Types;
using Folklore.Types.Primitive;

namespace Folklore.Tests;

public class TypeTests
{
    [Theory]
    [InlineData("number", typeof(NumberType))]
    [InlineData("text", typeof(TextType))]
    public void TestNumberTypeParsing(string typeName, Type expectedType)
    {
        string input = $"{typeName} myVar;";
        var syntaxTree = Folklore.Parser.Parse(input, out var errors);

        Assert.NotNull(syntaxTree);
        Assert.Null(errors);

        var typeNode = syntaxTree.Root.Children.FirstOrDefault();
        Assert.NotNull(typeNode);
        Assert.IsType<VariableDeclaration>(typeNode);

        var varDecl = typeNode as VariableDeclaration;

        Assert.NotNull(varDecl);
        Assert.IsType(expectedType, varDecl.VariableType);
    }
}