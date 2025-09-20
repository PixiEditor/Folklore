using Folklore.Types;

namespace Folklore.Logical;

public class Literal
{
    public FolkloreType Type { get; set; }
    public string LiteralValue { get; set; }

    public Literal(FolkloreType type, string literalValue)
    {
        Type = type;
        LiteralValue = literalValue;
    }

    public static bool TryParse(string literalValue, out Literal? literal)
    {
        if (int.TryParse(literalValue, out var intValue))
        {
            literal = new Literal<int>(new Types.Primitive.NumberType(), literalValue, intValue);
            return true;
        }
        if (double.TryParse(literalValue, out var doubleValue))
        {
            literal = new Literal<double>(new Types.Primitive.NumberType(), literalValue, doubleValue);
            return true;
        }

        // Try to parse as text (enclosed in quotes)
        if (literalValue.StartsWith('"') && literalValue.EndsWith('"'))
        {
            var textValue = literalValue.Substring(1, literalValue.Length - 2);
            literal = new Literal<string>(new Types.Primitive.TextType(), literalValue, textValue);
            return true;
        }


        literal = null;
        return false;
    }
}

public class Literal<T> : Literal
{
    public T Value { get; set; }

    public Literal(FolkloreType type, string literalValue, T value) : base(type, literalValue)
    {
        Value = value;
    }
}