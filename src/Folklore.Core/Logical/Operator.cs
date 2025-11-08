namespace Folklore.Logical;

public class Operator
{
    public char Symbol { get; set; }
    public int Precedence { get; set; }

    private static readonly Dictionary<char, int> PrecedenceMap = new Dictionary<char, int>
    {
        { '+', 1 },
        { '-', 1 },
        { '*', 2 },
        { '/', 2 },
        { '%', 2 },
        { '^', 3 },
    };

    public Operator(char symbol)
    {
        Symbol = symbol;
        if (!PrecedenceMap.TryGetValue(symbol, out var value))
        {
            throw new ArgumentException($"Invalid operator symbol: {symbol}");
        }

        Precedence = value;
    }

    public override string ToString()
    {
        return Symbol.ToString();
    }
}